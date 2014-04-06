using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Caliburn.Micro;
using MCFire.Modules.Test3D.Extensions;
using MCFire.Modules.WorldExplorer.Services;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using Substrate;
using Vector3 = SharpDX.Vector3;

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
        SpriteBatch _spriteBatch;
        BasicEffect _basicEffect;
        SharpDXElement _sharpDx;

        // content
        SpriteFont _font;
        VertexInputLayout _inputLayout;
        Buffer<VertexPositionColor> _vertices;

        // input
        Mouse _mouse;
        KeyboardManager _keyboard;

        // model
        Camera _camera;
        Buffer<VertexPositionColor> _chunkVertices;
        VertexInputLayout _chunkInputLayout;

        /// <summary>
        /// Initializes a new instance of the <see cref="D3DTestGame" /> class.
        /// </summary>
        /// <param name="sharpDx">The control used to listen to mouse drag events.</param>
        public D3DTestGame(SharpDXElement sharpDx)
        {
            ToDispose(new GraphicsDeviceManager(this));
            _sharpDx = sharpDx;
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
            _keyboard = ToDispose(new KeyboardManager(this));
            _keyboard.Initialize();
            _camera = ToDispose(new Camera(GraphicsDevice) { Position = new Vector3(0, 0, -5), Fov = MathUtil.PiOverTwo });
            _camera.LookAt(new Vector3(0, 0, 0));
            _mouse = ToDispose(new Mouse(new MouseManager(this)));

            _mouse.Right.DragStart += (s, e) =>
            {
                _sharpDx.CaptureMouse();
                System.Windows.Input.Mouse.OverrideCursor = Cursors.None;
            };
            _mouse.Right.DragMove += (s, e) =>
            {
                // perspective drag
                var change = (e.PrevPosition - e.Position) * new Vector2(GraphicsDevice.AspectRatio(), 1);
                _camera.Pan(change);
                Console.WriteLine(e.Position);

                // If mouse escapes the bounds of the SharpDxElement, loop it to the other side.
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (e.Position.X == 1)
                {
                    var newPos = _sharpDx.PointToScreen(new System.Windows.Point(1, e.Position.Y * _sharpDx.ActualHeight));
                    _mouse.Move(newPos.ToVector2());
                }
                else if (e.Position.X == 0)
                {
                    var newPos = _sharpDx.PointToScreen(new System.Windows.Point(_sharpDx.ActualWidth - 1, e.Position.Y * _sharpDx.ActualHeight));
                    _mouse.Move(newPos.ToVector2());
                }

                if (e.Position.Y == 1)
                {
                    var newPos = _sharpDx.PointToScreen(new System.Windows.Point(e.Position.X * _sharpDx.ActualWidth, 1));
                    _mouse.Move(newPos.ToVector2());
                }
                else if (e.Position.Y == 0)
                {
                    var newPos = _sharpDx.PointToScreen(new System.Windows.Point(e.Position.X * _sharpDx.ActualWidth, _sharpDx.ActualHeight - 1));
                    _mouse.Move(newPos.ToVector2());
                }
                // ReSharper restore CompareOfFloatsByEqualityOperator
            };
            _mouse.Right.DragEnd += (s, e) =>
            {
                _sharpDx.ReleaseMouseCapture();
                System.Windows.Input.Mouse.OverrideCursor = Cursors.Arrow;
            };

            // content
            _font = ToDisposeContent(Content.Load<SpriteFont>("Segoe12"));
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
            _inputLayout = VertexInputLayout.FromBuffer(0, _vertices);

            var worlds = IoC.Get<WorldExplorerService>().Installations.First().Worlds;
            var createThings = from world in worlds
                               let lastIndex = world.Path.LastIndexOf("\\", StringComparison.Ordinal)
                               where lastIndex != -1 && world.Path.Substring(lastIndex + 1) == "create things"
                               select world;
            var chunkBlocks = createThings.First().GetChunkManager().GetChunk(0, 0).Blocks;

            var chunkVerticesList = new List<VertexPositionColor>(500);
            for (var y = 0; y < chunkBlocks.YDim; y++)
                for (var x = 0; x < chunkBlocks.XDim; x++)
                    for (var z = 0; z < chunkBlocks.ZDim; z++)
                    {
                        var block = chunkBlocks.GetBlock(x, y, z);

                        if (block.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            continue;
                        // block isn't air, assume its solid.
                        // at this point, texture coordinates should be calculated
                        // block specific shapes should happen (eg water)

                        if (x + 1 < chunkBlocks.XDim)
                        {
                            var xPlusBlock = chunkBlocks.GetBlock(x + 1, y, z);
                            if (xPlusBlock.Info == BlockInfo.Air)
                            {
                                chunkVerticesList.Add(new VertexPositionColor(new Vector3(x + 1.0f, y, z), Color.LightGray));
                                chunkVerticesList.Add(new VertexPositionColor(new Vector3(x + 1.0f, y + 1.0f, z + 1.0f),
                                    Color.LightGray));
                                chunkVerticesList.Add(new VertexPositionColor(new Vector3(x + 1.0f, y, z + 1.0f),
                                    Color.LightGray));
                                chunkVerticesList.Add(new VertexPositionColor(new Vector3(x + 1.0f, y, z), Color.LightGray));
                                chunkVerticesList.Add(new VertexPositionColor(new Vector3(x + 1.0f, y + 1.0f, z),
                                    Color.LightGray));
                                chunkVerticesList.Add(new VertexPositionColor(new Vector3(x + 1.0f, y + 1.0f, z + 1.0f),
                                    Color.LightGray));
                            }
                        }
                    }

            _chunkVertices = ToDisposeContent(Buffer.Vertex.New(GraphicsDevice, chunkVerticesList.ToArray()));
            _chunkInputLayout = VertexInputLayout.FromBuffer(0, _chunkVertices);

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _camera.Update(_keyboard.GetState());
            _mouse.Update();

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

            // Draw the cube
            GraphicsDevice.SetVertexBuffer(_vertices);
            GraphicsDevice.SetVertexInputLayout(_inputLayout);
            _basicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.Draw(PrimitiveType.TriangleList, _vertices.ElementCount);

            // Draw chunks
            GraphicsDevice.SetVertexBuffer(_chunkVertices);
            GraphicsDevice.SetVertexInputLayout(_chunkInputLayout);
            _basicEffect.CurrentTechnique.Passes[0].Apply();
            GraphicsDevice.Draw(PrimitiveType.TriangleList, _chunkVertices.ElementCount);

            // Draw debug
            _spriteBatch.Begin();
            _spriteBatch.DrawString(_font, String.Format("Controls: WASD to move, QE to control yaw, RF to control pitch"), new Vector2(0, 0), Color.Black);
            _spriteBatch.DrawString(_font, String.Format("Camera: {0}", _camera.Position), new Vector2(0, 15), Color.Black);
            _spriteBatch.DrawString(_font, String.Format("LookAt: {0}", _camera.Direction), new Vector2(0, 30), Color.Black);
            _spriteBatch.End();

            // Reset
            GraphicsDevice.Flush();

            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
