using SharpDX;

namespace MCFire.Modules.Editor.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 ToNormal(this Vector3 vector3)
        {
            vector3.Normalize();
            return vector3;
        }
    }
}
