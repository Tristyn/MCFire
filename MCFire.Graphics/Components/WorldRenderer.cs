using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using MCFire.Common;
using MCFire.Common.Coordinates;
using MCFire.Common.Processing;
using MCFire.Graphics.Editor;
using MCFire.Graphics.Editor.Modules.Meshalyzer;
using MCFire.Graphics.Infrastructure.Extensions;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Components
{
    /// <summary>
    /// Generates the meshes for and renders a section of a world.
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class WorldRenderer : ICleanup
    {
        IEditorGame _game;
        VertexLitEffect _vertexLit;

        // TODO: long running thread pool thread
        Thread _meshingThread;
        readonly ChunkPrioritizer _prioritizer = new ChunkPrioritizer();
        readonly ConcurrentQueue<ChunkMeshPositionPair> _chunksToIntegrate = new ConcurrentQueue<ChunkMeshPositionPair>();
        readonly Dictionary<ChunkPosition, IChunkMesh> _chunks = new Dictionary<ChunkPosition, IChunkMesh>();

        ChunkCreationPolicy _policy;
        Matrix _worldMatrix;
        bool _worldMatrixChanged;

        [ImportMany, UsedImplicitly]
        IEnumerable<IMeshalyzer> _meshalyzers;
        [CanBeNull]
        IMeshalyzer _currentMeshalyzer;

        private BoxSelection? _inclusiveZone;

        public WorldRenderer()
        {
            WorldMatrix = Matrix.Identity;
        }

        public void LoadContent([NotNull] IEditorGame game)
        {
            _game = game;
            _vertexLit = new VertexLitEffect(game.LoadContent<Effect>("VertexLit"));

            // TODO: settings
            var mesher = _meshalyzers.LastOrDefault();
            if (mesher == null) return;

            SwitchMeshalyzer(mesher);
            Policy = ChunkCreationPolicy.Run;
            game.World.ChunksModified += ChunksModified;
        }

        public void UnloadContent()
        {
            Policy = ChunkCreationPolicy.Idle;
            _vertexLit.Dispose();

            if (_currentMeshalyzer != null)
                _currentMeshalyzer.UnloadContent(_game);
            _game.World.ChunksModified -= ChunksModified;
        }

        public void Dispose()
        {
            UnloadContent();
        }

        public void Draw([NotNull] GameTime time)
        {
            IntegrateNewChunks();

            _game.GraphicsDevice.SetBlendState(_game.GraphicsDevice.BlendStates.Opaque);
            // TODO: scissor rectangles http://msdn.microsoft.com/en-us/library/windows/desktop/bb205126(v=vs.85).aspx
            // the chunk will hold a bounding box, the square will be calculated based on that box.
            if (_worldMatrixChanged)
                foreach (var chunk in _chunks.Values)
                {
                    chunk.World = _worldMatrix;
                    chunk.Draw(_game);
                }
            else
                foreach (var chunk in _chunks.Values)
                {
                    // TODO: konami code easter egg or something :3
                    // uncomment this for a good time

                    //var secs = time.TotalGameTime.TotalSeconds / 16;
                    //if (Game.World.Title == "Floating island survival V 1_0")
                    //    secs *= 16;
                    //// chunk specific offset
                    //var o = chunk.ModelOrigin / 16;

                    //var sin = Math.Sin(secs * o.X);
                    //var cos = Math.Cos(secs * o.Z);
                    //var x = Matrix.RotationX((float)sin);
                    //var y = Matrix.RotationY((float)cos);
                    //var z = Matrix.RotationZ((float)(cos + Math.Sin(secs / 3) + Math.Cos(3 * secs / 5 + .5d)));
                    //var world = x * y * z;

                    //chunk.World = (world * o.X + world * o.Z);

                    chunk.Draw(_game);
                }

            _worldMatrixChanged = false;
        }

        void SwitchMeshalyzer([NotNull] IMeshalyzer newMeshalyzer)
        {
            var originalPolicy = Policy;

            // stop meshalyzer and unload content
            Policy = ChunkCreationPolicy.Idle;
            var current = _currentMeshalyzer;
            if (current != null)
                current.UnloadContent(_game);

            // load content
            newMeshalyzer.LoadContent(_game);
            _currentMeshalyzer = newMeshalyzer;

            // start meshing
            if (originalPolicy != ChunkCreationPolicy.Run)
                return;
            Policy = ChunkCreationPolicy.Run;
            /* TODO: leverage multiple cores. some considerations: 
             * a chunk will be generated multiple times unless a thread 
             * can claim that point and it wont be picked again. 
             * mabey have a more sophisticated chunk selection system.
             */
            _meshingThread = new Thread(MeshLoop);
            _meshingThread.Start();
        }

        void ChunksModified([NotNull] object sender, [NotNull] ChunksModifiedEventArgs chunksModifiedEventArgs)
        {
            foreach (var pos in chunksModifiedEventArgs.ModifiedChunks)
            {
                _prioritizer.ResetChunk(pos);
            }
        }

        void IntegrateNewChunks()
        {
            while (true)
            {
                ChunkMeshPositionPair chunk;
                if (!_chunksToIntegrate.TryDequeue(out chunk)) break;

                // Add it to the dictionary using its position as a key.
                _chunks[chunk.Position] = chunk.Mesh;
            }
        }

        /// <summary>
        /// Creates chunks on a background thread until policy changes.
        /// </summary>
        void MeshLoop()
        {
            Thread.CurrentThread.IsBackground = true;
            Thread.CurrentThread.Name = "Meshalyzer - " + _game.World.Title;
            var meshalyzer = _currentMeshalyzer;
            if (meshalyzer == null)
            {
                Debug.Assert(false);
                return;
            }

            // TODO: Meshalyzer should be refactored to spit out basic info (coords, colors) only.
            // An intermediate object would package it into a buffer
            while (Policy == ChunkCreationPolicy.Run)
            {
                ChunkPosition pos;
                if (!_prioritizer.GetNextDesiredChunk(_game.Camera.ChunkPosition, out pos))
                {
                    Thread.Sleep(1000);
                    continue;
                }

                var box = new BoxSelection(pos, new ChunkSize(16, 256, 16));
                var chunkVolume = new BlockSelection(box, _game.Dimension, _game.World);
                var buffer = meshalyzer.Meshalyze(_game, chunkVolume);

                var mesh = new ChunkMesh(chunkVolume.Selection.Lesser.AsVector3(), _vertexLit, buffer);
                _chunksToIntegrate.Enqueue(new ChunkMeshPositionPair(mesh, pos));
            }
        }

        /// <summary>
        /// The zone that will be created and rendered. Null to set no restriction.
        /// </summary>
        public BoxSelection? InclusiveZone
        {
            get { return _inclusiveZone; }
            set
            {
                var current = _inclusiveZone;
                _inclusiveZone = value;
            }
        }

        public Matrix WorldMatrix
        {
            get { return _worldMatrix; }
            set
            {
                _worldMatrix = value;
                _worldMatrixChanged = true;
            }
        }

        public ChunkCreationPolicy Policy
        {
            get { return _policy; }
            set
            {
                // stop/start meshing if it changes
                var current = _policy;
                if (current == value)
                    return;
                _policy = value;

                switch (value)
                {
                    case ChunkCreationPolicy.Idle:
                        if (_meshingThread != null)
                            _meshingThread.Join();
                        if (_currentMeshalyzer != null)
                            _currentMeshalyzer.UnloadContent(_game);
                        break;
                    case ChunkCreationPolicy.Run:
                        // start only if a meshalyzer is set, and it isn't already running
                        if (_currentMeshalyzer == null && _meshingThread != null)
                            return;

                        _meshingThread = new Thread(MeshLoop);
                        _meshingThread.Start();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("value");
                }
            }
        }

        struct ChunkMeshPositionPair
        {
            public ChunkMeshPositionPair([NotNull] IChunkMesh mesh, ChunkPosition position)
                : this()
            {
                Mesh = mesh;
                Position = position;
            }

            [NotNull]
            public IChunkMesh Mesh { get; private set; }
            public ChunkPosition Position { get; private set; }
        }
    }
}
