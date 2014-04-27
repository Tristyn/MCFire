using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Infrastructure.Models
{
    public static class GeometricPrimitives
    {
        public static readonly Matrix UpMatrix = Matrix.Identity;
        public static readonly Matrix ForwardMatrix = Matrix.RotationX(-MathUtil.PiOverTwo) * Matrix.Translation(0, 0, 1);
        public static readonly Matrix DownMatrix = Matrix.RotationX(MathUtil.Pi) * Matrix.Translation(0, 1, 1);
        public static readonly Matrix BackwardMatrix = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0);
        public static readonly Matrix RightMatrix = Matrix.RotationZ(-MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0);
        public static readonly Matrix LeftMatrix = Matrix.RotationZ(MathUtil.PiOverTwo) * Matrix.Translation(1, 0, 0);

        public static readonly Vector3[] QuadVertex =
        {
            new Vector3(1,1,1),
            new Vector3(0,1,1),
            new Vector3(0,1,0),
            new Vector3(1,1,0),
            new Vector3(1,1,1),
            new Vector3(0,1,0)
        };

        public static readonly VertexPositionTexture[] QuadVertexPositionTexture =
        {
            new VertexPositionTexture(new Vector3(1.0f, 1.0f, 1.0f),new Vector2(0,0)),
            new VertexPositionTexture(new Vector3(-1.0f, 1.0f, 1.0f), new Vector2(1,0)),
            new VertexPositionTexture(new Vector3(-1.0f, 1.0f, -1.0f), new Vector2(1,1)),
            new VertexPositionTexture(new Vector3(1.0f, 1.0f, -1.0f),  new Vector2(0,1)),
            new VertexPositionTexture(new Vector3(1.0f, 1.0f, 1.0f), new Vector2(0,0)),
            new VertexPositionTexture(new Vector3(-1.0f, 1.0f, -1.0f), new Vector2(1,1)),
        };
    }
}
