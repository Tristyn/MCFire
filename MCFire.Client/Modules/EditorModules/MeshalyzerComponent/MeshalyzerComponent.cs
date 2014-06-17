using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Caliburn.Micro;
using JetBrains.Annotations;
using MCFire.Client.Services.Explorer.Messages;
using MCFire.Common.Coordinates;
using MCFire.Graphics.Editor;
using MCFire.Graphics.Editor.Modules.Meshalyzer;
using MCFire.Graphics.Editor.Tools.BoxSelector;
using SharpDX;
using SharpDX.Toolkit;
using Substrate.Core;

namespace MCFire.Client.Modules.EditorModules.MeshalyzerComponent
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IGameComponent))]
    public class MeshalyzerComponent : GameComponentBase, IHandle<ChunkModifiedMessage>
    {
        Thread _meshingThread;
        ChunkCreationPolicy _policy;
        bool _disposed;

        readonly ChunkPrioritizer _prioritizer = new ChunkPrioritizer();
        readonly ConcurrentQueue<ChunkMeshPositionPair> _chunksToIntegrate = new ConcurrentQueue<ChunkMeshPositionPair>();
        readonly Dictionary<ChunkPosition, IChunkMesh> _chunks = new Dictionary<ChunkPosition, IChunkMesh>();

        [Import]
        IEventAggregator Aggregator { set { value.Subscribe(this); } }
        [ImportMany, UsedImplicitly]
        IEnumerable<IMeshalyzer> _meshalyzers;
        [CanBeNull]
        IMeshalyzer _currentMeshalyzer;

        // TODO: use new meshalying system (BlockMeshalyzer)
        // this comment is so old that it should be in IMeshalyzer

        protected override void LoadContent()
        {
            // TODO: settings
            var mesher = _meshalyzers.LastOrDefault();
            if (mesher == null) return;
            SwitchMeshalyzer(mesher);
        }

        public override void UnloadContent()
        {
            if (_currentMeshalyzer != null)
                _currentMeshalyzer.UnloadContent(Game);
        }

        void StopMeshalyzer()
        {
            _policy = ChunkCreationPolicy.Idle;
            if (_meshingThread != null)
                _meshingThread.Join();
            if (_currentMeshalyzer != null)
                _currentMeshalyzer.UnloadContent(Game);
        }

        void SwitchMeshalyzer([NotNull] IMeshalyzer newMeshalyzer)
        {
            // load content
            StopMeshalyzer();
            var current = _currentMeshalyzer;
            if (current != null)
                current.UnloadContent(Game);
            newMeshalyzer.LoadContent(Game);
            _currentMeshalyzer = newMeshalyzer;

            // start meshing
            if (_meshingThread != null && _meshingThread.IsAlive)
                return;
            /* TODO: leverage multiple cores. some considerations: 
             * a chunk will be generated multiple times unless a thread 
             * can claim that point and it wont be picked again. 
             * mabey have a more sophisticated chunk selection system.
             */
            _meshingThread = new Thread(MeshLoop);
            _meshingThread.Start();
        }

        public override void Draw([NotNull] GameTime time)
        {
            if (_disposed)
                throw new ObjectDisposedException("MeshalyzerComponent");

            IntegrateNewChunks();

            GraphicsDevice.SetBlendState(GraphicsDevice.BlendStates.Opaque);
            // TODO: scissor rectangles http://msdn.microsoft.com/en-us/library/windows/desktop/bb205126(v=vs.85).aspx
            // the chunk will hold a bounding box, the square will be calculated based on that box.
            foreach (var chunk in _chunks.Values)
            {
                chunk.Draw(Game);
            }
        }

        public void Handle([NotNull] ChunkModifiedMessage message)
        {
            var pos = message.Position;
            if (message.World != World ||
                pos.Dimension != Dimension) return;
            _prioritizer.ChunkNeedsRegen(pos);
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
            Thread.CurrentThread.Name = "Meshalyzer - " + Game.World.Title;
            _policy = ChunkCreationPolicy.Run;
            var meshalyzer = _currentMeshalyzer;
            if (meshalyzer == null)
            {
                Debug.Assert(false);
                return;
            }

            // TODO: Meshalyzer should be refactored to spit out basic info (coords, colors) only.
            // An intermediate object would package it into a buffer
            while (_policy == ChunkCreationPolicy.Run)
            {
                ChunkPosition pos;
                if (!_prioritizer.GetNextDesiredChunk(Game.Camera.ChunkPosition, out pos))
                {
                    Thread.Sleep(1000);
                    continue;
                }
                var chunkLesser = new BlockPosition(pos,
                    new LocalBlockPosition(0, 0, 0), new ChunkSize(16, 256, 16));
                var chunkGreater = new BlockPosition(pos,
                    new LocalBlockPosition(15, 255, 15), new ChunkSize(16, 256, 16));
                var chunkVolume = new BlockSelection(new BoxSelection(chunkLesser, chunkGreater), Dimension, World);
                var drawable = meshalyzer.Meshalyze(Game, chunkVolume,
                    new Vector3(chunkLesser.X, chunkLesser.Y, chunkLesser.Z));

                _chunksToIntegrate.Enqueue(new ChunkMeshPositionPair(drawable, pos));
            }
        }

        public override void Dispose()
        {
            if (_disposed)
                return;

            StopMeshalyzer();
            foreach (var meshalyzer in _meshalyzers)
            {
                meshalyzer.Dispose();
            }

            ChunkMeshPositionPair chunk;
            while (_chunksToIntegrate.TryDequeue(out chunk))
            {
                chunk.Mesh.Dispose();
            }

            foreach (var keyValue in _chunks)
            {
                keyValue.Value.Dispose();
            }

            _disposed = true;
        }

        public override int DrawPriority { get { return 20; } }

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
