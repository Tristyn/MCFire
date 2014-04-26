using System;
using SharpDX;
using Point = MCFire.Modules.Infrastructure.Models.Point;

namespace MCFire.Modules.Infrastructure.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 ToNormal(this Vector3 vector3)
        {
            vector3.Normalize();
            return vector3;
        }

        public static Point ToChunkSpace(this Vector2 position)
        {
            return new Point((int)Math.Floor(position.X), (int)Math.Floor(position.Y));
        }
    }
}
