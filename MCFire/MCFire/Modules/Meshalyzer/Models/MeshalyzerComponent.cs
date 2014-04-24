﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Explorer.Models;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Substrate;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;
using Vector3 = SharpDX.Vector3;

namespace MCFire.Modules.Meshalyzer.Models
{
    [Export(typeof(IGameComponent))]
    public class MeshalyzerComponent : GameComponentBase
    {
        Thread _meshingThread;
        ChunkCreationPolicy _policy;
        bool _disposed;

        ConcurrentQueue<VisualChunk> _chunksToIntegrate = new ConcurrentQueue<VisualChunk>();
        Dictionary<Point, VisualChunk> _chunks = new Dictionary<Point, VisualChunk>();

        MCFireWorld _world;
        NbtWorld _substrateWorld;
        int _dimension;
        VertexLitEffect _vertexLit;
        GraphicsDevice _graphicsDevice;

        #region Statics

        static readonly Matrix Up = Matrix.Identity;
        static readonly Matrix Forward = Matrix.RotationX(-MathUtil.PiOverTwo) * Matrix.Translation(0, 0, 1);
        static readonly Matrix Down = Matrix.RotationX(MathUtil.Pi) * Matrix.Translation(0, 1, 1);
        static readonly Matrix Backward = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0);
        static readonly Matrix Right = Matrix.RotationZ(-MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0);
        static readonly Matrix Left = Matrix.RotationZ(MathUtil.PiOverTwo) * Matrix.Translation(1, 0, 0);

        static readonly Vector3[] QuadVertices =
        {
            new Vector3(1,1,1),
            new Vector3(0,1,1),
            new Vector3(0,1,0),
            new Vector3(1,1,0),
            new Vector3(1,1,1),
            new Vector3(0,1,0)
        };

        private Shiftable2DArray<Point> _chunkPoints;

        #endregion

        // TODO: use new meshalying system (BlockMeshalyzer)

        public MeshalyzerComponent()
        {
            ViewDistance = 10;
        }

        public override void LoadContent()
        {
            _world = Game.World;
            _substrateWorld = Game.SubstrateWorld;
            _dimension = Game.Dimension;
            _graphicsDevice = Game.GraphicsDevice;
            _vertexLit = new VertexLitEffect(Game.LoadContent<Effect>(@"VertexLit"));
            BeginBuildChunks();
        }

        public override void Draw(GameTime time)
        {
            if (_disposed)
                throw new ObjectDisposedException("MeshalyzerComponent");

            IntegrateNewChunks();

            foreach (var keyValue in _chunks)
            {
                keyValue.Value.Draw(Game);
            }
        }

        void IntegrateNewChunks()
        {
            while (true)
            {
                VisualChunk chunk;
                if(!_chunksToIntegrate.TryDequeue(out chunk)) break;

                // Add it to the dictionary using its position as a key.
                _chunks[chunk.ChunkPosition] = chunk;
            }
        }

        /// <summary>
        /// Creates chunks on a background thread until policy changes.
        /// </summary>
        void BeginBuildChunks()
        {
            // ReSharper disable once ObjectCreationAsStatement
            if (_meshingThread != null && _meshingThread.IsAlive)
                return;
            /* TODO: leverage multiple cores. some considerations: 
             * a chunk will be generated multiple times unless a thread 
             * can claim that point and it wont be picked again. 
             * mabey have a more sophisticated chunk selection system.
             */
            _meshingThread = new Thread(() =>
            {
                while (_policy == ChunkCreationPolicy.Run)
                {
                    if (!MeshalyzeNext())
                        Thread.Sleep(5000);
                }
            });
            Thread.CurrentThread.IsBackground = true;
            _meshingThread.Start();
        }

        /// <summary>
        /// Meshalyzes one chunk. Closer chunks are prioritized.
        /// </summary>
        /// <returns>If there are still chunks to be meshalyzed. Sleeping is a good strategy when true is returned.</returns>
        bool MeshalyzeNext()
        {
            Point chunkPoint;

            // TODO: replace game call with new algorithm thats faster because dictionaries!!!!!!
            if (!Game.GetNextDesiredChunk(out chunkPoint))
                return false;
            var chunk = _world.GetChunk(_dimension, chunkPoint.X, chunkPoint.Y);
            Buffer<VertexPositionColor> buffer = null;

            if (chunk != null)
                buffer = GenerateMainMesh(chunk);

            var chunkVisual = new VisualChunk(PopulationState.Populated, chunkPoint, _vertexLit, chunk, buffer);

            _chunksToIntegrate.Enqueue(chunkVisual);
            return true;
        }

        [CanBeNull]
        Buffer<VertexPositionColor> GenerateMainMesh(ChunkRef chunk)
        {
            var chunkBlocks = chunk.Blocks;
            var chunkVerticesList = new List<VertexPositionColor>(500);
            // TODO: speed up meshing by reading chunks sections that exist. some 16x16x16 chunks arent saved.
            for (var y = 0; y < chunkBlocks.YDim; y++)
                for (var x = 0; x < chunkBlocks.XDim; x++)
                    for (var z = 0; z < chunkBlocks.ZDim; z++)
                    {
                        var block = chunkBlocks.GetBlock(x, y, z);

                        if (block.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            continue;
                        // block isn't air, assume its solid.
                        // at this point, texture coordinates should be calculated
                        // block specific shapes should happen (eg water)

                        if (x + 1 < chunkBlocks.XDim)
                        {
                            // face with normal x+
                            var xPlusBlock = chunkBlocks.GetBlock(x + 1, y, z);
                            if (xPlusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Right, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x + 1, y, z), chunkBlocks.GetSkyLight(x + 1, y, z)));
                            }
                        }

                        if (y + 1 < chunkBlocks.YDim)
                        {
                            var yPlusBlock = chunkBlocks.GetBlock(x, y + 1, z);
                            if (yPlusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Up, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x, y + 1, z), chunkBlocks.GetSkyLight(x, y + 1, z)));
                            }
                        }

                        if (z + 1 < chunkBlocks.ZDim)
                        {
                            var zPlubBlock = chunkBlocks.GetBlock(x, y, z + 1);
                            if (zPlubBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Backward, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x, y, z + 1), chunkBlocks.GetSkyLight(x, y, z + 1)));
                            }
                        }

                        if (x - 1 >= 0)
                        {
                            var xMinusBlock = chunkBlocks.GetBlock(x - 1, y, z);
                            if (xMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Left, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x - 1, y, z), chunkBlocks.GetSkyLight(x - 1, y, z)));
                            }
                        }

                        if (y - 1 >= 0)
                        {
                            var yMinusBlock = chunkBlocks.GetBlock(x, y - 1, z);
                            if (yMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Down, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x, y - 1, z), chunkBlocks.GetSkyLight(x, y - 1, z)));
                            }
                        }

                        if (z - 1 >= 0)
                        {
                            var zMinusBlock = chunkBlocks.GetBlock(x, y, z - 1);
                            if (zMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Forward, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x, y, z - 1), chunkBlocks.GetSkyLight(x, y, z - 1)));
                            }
                        }
                    }

            return chunkVerticesList.Count == 0 ? null : Buffer.Vertex.New(_graphicsDevice, chunkVerticesList.ToArray());
        }

        static void AddTriangleQuad(Vector3 location, Matrix direction, ICollection<VertexPositionColor> triangleMesh, byte luminance)
        {
            // 0-15 to 0-255
            luminance *= 16;

            foreach (var vertex in QuadVertices)
            {
                // rotate the vector to face the direction specified
                var transformed = Vector3.TransformCoordinate(vertex, direction);

                // translate the vector into substrateWorld space
                transformed += location;

                // add the vector to the list as a vertex
                triangleMesh.Add(new VertexPositionColor(transformed, new Color(luminance, luminance, luminance, 255)));
                //triangleMesh.Add(new VertexPositionColor(transformed, new Color(luminance)));
            }
        }

        void GenerateChunkPoints()
        {
            var points = new List<Point>(ViewDistance * ViewDistance * 4);
            //create points
            for (var i = -ViewDistance; i < ViewDistance; i++)
            {
                for (var j = -ViewDistance; j < ViewDistance; j++)
                {
                    points.Add(new Point(i, j));
                }
            }

            var chunkPoints = (from point in points
                           // order by hypotenuse distance to (0,0)
                           orderby point.X * point.X + point.Y * point.Y
                           select point).ToArray();
            // TODO:
            //_chunkPoints = new Shiftable2DArray<Point>(chunkPoints);
        }

        public override void Dispose()
        {
            _policy= ChunkCreationPolicy.Idle;
            _meshingThread.Join();
            _meshingThread = null;

            if(_disposed)
                return;

            VisualChunk chunk;
            while (_chunksToIntegrate.TryDequeue(out chunk))
            {
                chunk.Dispose();
            }
            _chunksToIntegrate = null;

            foreach (var keyValue in _chunks)
            {
                keyValue.Value.Dispose();
            }
            _chunks = null;

            _world = null;
            _substrateWorld = null;
            _vertexLit.Dispose();
            if(_vertexLit==null)
            _graphicsDevice = null;

            _disposed = true;
        }

        public int ViewDistance { get; set; }
    }
}
