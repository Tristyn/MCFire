using SharpDX;

namespace MCFire.Modules.Test3D.Extensions
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
