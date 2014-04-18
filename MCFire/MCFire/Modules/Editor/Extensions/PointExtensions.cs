using SharpDX;
using Point = System.Windows.Point;

namespace MCFire.Modules.Editor.Extensions
{
    public static class PointExtensions
    {
        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2((float)point.X, (float)point.Y);
        }
    }
}
