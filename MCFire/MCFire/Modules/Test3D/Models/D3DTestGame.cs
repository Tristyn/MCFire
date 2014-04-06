using System;
using System.Diagnostics;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace MCFire.Modules.Test3D.Models
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    using SharpDX.Toolkit.Graphics;

    /// <summary>
    /// Simple MiniCube application using SharpDX.Toolkit.
    /// The purpose of this application is to show a rotating cube using <see cref="BasicEffect"/>.
    /// </summary>
    public class D3DTestGame : Game
    {
        // rendering
        GraphicsDeviceManager _graphicsDeviceManager;
        SpriteBatch _spriteBatch;
        BasicEffect _basicEffect;

        // content
        SpriteFont _font;
        VertexInputLayout _inputLayout;
        Buffer<VertexPositionColor> _vertices;

        // input
        MouseManager _mouse;
        KeyboardManager _keyboard;
        Vector3 _camera;
        Vector3 _cameraTarget;

        /// <summary>
        /// Initializes a new instance of the <see cref="D3DTestGame" /> class.
        /// </summary>
        public D3DTestGame()
        {
            // Creates a graphics manager. This is mandatory.
            _graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = @"Modules/Test3D/Content";
        }

        protected override void LoadContent()
        {
            // rendering
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _basicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
            });

            // input
            _mouse = new MouseManager(this);
            _mouse.Initialize();
            _keyboard = new KeyboardManager(this);
            _keyboard.Initialize();
            _camera = new Vector3(0, 0, -5);
            _cameraTarget = Vector3.ForwardLH;
            // content
            _font = Content.Load<SpriteFont>("Segoe12");
            _inputLayout = VertexInputLayout.FromBuffer(0, _vertices);
            _vertices = ToDisposeContent(Buffer.Vertex.New(
                GraphicsDevice,
                new[]
                    {
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange), // Front
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.Orange), // BACK
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.OrangeRed), // Top
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.OrangeRed), // Bottom
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.DarkOrange), // Left
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.DarkOrange), // Right
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.DarkOrange)
                    }));

            base.LoadContent();
        }

        protected override void Initialize()
        {
            Window.Title = "MiniCube demo";

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            // Rotate the cube.
            //var time = (float)gameTime.TotalGameTime.TotalSeconds;
            //_basicEffect.World = Matrix.RotationX(time) * Matrix.RotationY(time * 2.0f) * Matrix.RotationZ(time * .7f);
            //_basicEffect.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);

            var keystate = _keyboard.GetState();

            // move forward, back
            if (keystate.IsKeyDown(Keys.W))
                _camera += _cameraTarget * .1f;

            if (keystate.IsKeyDown(Keys.S))
                _camera -= _cameraTarget * .1f;

            // move left, right
            if (keystate.IsKeyDown(Keys.D))
                _camera = Vector3.Cross(_cameraTarget, Vector3.Down) * .1f + _camera;

            if (keystate.IsKeyDown(Keys.A))
                _camera = Vector3.Cross(_cameraTarget, Vector3.Up) * .1f + _camera;

            // rotate yaw
            if (keystate.IsKeyDown(Keys.E))
                _cameraTarget = Vector3.TransformCoordinate(_cameraTarget, Matrix.RotationAxis(Vector3.Up, MathUtil.DegreesToRadians(1)));

            if (keystate.IsKeyDown(Keys.Q))
                _cameraTarget = Vector3.TransformCoordinate(_cameraTarget, Matrix.RotationAxis(Vector3.Up, MathUtil.DegreesToRadians(-1)));

            // rotate pitch
            if (keystate.IsKeyDown(Keys.R))
            {
                var left = Vector3.Cross(_cameraTarget, Vector3.Up);
                var pitchMatrix = Matrix.RotationAxis(left, MathUtil.DegreesToRadians(1));
                _cameraTarget = Vector3.TransformCoordinate(_cameraTarget, pitchMatrix);
            }

            if (keystate.IsKeyDown(Keys.F))
            {
                var right = Vector3.Cross(_cameraTarget, Vector3.Down);
                var pitchmatrix = Matrix.RotationAxis(right, MathUtil.DegreesToRadians(1));
                _cameraTarget = Vector3.TransformCoordinate(_cameraTarget, pitchmatrix);
            }

            // Handle base.Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Calculate matrices
            _basicEffect.World = Matrix.Identity;
            _basicEffect.View = Matrix.LookAtLH(_camera, _cameraTarget + _camera, Vector3.Up);
            _basicEffect.Projection = Matrix.PerspectiveFovLH(MathUtil.Pi/3,
                (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 1000.0f);

            // Setup the vertices
            GraphicsDevice.SetVertexBuffer(_vertices);
            GraphicsDevice.SetVertexInputLayout(_inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            _basicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.Draw(PrimitiveType.TriangleList, _vertices.ElementCount);

            // Draw debug
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, String.Format("Controls: WASD to move, QE to control yaw, RF to control pitch"), new Vector2(0, 0), Color.Black);
            _spriteBatch.DrawString(_font, String.Format("Camera: {0}", _camera), new Vector2(0, 15), Color.Black);
            _spriteBatch.DrawString(_font, String.Format("LookAt: {0}", _cameraTarget), new Vector2(0, 30), Color.Black);
            _spriteBatch.End();

            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
