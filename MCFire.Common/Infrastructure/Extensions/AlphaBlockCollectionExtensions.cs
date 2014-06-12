using MCFire.Common.Coordinates;
using Substrate;

namespace MCFire.Common.Infrastructure.Extensions
{
    public static class AlphaBlockCollectionExtensions
    {
        public static int GetBlockLight(this AlphaBlockCollection blocks, LocalBlockPosition position)
        {
            return blocks.GetBlockLight(position.X,position.Y,position.Z);
        }

        public static int GetData(this AlphaBlockCollection blocks, LocalBlockPosition position)
        {
            return blocks.GetData(position.X, position.Y, position.Z);
        }

        public static int GetID(this AlphaBlockCollection blocks, LocalBlockPosition position)
        {
            return blocks.GetID(position.X, position.Y, position.Z);
        }

        public static int GetSkyLight(this AlphaBlockCollection blocks, LocalBlockPosition position)
        {
            return blocks.GetSkyLight(position.X, position.Y, position.Z);
        }
        public static int GetTileTickValue(this AlphaBlockCollection blocks, LocalBlockPosition position)
        {
            return blocks.GetTileTickValue(position.X, position.Y, position.Z);
        }
        public static AlphaBlock GetBlock(this AlphaBlockCollection blocks, LocalBlockPosition position)
        {
            return blocks.GetBlock(position.X, position.Y, position.Z);
        }
        public static AlphaBlockRef GetBlockRef(this AlphaBlockCollection blocks, LocalBlockPosition position)
        {
            return blocks.GetBlockRef(position.X, position.Y, position.Z);
        }
        public static TileTick GetTileTick(this AlphaBlockCollection blocks, LocalBlockPosition position)
        {
            return blocks.GetTileTick(position.X, position.Y, position.Z);
        }
        public static BlockInfo GetInfo(this AlphaBlockCollection blocks, LocalBlockPosition position)
        {
            return blocks.GetInfo(position.X, position.Y, position.Z);
        }
        public static TileEntity GetTileEntity(this AlphaBlockCollection blocks, LocalBlockPosition position)
        {
            return blocks.GetTileEntity(position.X, position.Y, position.Z);
        }
    }
}
