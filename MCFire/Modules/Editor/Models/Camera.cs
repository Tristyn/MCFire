﻿using System;
using System.Windows.Input;
using MCFire.Modules.Editor.Extensions;
using MCFire.Modules.Infrastructure.Extensions;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Matrix = SharpDX.Matrix;
using Point = MCFire.Modules.Infrastructure.Models.Point;

namespace MCFire.Modules.Editor.Models
{
    public class Camera : IDisposable
    {
        #region Fields

        // refs
        EditorGame _game;
        GraphicsDevice _graphicsDevice;
        SharpDXElement _sharpDx;
        Mouse _mouse;

        // transform
        Vector3 _position = new Vector3(0, 0, -5);
        Vector3 _direction = Vector3.ForwardRH;
        Vector3 _velocity;
        float _fov = MathUtil.Pi / 3;
        float _acceleration = 4f;
        float _slowDown = .9f;

        // idle rotate args
        Vector3 _idleRotateCenter;
        float _idleRotateHorizontalRadius;
        float _idleRotateSpeed;
        float _idleRotateTime;

        // state
        bool _disposed;
        bool _idleRotate;

        //
        float _nearZClip = .1f;
        float _farZClip = 1000f;
        float _idleRotatePitch;

        #endregion

        #region Constructor

        public Camera(EditorGame game)
        {
            _game = game;
            _graphicsDevice = _game.GraphicsDevice;
            _sharpDx = _game.SharpDxElement;
            _mouse = _game.Mouse;

            _mouse.Right.DragStart += PerspectiveDragStart;
            _mouse.Right.DragMove += PerspectiveDragMove;
            _mouse.Right.DragEnd += PerspectiveDragEnd;
        }

        #endregion

        #region Methods

        public void Update(GameTime time)
        {
            if (_disposed)
                throw new ObjectDisposedException("Camera");
            var deltaTime = (float)time.ElapsedGameTime.TotalSeconds;

            PositionKeyboard(deltaTime);
            PerspectiveKeyboard(deltaTime);

            if(_velocity!=Vector3.Zero)
                _idleRotate = false;

            _velocity *= _slowDown;
            _position += _velocity;

            if (_idleRotate)
                IdleRotate(time);
        }

        #region Position

        void PositionKeyboard(float deltaTime)
        {
            var keystate = _game.Keyboard.State;

            // move forward, back
            if (keystate.IsKeyDown(Keys.W))
            {
                _velocity += Direction*_acceleration*deltaTime;
            }

            if (keystate.IsKeyDown(Keys.S))
            {
                _velocity -= Direction * _acceleration * deltaTime;
            }

            // move left, right
            if (keystate.IsKeyDown(Keys.D))
            {
                var strafeDir = Vector3.Cross(Direction, Vector3.Up);
                strafeDir.Normalize();
                _velocity += strafeDir * _acceleration * deltaTime;
            }

            if (keystate.IsKeyDown(Keys.A))
            {
                var strafeDir = Vector3.Cross(Direction, Vector3.Down);
                strafeDir.Normalize();
                _velocity += strafeDir * _acceleration * deltaTime;
            }
        }

        void IdleRotate(GameTime time)
        {
            // update time
            _idleRotateTime += (float)time.ElapsedGameTime.TotalSeconds * _idleRotateSpeed;

            // calculate the negative direction based on time
            var dir = new Vector3(0,-_idleRotatePitch, 1);
            dir.Normalize();
            var rot = Quaternion.RotationYawPitchRoll(_idleRotateTime, 0, 0);
            var direction = Vector3.Transform(dir, rot);

            // position
            var radiusOffset = direction * _idleRotateHorizontalRadius;
            Position = _idleRotateCenter + radiusOffset;

            // rotation
            Direction = -direction;
        }

        #endregion

        #region Perspective

        #region Perspective Mouse Drag

        void PerspectiveDragEnd(object s, KeyEventArgs e)
        {
            _sharpDx.ReleaseMouseCapture();
            System.Windows.Input.Mouse.OverrideCursor = Cursors.Arrow;
        }

        void PerspectiveDragMove(object s, KeyEventArgs e)
        {
            // perspective drag
            var change = (e.PrevPosition - e.Position) * new Vector2(_game.GraphicsDevice.AspectRatio(), 1);
            Pan(change * 2);

            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (e.Position.X == 1)
            {
                _mouse.MoveSilently(new Vector2(0.01f, e.Position.Y));
            }
            else if (e.Position.X == 0)
            {
                _mouse.MoveSilently(new Vector2(0.99f, e.Position.Y));
            }
            if (e.Position.Y == 1)
            {
                _mouse.MoveSilently(new Vector2(e.Position.X, 0.01f));
            }
            else if (e.Position.Y == 0)
            {
                _mouse.MoveSilently(new Vector2(e.Position.X, 0.99f));
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        void PerspectiveDragStart(object s, KeyEventArgs e)
        {
            _sharpDx.CaptureMouse();
            System.Windows.Input.Mouse.OverrideCursor = Cursors.None;
        }

        #endregion

        #region Perspective Keyboard

        public void PerspectiveKeyboard(float deltaTime)
        {
            var keystate = _game.Keyboard.State;

            // rotate yaw
            if (keystate.IsKeyDown(Keys.E))
                Pan(new Vector2(deltaTime, 0));

            if (keystate.IsKeyDown(Keys.Q))
                Pan(new Vector2(-deltaTime, 0));

            // rotate pitch
            if (keystate.IsKeyDown(Keys.R))
            {
                Pan(new Vector2(0, deltaTime));
            }

            if (keystate.IsKeyDown(Keys.F))
            {
                Pan(new Vector2(0, -deltaTime));
            }
        }

        #endregion

        #endregion

        #region Publics

        /// <summary>
        /// Sets the camera to look at the target from its current position.
        /// </summary>
        /// <param name="target"></param>
        public void LookAt(Vector3 target)
        {
            if (_disposed)
                throw new ObjectDisposedException("Camera");

            Direction = (target - Position).ToNormal();
        }

        /// <summary>
        /// Rotates the camera from its current direction.
        /// </summary>
        /// <param name="magnitude">The angle, in radians, to pan the camera. +X is right, +Y is down.</param>
        public void Pan(Vector2 magnitude)
        {
            if (_disposed)
                throw new ObjectDisposedException("Camera");

            var yaw = Quaternion.RotationAxis(Vector3.Up, magnitude.X);
            var pitch = Quaternion.RotationAxis(Vector3.Cross(Direction, Vector3.Up), magnitude.Y);

            // clamps total pitch, so that it doesnt go above 1 or below -1
            var pitchHack = Direction.Y + magnitude.Y;
            var rotation = pitchHack >= 1 || pitchHack <= -1 ? yaw : yaw * pitch;

            Direction = Vector3.TransformCoordinate(Direction, Matrix.RotationQuaternion(rotation)).ToNormal();

            _idleRotate = false;
        }

        /// <summary>
        /// Creates a ray trace from screen coordinate.
        /// </summary>
        /// <param name="screenCoords">Normalized (0 to 1) screen coordinates</param>
        /// <returns>A tracer that can enumerate through blocks, entities, ect.</returns>
        public VoxelTracer RayTraceScreenPoint(Vector2 screenCoords)
        {
            if (_disposed)
                throw new ObjectDisposedException("Camera");

            // expand coord to fit the view
            screenCoords *= new Vector2(_graphicsDevice.BackBuffer.Width, _graphicsDevice.BackBuffer.Height);

            // translate vectors into world space by unprojecting them.
            var customViewport = new ViewportF(0, 0, _graphicsDevice.BackBuffer.Width, _graphicsDevice.BackBuffer.Height, _nearZClip, _farZClip);
            var near = customViewport.Unproject(new Vector3(screenCoords, _nearZClip), ProjectionMatrix, ViewMatrix, Matrix.Identity);
            var far = customViewport.Unproject(new Vector3(screenCoords, _farZClip), ProjectionMatrix, ViewMatrix, Matrix.Identity);

            var traceDirection = (far - near);
            traceDirection.Normalize();

            return new VoxelTracer(Position, traceDirection);
        }

        public void IdleRotate(Vector3 center, float horizontalRadius, float rotSpeed, float pitch)
        {
            _idleRotateCenter = center;
            _idleRotateHorizontalRadius = horizontalRadius;
            _idleRotateSpeed = rotSpeed;
            _idleRotatePitch = pitch;
            _idleRotateTime = 0;
            _idleRotate = true;
        }

        #endregion

        public void Dispose()
        {
            _game = null;
            _graphicsDevice = null;
            _sharpDx = null;
            _mouse = null;

            _disposed = true;
        }

        #endregion

        #region Properties

        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Point ChunkPosition
        {
            get { return new Point((int)(_position.X / 16), (int)(_position.Z / 16)); }
        }

        public Vector3 Direction
        {
            get { return _direction; }
            set { _direction = value.ToNormal(); }
        }

        public float Fov
        {
            get { return _fov; }
            set { _fov = MathUtil.Clamp(value, float.Epsilon, MathUtil.PiOverTwo - float.Epsilon); }
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
                    (float)_graphicsDevice.BackBuffer.Width / _graphicsDevice.BackBuffer.Height, _nearZClip, _farZClip);
            }
        }

        #endregion
    }
}