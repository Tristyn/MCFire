using System;
using MCFire.Modules.Test3D.Extensions;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace MCFire.Modules.Test3D.Models
{
    public class Camera : IDisposable
    {
        GraphicsDevice _graphicsDevice;
        Vector3 _position;
        Vector3 _direction;
        float _fov;

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
                Pan(new Vector2(0.0174532925f, 0));

            if (keystate.IsKeyDown(Keys.Q))
                Pan(new Vector2(-0.0174532925f, 0));

            // rotate pitch
            if (keystate.IsKeyDown(Keys.R))
            {
                Pan(new Vector2(0, -0.0174532925f));
            }

            if (keystate.IsKeyDown(Keys.F))
            {
                Pan(new Vector2(0, 0.0174532925f));
            }
        }

        /// <summary>
        /// Sets the camera to look at the target from its current position.
        /// </summary>
        /// <param name="target"></param>
        public void LookAt(Vector3 target)
        {
            Direction = (target - Position).ToNormal();
        }

        /// <summary>
        /// Rotates the camera from its current direction.
        /// </summary>
        /// <param name="magnitude">The angle, in radians, to pan the camera. +X is right, +Y is down.</param>
        public void Pan(Vector2 magnitude)
        {
            // TODO: clamp Y, because when you look to far down or up, X gets flipped.
            var yaw = Matrix.RotationY(magnitude.X);
            var pitch = Matrix.RotationX(magnitude.Y);
            var rotation = yaw * pitch;
            Direction = Vector3.TransformCoordinate(Direction, rotation).ToNormal();
        }

        public void Dispose()
        {
            _graphicsDevice = null;
        }

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector3 Direction
        {
            get { return _direction; }
            set
            {
                _direction = value.ToNormal();
            }
        }

        public float Fov
        {
            get { return _fov; }
            set { _fov = MathUtil.Clamp(value, 1, 179); }
        }

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
