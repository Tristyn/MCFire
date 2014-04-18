﻿using System;
using MCFire.Modules.Editor.Extensions;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace MCFire.Modules.Editor.Models
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
            Direction = Vector3.ForwardRH;
            Position = new Vector3(0, 0, -5);
            _graphicsDevice = graphicsDevice;
        }

        public void Update(KeyboardState keystate)
        {
            // move forward, back
            if (keystate.IsKeyDown(Keys.W))
                Position += Direction * .5f;
            // TODO: acceleration
            if (keystate.IsKeyDown(Keys.S))
                Position -= Direction * .5f;

            // move left, right
            if (keystate.IsKeyDown(Keys.D))
                Position = Vector3.Cross(Direction, Vector3.Up) * .5f + Position;

            if (keystate.IsKeyDown(Keys.A))
                Position = Vector3.Cross(Direction, Vector3.Down) * .5f + Position;

            // rotate yaw
            if (keystate.IsKeyDown(Keys.E))
                Pan(new Vector2(0.0174532925f, 0));

            if (keystate.IsKeyDown(Keys.Q))
                Pan(new Vector2(-0.0174532925f, 0));

            // rotate pitch
            if (keystate.IsKeyDown(Keys.R))
            {
                Pan(new Vector2(0, 0.0174532925f));
            }

            if (keystate.IsKeyDown(Keys.F))
            {
                Pan(new Vector2(0, -0.0174532925f));
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
            var yaw = Quaternion.RotationAxis(Vector3.Up, magnitude.X);
            var pitch = Quaternion.RotationAxis(Vector3.Cross(Direction, Vector3.Up), magnitude.Y);

            var pitchHack = Direction.Y + magnitude.Y;

            Quaternion rotation = pitchHack >= 1 || pitchHack <= -1 ? yaw : yaw * pitch;

            Direction = Vector3.TransformCoordinate(Direction, Matrix.RotationQuaternion(rotation)).ToNormal();
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
            get { return Matrix.LookAtRH(Position, Direction + Position, Vector3.Up); }
        }

        public Matrix ProjectionMatrix
        {
            get
            {
                return Matrix.PerspectiveFovRH(Fov,
                    (float)_graphicsDevice.BackBuffer.Width / _graphicsDevice.BackBuffer.Height, 0.1f, 1000.0f);
            }
        }
    }
}