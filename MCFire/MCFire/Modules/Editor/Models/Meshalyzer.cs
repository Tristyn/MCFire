using System;
using System.Collections.Generic;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Infrastructure.Extensions;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Substrate;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;
using Vector3 = SharpDX.Vector3;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// Service for creating meshes for the 3D editor.
    /// </summary>
    public sealed class Meshalyzer
    {
        readonly EditorGame _game;
        readonly MCFireWorld _world;
        readonly int _dimension;
        readonly VertexLitEffect _vertexLit;

        public Meshalyzer(EditorGame game, MCFireWorld world, int dimension)
        {
            _game = game;
            _world = world;
            _dimension = dimension;
            _vertexLit = new VertexLitEffect(_game.LoadContent<Effect>(@"VertexLit"));
        }

        public bool MeshalyzeNext()
        {
            Point chunkPoint;

            if (!_game.GetNextDesiredChunk(out chunkPoint))
                return false;
            var chunk = _world.GetChunk(_dimension, chunkPoint.X, chunkPoint.Y);
            if (chunk == null) 
                return false;

            var chunkBlocks = chunk.Blocks;
            var chunkVerticesList = new List<VertexPositionColor>(500);
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


            var chunkVisual = new VisualChunk(Buffer.Vertex.New(_game.GraphicsDevice, chunkVerticesList.ToArray()), _vertexLit, chunk.ChunkPosition());

            _game.AddChunk(chunk, chunkVisual);
            return true;
        }

        static void AddTriangleQuad(Vector3 location, Matrix direction, ICollection<VertexPositionColor> triangleMesh, byte luminance)
        {
            // 0-15 to 0-255
            luminance *= 16;

            foreach (var vertex in QuadVertices)
            {
                // rotate the vector to face the direction specified
                var transformed = Vector3.TransformCoordinate(vertex, direction);

                // translate the vector into world space
                transformed += location;

                // add the vector to the list as a vertex
                triangleMesh.Add(new VertexPositionColor(transformed, new Color(luminance, luminance, luminance, 255)));
                //triangleMesh.Add(new VertexPositionColor(transformed, new Color(luminance)));
            }
        }

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

        #endregion
    }
}
