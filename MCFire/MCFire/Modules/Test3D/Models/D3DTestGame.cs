using System;
using System.Windows.Input;
using MCFire.Modules.Test3D.Extensions;
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
        SharpDXElement _sharpDx;

        // content
        SpriteFont _font;
        VertexInputLayout _inputLayout;
        Buffer<VertexPositionColor> _vertices;

        // input
        MouseManager _mouse;
        MouseState _mousePrevState;
        bool _mouseRightDragging;
        KeyboardManager _keyboard;

        // model
        Camera _camera;

        /// <summary>
        /// Initializes a new instance of the <see cref="D3DTestGame" /> class.
        /// </summary>
        /// <param name="sharpDx">The control used to listen to mouse drag events.</param>
        public D3DTestGame(SharpDXElement sharpDx)
        {
            _sharpDx = sharpDx;
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
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
            _mousePrevState = _mouse.GetState();
            _keyboard = new KeyboardManager(this);
            _keyboard.Initialize();
            _camera = ToDispose(new Camera(GraphicsDevice) { Position = new Vector3(0, 0, -5) });
            _camera.LookAt(new Vector3(0, 0, 0));

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

        protected override void Update(GameTime gameTime)
        {
            var keyState = _keyboard.GetState();
            _camera.Update(keyState);

            var mouseState = _mouse.GetState();
            var mousePos = new Vector2(mouseState.X, mouseState.Y);
            var mousePrevPos = new Vector2(_mousePrevState.X, _mousePrevState.Y);

            if (mouseState.Right == ButtonState.Pressed && _mousePrevState.Right == ButtonState.Pressed)
            {// right held
                if (_mouseRightDragging || mousePos != mousePrevPos)
                {// right dragging
                    _mouseRightDragging = true;
                    // perspective drag
                    // TODO: refactor into class and event?
                    var change = new Vector2((mousePrevPos.X - mousePos.X) * GraphicsDevice.AspectRatio(), mousePrevPos.Y - mousePos.Y);
                    _camera.Pan(change);
                }
            }

            _mousePrevState = mouseState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Calculate matrices
            _basicEffect.World = Matrix.Identity;
            _basicEffect.View = _camera.ViewMatrix;
            _basicEffect.Projection = _camera.ProjectionMatrix;

            // Setup the vertices
            GraphicsDevice.SetVertexBuffer(_vertices);
            GraphicsDevice.SetVertexInputLayout(_inputLayout);

            // Apply the basic effect technique and draw the rotating cube
            _basicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.Draw(PrimitiveType.TriangleList, _vertices.ElementCount);

            // Draw debug
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, String.Format("Controls: WASD to move, QE to control yaw, RF to control pitch"), new Vector2(0, 0), Color.Black);
            _spriteBatch.DrawString(_font, String.Format("Camera: {0}", _camera.Position), new Vector2(0, 15), Color.Black);
            _spriteBatch.DrawString(_font, String.Format("LookAt: {0}", _camera.Direction), new Vector2(0, 30), Color.Black);
            _spriteBatch.End();

            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
