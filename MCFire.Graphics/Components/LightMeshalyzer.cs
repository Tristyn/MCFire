using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using JetBrains.Annotations;
using MCFire.Common;
using MCFire.Common.Coordinates;
using MCFire.Common.Infrastructure.Extensions;
using MCFire.Common.MCFire.Modules.Infrastructure.Models;
using MCFire.Graphics.Editor;
using MCFire.Graphics.Infrastructure.Extensions;
using MCFire.Graphics.Primitives;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Substrate;
using Substrate.Core;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;
using Vector3 = SharpDX.Vector3;

namespace MCFire.Graphics.Components
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export(typeof(IMeshalyzer))]
    public class LightMeshalyzer : MeshalyzerBase
    {
        // TODO: methods become protected and extensible to make inheriting a meshalyzer easier
        // TODO: refactoring. IMeshalyzers shouldnt be exposed to IEditorGame. Perhaps expose an IEnumerable to let consumers place its data in the proper area
        public override void LoadContent([NotNull] IEditorGame game)
        {
        }

        public override void UnloadContent([NotNull] IEditorGame game)
        {
        }

        // TODO: let consumers of IChunkMesh set its effect, so consumers can let it be transparent.
        public override Buffer<VertexPositionColorTexture> Meshalyze(IEditorGame game, BlockSelection volume)
        {
            var chunkVerticesList = new List<VertexPositionColorTexture>(13000);

            volume.GetChunks(AccessMode.Read, portions =>
            {
                foreach (var partition in portions)
                {
                    var chunk = partition.Chunk;
                    var blocks = chunk.Blocks;

                    for (var y = partition.YMin; y < partition.YMax; y++)
                        for (var x = partition.XMin; x < partition.XMax; x++)
                            for (var z = partition.ZMin; z < partition.ZMax; z++)
                            {
                                var block = blocks.GetBlock(x, y, z);
                                if (block.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                                    continue;

                                var blockVector = new Vector3(x,y,z);
                                // block isn't air, assume its solid.
                                // at this point, texture coordinates should be calculated
                                // block specific shapes should happen (eg water)
                                if (x + 1 < blocks.XDim)
                                {
                                    // face with normal x+
                                    var xPlusBlock = blocks.GetBlock(x + 1, y, z);
                                    if (xPlusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                                    {
                                        AddTriangleQuad(blockVector, GeometricPrimitives.RightQuad, chunkVerticesList,
                                            GetVertexColor(chunk, new LocalBlockPosition(x, y, z),
                                                new LocalBlockPosition(x + 1, y, z)));
                                    }
                                }

                                if (y + 1 < blocks.YDim)
                                {
                                    var yPlusBlock = blocks.GetBlock(x, y + 1, z);
                                    if (yPlusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                                    {
                                        AddTriangleQuad(blockVector, GeometricPrimitives.TopQuad, chunkVerticesList,
                                            GetVertexColor(chunk, new LocalBlockPosition(x, y, z),
                                                new LocalBlockPosition(x, y + 1, z)));
                                    }
                                }

                                if (z + 1 < blocks.ZDim)
                                {
                                    var zPlusBlock = blocks.GetBlock(x, y, z + 1);
                                    if (zPlusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                                    {
                                        AddTriangleQuad(blockVector, GeometricPrimitives.BackwardQuad,
                                            chunkVerticesList,
                                            GetVertexColor(chunk, new LocalBlockPosition(x, y, z),
                                                new LocalBlockPosition(x, y, z + 1)));
                                    }
                                }

                                if (x - 1 >= 0)
                                {
                                    var xMinusBlock = blocks.GetBlock(x - 1, y, z);
                                    if (xMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                                    {
                                        AddTriangleQuad(blockVector, GeometricPrimitives.LeftQuad, chunkVerticesList,
                                            GetVertexColor(chunk, new LocalBlockPosition(x, y, z),
                                                new LocalBlockPosition(x - 1, y, z)));
                                    }
                                }

                                if (y - 1 >= 0)
                                {
                                    var yMinusBlock = blocks.GetBlock(x, y - 1, z);
                                    if (yMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                                    {
                                        AddTriangleQuad(blockVector, GeometricPrimitives.BottomQuad, chunkVerticesList,
                                            GetVertexColor(chunk, new LocalBlockPosition(x, y, z),
                                                new LocalBlockPosition(x, y - 1, z)));
                                    }
                                }

                                if (z - 1 >= 0)
                                {
                                    var zMinusBlock = blocks.GetBlock(x, y, z - 1);
                                    if (zMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                                    {
                                        AddTriangleQuad(blockVector, GeometricPrimitives.ForwardQuad, chunkVerticesList,
                                            GetVertexColor(chunk, new LocalBlockPosition(x, y, z),
                                                new LocalBlockPosition(x, y, z - 1)));
                                    }
                                }
                            }
                }
            });

            return chunkVerticesList.Count == 0
                ? null
                : Buffer.Vertex.New(game.GraphicsDevice, chunkVerticesList.ToArray());
        }

        protected virtual Color GetVertexColor([NotNull] IChunk chunk, LocalBlockPosition blockPosition, LocalBlockPosition airPosition)
        {
            var bl = chunk.Blocks.GetBlockLight(airPosition);
            var sl = chunk.Blocks.GetSkyLight(airPosition);
            // range 32-255
            var lum = (byte)(Math.Max(bl, sl) * 14 + 32);
            return new Color(lum, lum, lum, (byte)255);
        }

        static void AddTriangleQuad(Vector3 location, [NotNull] IEnumerable<Vector3> quad, [NotNull] ICollection<VertexPositionColorTexture> triangleMesh, Color color)
        {
            // 0-15 to 0-255
            foreach (var vertex in quad)
            {
                // translate the vector into chunk space
                var translated = vertex + location;

                // add the vector to the list as a vertex
                triangleMesh.Add(new VertexPositionColorTexture(translated, color, new Vector2()));
            }
        }
    }
}
