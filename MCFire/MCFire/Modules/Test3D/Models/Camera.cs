using SharpDX;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace MCFire.Modules.Test3D.Models
{
    public class Camera
    {
        readonly GraphicsDevice _graphicsDevice;

        public Camera(GraphicsDevice graphicsDevice)
        {
            Fov = MathUtil.Pi / 3;
            Direction = Vector3.ForwardLH;
            Position = new Vector3(0, 0, -5);
            _graphicsDevice = graphicsDevice;
        }

        public void Update(KeyboardState keystate)
        {
            // move forward, back
            if (keystate.IsKeyDown(Keys.W))
                Position += Direction * .1f;

            if (keystate.IsKeyDown(Keys.S))
                Position -= Direction * .1f;

            // move left, right
            if (keystate.IsKeyDown(Keys.D))
                Position = Vector3.Cross(Direction, Vector3.Down) * .1f + Position;

            if (keystate.IsKeyDown(Keys.A))
                Position = Vector3.Cross(Direction, Vector3.Up) * .1f + Position;

            // rotate yaw
            if (keystate.IsKeyDown(Keys.E))
                Direction = Vector3.TransformCoordinate(Direction, Matrix.RotationAxis(Vector3.Up, MathUtil.DegreesToRadians(1)));

            if (keystate.IsKeyDown(Keys.Q))
                Direction = Vector3.TransformCoordinate(Direction, Matrix.RotationAxis(Vector3.Up, MathUtil.DegreesToRadians(-1)));

            // rotate pitch
            if (keystate.IsKeyDown(Keys.R))
            {
                var left = Vector3.Cross(Direction, Vector3.Up);
                var pitchMatrix = Matrix.RotationAxis(left, MathUtil.DegreesToRadians(1));
                Direction = Vector3.TransformCoordinate(Direction, pitchMatrix);
            }

            if (keystate.IsKeyDown(Keys.F))
            {
                var left = Vector3.Cross(Direction, Vector3.Up);
                var pitchmatrix = Matrix.RotationAxis(left, MathUtil.DegreesToRadians(-1));
                Direction = Vector3.TransformCoordinate(Direction, pitchmatrix);
            }
        }

        /// <summary>
        /// Sets the camera to look at the target from its current position.
        /// </summary>
        /// <param name="target"></param>
        public void LookAt(Vector3 target)
        {
            Direction = Vector3.Normalize(target - Position);
        }

        public Vector3 Position { get; set; }

        public Vector3 Direction { get; set; }

        public float Fov { get; set; }

        public Matrix ViewMatrix
        {
            get { return Matrix.LookAtLH(Position, Direction + Position, Vector3.Up); }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                return Matrix.PerspectiveFovLH(Fov,
                    (float)_graphicsDevice.BackBuffer.Width / _graphicsDevice.BackBuffer.Height, 0.1f, 1000.0f);
            }
        }
    }
}
