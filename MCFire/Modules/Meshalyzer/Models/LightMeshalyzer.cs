﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Documents;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.Infrastructure.Models;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Substrate;
using Substrate.Core;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;
using Vector3 = SharpDX.Vector3;

namespace MCFire.Modules.Meshalyzer.Models
{
    [Export(typeof(IMeshalyzer))]
    public class LightMeshalyzer : MeshalyzerBase
    {
        readonly ChunkPrioritizer _prioritizer = new ChunkPrioritizer();
        VertexLitEffect _vertexLit;
        // TODO: methods become protected and extensible to make inheriting a meshalyzer easier
        public override void LoadContent(EditorGame game)
        {
            _vertexLit = new VertexLitEffect(game.LoadContent<Effect>(@"VertexLit"));
        }

        public override void UnloadContent(EditorGame game)
        {
            _vertexLit.Dispose();
        }

        /// <summary>
        /// Meshalyzes one chunk. Closer chunks are prioritized.
        /// </summary>
        /// <returns>If there are still chunks to be meshalyzed. Sleeping is a good strategy when true is returned.</returns>
        public override IChunkMesh MeshalyzeNext(EditorGame game)
        {
            ChunkPosition chunkPoint;
            if (!_prioritizer.GetNextDesiredChunk(game.Camera.ChunkPosition, out chunkPoint))
                return null;

            Buffer<VertexPositionColor> buffer = null;
            game.World.GetChunk(new ChunkPositionDimension(chunkPoint, game.Dimension), AccessMode.Read, chunk =>
            {
                if (chunk == null)
                    return;
                buffer = GenerateMainMesh(chunk, game);
            });

            return new VisualChunk(chunkPoint, _vertexLit, buffer);
        }

        [CanBeNull]
        Buffer<VertexPositionColor> GenerateMainMesh([NotNull] IChunk chunk, EditorGame game)
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
                                AddTriangleQuad(new Vector3(x, y, z), Right, chunkVerticesList, GetVertexColor(chunk, new BlockPosition(x, y, z), new BlockPosition(x + 1, y, z)));
                            }
                        }

                        if (y + 1 < chunkBlocks.YDim)
                        {
                            var yPlusBlock = chunkBlocks.GetBlock(x, y + 1, z);
                            if (yPlusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Up, chunkVerticesList, GetVertexColor(chunk, new BlockPosition(x, y, z), new BlockPosition(x, y + 1, z)));
                            }
                        }

                        if (z + 1 < chunkBlocks.ZDim)
                        {
                            var zPlubBlock = chunkBlocks.GetBlock(x, y, z + 1);
                            if (zPlubBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Backward, chunkVerticesList, GetVertexColor(chunk, new BlockPosition(x, y, z), new BlockPosition(x, y, z + 1)));
                            }
                        }

                        if (x - 1 >= 0)
                        {
                            var xMinusBlock = chunkBlocks.GetBlock(x - 1, y, z);
                            if (xMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Left, chunkVerticesList, GetVertexColor(chunk, new BlockPosition(x, y, z), new BlockPosition(x - 1, y, z)));
                            }
                        }

                        if (y - 1 >= 0)
                        {
                            var yMinusBlock = chunkBlocks.GetBlock(x, y - 1, z);
                            if (yMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Down, chunkVerticesList, GetVertexColor(chunk, new BlockPosition(x, y, z), new BlockPosition(x, y - 1, z)));
                            }
                        }

                        if (z - 1 >= 0)
                        {
                            var zMinusBlock = chunkBlocks.GetBlock(x, y, z - 1);
                            if (zMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Forward, chunkVerticesList, GetVertexColor(chunk, new BlockPosition(x, y, z), new BlockPosition(x, y, z - 1)));
                            }
                        }
                    }

            return chunkVerticesList.Count == 0 ? null : Buffer.Vertex.New(game.GraphicsDevice, chunkVerticesList.ToArray());
        }

        protected virtual Color GetVertexColor(IChunk chunk, BlockPosition blockPosition, BlockPosition airPosition)
        {
            var bl = chunk.Blocks.GetBlockLight(airPosition);
            var sl = chunk.Blocks.GetSkyLight(airPosition);
            // range 32-255
            var lum = (byte)(Math.Max(bl, sl) * 14 + 32);
            return new Color(lum, lum, lum, 255);
        }

        static void AddTriangleQuad(Vector3 location, Matrix direction, ICollection<VertexPositionColor> triangleMesh, Color color)
        {
            // 0-15 to 0-255

            foreach (var vertex in QuadVertices)
            {
                // rotate the vector to face the direction specified
                var transformed = Vector3.TransformCoordinate(vertex, direction);

                // translate the vector into substrateWorld space
                transformed += location;

                // add the vector to the list as a vertex
                triangleMesh.Add(new VertexPositionColor(transformed, color));
                //triangleMesh.Add(new VertexPositionColor(transformed, new Color(luminance)));
            }
        }

        protected static readonly Matrix Up = Matrix.Identity;
        protected static readonly Matrix Forward = Matrix.RotationX(-MathUtil.PiOverTwo) * Matrix.Translation(0, 0, 1);
        protected static readonly Matrix Down = Matrix.RotationX(MathUtil.Pi) * Matrix.Translation(0, 1, 1);
        protected static readonly Matrix Backward = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0);
        protected static readonly Matrix Right = Matrix.RotationZ(-MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0);
        protected static readonly Matrix Left = Matrix.RotationZ(MathUtil.PiOverTwo) * Matrix.Translation(1, 0, 0);

        protected static readonly Vector3[] QuadVertices =
        {
            new Vector3(1,1,1),
            new Vector3(0,1,1),
            new Vector3(0,1,0),
            new Vector3(1,1,0),
            new Vector3(1,1,1),
            new Vector3(0,1,0)
        };
    }

    public abstract class MeshalyzerBase : IMeshalyzer
    {
        public virtual void Dispose() { }
        public virtual void LoadContent(EditorGame game) { }
        public virtual void UnloadContent(EditorGame game) { }

        public abstract IChunkMesh MeshalyzeNext(EditorGame game);
    }
}