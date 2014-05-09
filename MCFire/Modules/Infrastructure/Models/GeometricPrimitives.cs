using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Infrastructure.Models
{
    public static class GeometricPrimitives
    {
        public static readonly Vector3[] ForwardQuad =
        {
            new Vector3(1,0,0),
            new Vector3(1,1,0),
            new Vector3(0,0,0),
            new Vector3(0,0,0),
            new Vector3(1,1,0),
            new Vector3(0,1,0) 
        };

        public static readonly Vector3[] BackwardQuad =
        {
            new Vector3(0,0,1),
            new Vector3(0,1,1),
            new Vector3(1,0,1),
            new Vector3(1,0,1),
            new Vector3(0,1,1),
            new Vector3(1,1,1)
        };

        public static readonly Vector3[] RightQuad =
        {
            new Vector3(1,0,1),
            new Vector3(1,1,1),
            new Vector3(1,0,0),
            new Vector3(1,0,0),
            new Vector3(1,1,1),
            new Vector3(1,1,0)
        };

        public static readonly Vector3[] LeftQuad =
        {
            new Vector3(0,0,0),
            new Vector3(0,1,0),
            new Vector3(0,0,1),
            new Vector3(0,0,1),
            new Vector3(0,1,0),
            new Vector3(0,1,1) 
        };

        public static readonly Vector3[] DownQuad =
        {
            new Vector3(0,0,1), 
            new Vector3(1,0,1),
            new Vector3(0,0,0),
            new Vector3(0,0,0), 
            new Vector3(1,0,1),
            new Vector3(1,0,0)
        };

        // notch messed up! it isnt a rotation of z+ like the top is. Its a mirror of z+ on the edge they share. It must have been like this forever.
        public static readonly Vector3[] UpQuad =
        {
            new Vector3(0,1,1),
            new Vector3(0,1,0),
            new Vector3(1,1,1),
            new Vector3(1,1,1),
            new Vector3(0,1,0),
            new Vector3(1,1,0)
        };

        public static readonly Vector2[] QuadUv =
        {
            new Vector2(0,1),
            new Vector2(0,0),
            new Vector2(1,1),
            new Vector2(1,1),
            new Vector2(0,0),
            new Vector2(1,0)
        };
    }
}
