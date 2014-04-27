using System;
using SharpDX;
using Point = MCFire.Modules.Infrastructure.Models.Point;

namespace MCFire.Modules.Infrastructure.Extensions
{
    public static class Vector2Extensions
    {
        public static Point ToChunkSpace(this Vector2 position)
        {
            return new Point((int)Math.Floor(position.X / 16), (int)Math.Floor(position.Y / 16));
        }
    }
}
