using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Substrate;
using Vector3 = SharpDX.Vector3;

namespace MCFire.Modules.Editor.Meshalyzer
{
    public abstract class BlockMeshalyzer
    {
        protected static readonly AlphaBlock DefaultBlock = new AlphaBlock(0);
        protected ChunkRef Chunk;
        protected AlphaBlockCollection Blocks;
        protected AnvilBiomeCollection Biomes;
        protected EntityCollection Entities;

        public virtual void Meshalyze(int x, int y, int z, ICollection<VertexPositionColorTexture> buffer)
        {

        }

        protected AlphaBlock GetBlock(int x, int y, int z)
        {
            var containingChunk = Chunk;

            /* Theres an edge case here.
             * For example: when the chunk is populated, its northwest neighbor is populated, but its north neighbor isn't,
             * if a block is requested from the northwest neighbor, it will jump to the north and return DefaultBlock,
             * even though the chunk that the requested block is in exists.
             */
            // wrap to adjecent chunks if it overlaps the dimensions if this one.
            while (x >= Blocks.XDim)
            {
                containingChunk = containingChunk.GetSouthNeighbor();
                if (containingChunk == null) return DefaultBlock;
                x -= Blocks.XDim;
            }
            while (x < 0)
            {
                containingChunk = containingChunk.GetNorthNeighbor();
                if (containingChunk == null) return DefaultBlock;
                x += Blocks.XDim;
            }

            if (y >= Blocks.YDim)
            {
                return DefaultBlock; // sky limit
            }
            if (y < 0)
            {
                return DefaultBlock; // void limit
            }

            while (z >= Blocks.ZDim)
            {
                containingChunk = containingChunk.GetWestNeighbor();
                if (containingChunk == null) return DefaultBlock;
                z -= Blocks.ZDim;
            }
            while (z < 0)
            {
                containingChunk = containingChunk.GetEastNeighbor();
                if (containingChunk == null) return DefaultBlock;
                z += Blocks.ZDim;
            }

            return containingChunk.Blocks.GetBlock(x, y, z);
        }

        [NotNull]
        public virtual ChunkRef CurrentChunk
        {
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                Chunk = value;
                Blocks = value.Blocks;
                Biomes = value.Biomes;
                Entities = value.Entities;
            }
        }

        public virtual void Close()
        {
            Chunk = null;
            Blocks = null;
            Biomes = null;
            Entities = null;
        }

        #region Statics

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

        protected static readonly Vector2[] QuadTextureUVs =
        {
            new Vector2(1,1),
            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,0)
        };

        protected static Matrix[] Faces =
        {
            Matrix.Identity,
            Matrix.RotationX(-MathUtil.PiOverTwo) * Matrix.Translation(0, 0, 1),
            Matrix.RotationX(MathUtil.Pi) * Matrix.Translation(0, 1, 1),
            Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0),
            Matrix.RotationZ(-MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0),
            Matrix.RotationZ(MathUtil.PiOverTwo) * Matrix.Translation(1, 0, 0)
        };

        protected static Matrix[] SideFaces =
        {
            Matrix.RotationX(-MathUtil.PiOverTwo) * Matrix.Translation(0, 0, 1),
            Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0),
            Matrix.RotationZ(-MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0),
            Matrix.RotationZ(MathUtil.PiOverTwo) * Matrix.Translation(1, 0, 0)
        };

        #endregion
    }
}
