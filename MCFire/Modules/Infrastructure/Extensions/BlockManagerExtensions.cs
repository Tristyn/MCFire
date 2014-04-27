using MCFire.Modules.Infrastructure.Models;
using Substrate;
using Substrate.Core;

namespace MCFire.Modules.Infrastructure.Extensions
{
   public static class BlockManagerExtensions
   {
       public static AlphaBlock GetBlock(this IBlockManager blockManager, Point3 position)
       {
           return blockManager.GetBlock(position.X, position.Y, position.Z);
       }
   }
}
