using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Substrate;
using Substrate.Core;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;
using Vector3 = SharpDX.Vector3;

namespace MCFire.Modules.Test3D.Models
{
    public class ChunkVisual
    {
        readonly GraphicsDevice _device;
        readonly List<Mesh> _blockMeshes = new List<Mesh>();
        public readonly int Cx;
        public readonly int Cy;

        public ChunkVisual(GraphicsDevice device, IChunkManager chunkManager, int cx, int cy)
        {
            _device = device;
            Cx = cx;
            Cy = cy;
            Construct(chunkManager);
        }

        public void Construct([NotNull] IChunkManager chunkManager)
        {
            if (chunkManager == null) throw new ArgumentNullException("chunkManager");

            var chunk = chunkManager.GetChunk(Cx, Cy);
            if (chunk == null) return;

            var blocks = chunk.Blocks;

            // ======= COPIED =======

            var chunkVerticesList = new List<VertexPositionColor>(500);
            for (var y = 0; y < blocks.YDim; y++)
                for (var x = 0; x < blocks.XDim; x++)
                    for (var z = 0; z < blocks.ZDim; z++)
                    {
                        var block = blocks.GetBlock(x, y, z);

                        if (block.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            continue;
                        // block isn't air, assume its solid.
                        // at this point, texture coordinates should be calculated
                        // block specific shapes should happen (eg water)

                        if (x + 1 < blocks.XDim)
                        {
                            // face with normal x+
                            var xPlusBlock = blocks.GetBlock(x + 1, y, z);
                            if (xPlusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Right, chunkVerticesList, (byte)Math.Max(blocks.GetBlockLight(x + 1, y, z), blocks.GetSkyLight(x + 1, y, z)));
                            }
                        }

                        if (y + 1 < blocks.YDim)
                        {
                            var yPlusBlock = blocks.GetBlock(x, y + 1, z);
                            if (yPlusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Up, chunkVerticesList, (byte)Math.Max(blocks.GetBlockLight(x, y + 1, z), blocks.GetSkyLight(x, y + 1, z)));
                            }
                        }

                        if (z + 1 < blocks.ZDim)
                        {
                            var zPlubBlock = blocks.GetBlock(x, y, z + 1);
                            if (zPlubBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Backward, chunkVerticesList, (byte)Math.Max(blocks.GetBlockLight(x, y, z + 1), blocks.GetSkyLight(x, y, z + 1)));
                            }
                        }

                        if (x - 1 >= 0)
                        {
                            var xMinusBlock = blocks.GetBlock(x - 1, y, z);
                            if (xMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Left, chunkVerticesList, (byte)Math.Max(blocks.GetBlockLight(x - 1, y, z), blocks.GetSkyLight(x - 1, y, z)));
                            }
                        }

                        if (y - 1 >= 0)
                        {
                            var yMinusBlock = blocks.GetBlock(x, y - 1, z);
                            if (yMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Down, chunkVerticesList, (byte)Math.Max(blocks.GetBlockLight(x, y - 1, z), blocks.GetSkyLight(x, y - 1, z)));
                            }
                        }

                        if (z - 1 >= 0)
                        {
                            var zMinusBlock = blocks.GetBlock(x, y, z - 1);
                            if (zMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Forward, chunkVerticesList, (byte)Math.Max(blocks.GetBlockLight(x, y, z - 1), blocks.GetSkyLight(x, y, z - 1)));
                            }
                        }
                    }

            var buffer = Buffer.Vertex.New(_device, chunkVerticesList.ToArray());
            var mesh = new Mesh
            {
                VertexBuffer = buffer,
                VertexInputLayout = VertexInputLayout.FromBuffer(0, buffer)
            };

            _blockMeshes.Add(mesh);
        }

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

        public void Initialize(Effect effect)
        {
            // TODO: each mesh should needs its own effect
            foreach (var mesh in _blockMeshes)
            {
                mesh.Effect = effect;
            }
        }

        public void Draw(GraphicsDevice context, Camera camera)
        {
            foreach (var mesh in _blockMeshes)
            {
                mesh.Draw(context);
            }
        }

        //bool IsBackFaceCulled(Camera camera, Vector3 _position, Vector3 normal)
        //{
        //    
        //}

        //IEnumerable<Mesh> Meshes
        //{
        //    get
        //    {
        //        foreach (var mesh in BlockMeshes)
        //        {
        //            yield return mesh;
        //        }
        //    }
        //}
    }
}
