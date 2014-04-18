using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Caliburn.Micro;
using MCFire.Modules.Editor.Extensions;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.WorldExplorer.Services;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Substrate;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;
using Vector3 = SharpDX.Vector3;

namespace MCFire.Modules.Editor.Models
{
    // Use this namespace here in case we need to use Direct3D11 namespace as well, as this
    // namespace will override the Direct3D11.
    
    /// <summary>
    /// Simple MiniCube application using SharpDX.Toolkit.
    /// The purpose of this application is to show a rotating cube using <see cref="BasicEffect"/>.
    /// </summary>
    public class D3DTestGame : Game
    {
        // rendering
        SpriteBatch _spriteBatch;
        BasicEffect _basicEffect;
        readonly SharpDXElement _sharpDx;

        // content
        SpriteFont _font;
        VertexInputLayout _inputLayout;
        Buffer<VertexPositionColor> _vertices;

        // input
        Mouse _mouse;
        KeyboardManager _keyboard;
        bool _ignoreNextMoveEvent;

        // model
        Camera _camera;
        Buffer<VertexPositionColor> _chunkVertices;
        VertexInputLayout _chunkInputLayout;
        Mesh _chunkMesh;
        Effect _chunkEffect;

        // new fields
        readonly List<ChunkVisual> _chunks = new List<ChunkVisual>();
        readonly object _drawLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="D3DTestGame" /> class.
        /// </summary>
        /// <param name="sharpDx">The control used to listen to mouse drag events.</param>
        public D3DTestGame(SharpDXElement sharpDx)
        {
            ToDispose(new GraphicsDeviceManager(this));
            _sharpDx = sharpDx;
            Content.RootDirectory = @"Modules/Editor/Content";
            ViewDistance = 10;
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
                _camera.Pan(change * 2);
                Console.WriteLine(e.Position);

                // If mouse escapes the bounds of the SharpDxElement, loop it to the other side.
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (_ignoreNextMoveEvent)
                {
                    _ignoreNextMoveEvent = false;
                    return;
                }
                if (e.Position.X == 1)
                {
                    var newPos = _sharpDx.PointToScreen(new System.Windows.Point(0.01f, e.Position.Y * _sharpDx.ActualHeight));
                    _mouse.MoveSilently(newPos.ToVector2());
                    _ignoreNextMoveEvent = true;
                }
                else if (e.Position.X == 0)
                {
                    var newPos = _sharpDx.PointToScreen(new System.Windows.Point(_sharpDx.ActualWidth - 0.99f, e.Position.Y * _sharpDx.ActualHeight));
                    _mouse.MoveSilently(newPos.ToVector2());
                    _ignoreNextMoveEvent = true;
                }
                if (e.Position.Y == 1)
                {
                    var newPos = _sharpDx.PointToScreen(new System.Windows.Point(e.Position.X * _sharpDx.ActualWidth, 0.01f));
                    _mouse.MoveSilently(newPos.ToVector2());
                    _ignoreNextMoveEvent = true;
                }
                else if (e.Position.Y == 0)
                {
                    var newPos = _sharpDx.PointToScreen(new System.Windows.Point(e.Position.X * _sharpDx.ActualWidth, _sharpDx.ActualHeight - 0.99f));
                    _mouse.MoveSilently(newPos.ToVector2());
                    _ignoreNextMoveEvent = true;
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
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.Orange), // Front
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.Orange), // BACK
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.Orange),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.OrangeRed), // Top
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.OrangeRed), // Bottom
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.OrangeRed),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.DarkOrange), // Left
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.DarkOrange), // Right
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.DarkOrange),
                        new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.DarkOrange)
                    }));
            _inputLayout = VertexInputLayout.FromBuffer(0, _vertices);

            var install = IoC.Get<WorldExplorerService>().Installations.FirstOrDefault();
            if (install == null || !install.Worlds.Any())
                throw new Exception("Add an installation and a world to MC Fire before using the 3D test. The (0,0) coordinate must be generated for that world.");
            var worlds = install.Worlds;

            //var createThings = from world in worlds
            //                   let lastIndex = world.Path.LastIndexOf("\\", StringComparison.Ordinal)
            //                   where lastIndex != -1 && world.Path.Substring(lastIndex + 1) == "create things"
            //                   select world;
            var createThings = worlds.First();

            var chunkBlocks = createThings.GetChunkManager().GetChunk(0, 0).Blocks;

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
                            // face with normal x+
                            var xPlusBlock = chunkBlocks.GetBlock(x + 1, y, z);
                            if (xPlusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Right, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x + 1, y, z), chunkBlocks.GetSkyLight(x + 1, y, z)));
                            }
                        }

                        if (y + 1 < chunkBlocks.YDim)
                        {
                            var yPlusBlock = chunkBlocks.GetBlock(x, y + 1, z);
                            if (yPlusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Up, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x, y + 1, z), chunkBlocks.GetSkyLight(x, y + 1, z)));
                            }
                        }

                        if (z + 1 < chunkBlocks.ZDim)
                        {
                            var zPlubBlock = chunkBlocks.GetBlock(x, y, z + 1);
                            if (zPlubBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Backward, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x, y, z + 1), chunkBlocks.GetSkyLight(x, y, z + 1)));
                            }
                        }

                        if (x - 1 >= 0)
                        {
                            var xMinusBlock = chunkBlocks.GetBlock(x - 1, y, z);
                            if (xMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Left, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x - 1, y, z), chunkBlocks.GetSkyLight(x - 1, y, z)));
                            }
                        }

                        if (y - 1 >= 0)
                        {
                            var yMinusBlock = chunkBlocks.GetBlock(x, y - 1, z);
                            if (yMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Down, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x, y - 1, z), chunkBlocks.GetSkyLight(x, y - 1, z)));
                            }
                        }

                        if (z - 1 >= 0)
                        {
                            var zMinusBlock = chunkBlocks.GetBlock(x, y, z - 1);
                            if (zMinusBlock.Info.State == BlockState.NONSOLID || block.Info.State == BlockState.FLUID)
                            {
                                AddTriangleQuad(new Vector3(x, y, z), Forward, chunkVerticesList, (byte)Math.Max(chunkBlocks.GetBlockLight(x, y, z - 1), chunkBlocks.GetSkyLight(x, y, z - 1)));
                            }
                        }
                    }

            // create chunk mesh
            _chunkEffect = Content.Load<Effect>(@"VertexLit");

            _chunkVertices = ToDisposeContent(Buffer.Vertex.New(GraphicsDevice, chunkVerticesList.ToArray()));
            _chunkInputLayout = VertexInputLayout.FromBuffer(0, _chunkVertices);

            _chunkMesh = new Mesh
            {
                Effect = _basicEffect,
                VertexBuffer = _chunkVertices,
                VertexInputLayout = _chunkInputLayout
            };

            base.LoadContent();
        }

        static readonly Matrix Up = Matrix.Identity;
        static readonly Matrix Forward = Matrix.RotationX(-MathUtil.PiOverTwo) * Matrix.Translation(0, 0, 1);
        static readonly Matrix Down = Matrix.RotationX(MathUtil.Pi) * Matrix.Translation(0, 1, 1);
        static readonly Matrix Backward = Matrix.RotationX(MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0);
        static readonly Matrix Right = Matrix.RotationZ(-MathUtil.PiOverTwo) * Matrix.Translation(0, 1, 0);
        static readonly Matrix Left = Matrix.RotationZ(MathUtil.PiOverTwo) * Matrix.Translation(1, 0, 0);

        static readonly Vector3[] QuadVertices =
        {
            new Vector3(1,1,1),
            new Vector3(0,1,1),
            new Vector3(0,1,0),
            new Vector3(1,1,0),
            new Vector3(1,1,1),
            new Vector3(0,1,0)
        };

        public void AddChunkVisual(ChunkVisual chunk)
        {
            // TODO: use coordinate system, and replace the chunk that shares the same coordinate

            lock (_drawLock)
            {
                chunk.Initialize(_chunkEffect);
                _chunks.Add(chunk);
            }
        }

        /// <summary>
        /// Returns the position of the chunk that the game should generate next.
        /// </summary>
        /// <param name="point">The point in chunk space that should be generated</param>
        /// <returns>If any chunks should be generated</returns>
        public bool GetNextDesiredChunk(out Point point)
        {
            var cameraChunkPosition = new Point((int)_camera.Position.X / 16, (int)_camera.Position.Z / 16);

            foreach (var chunkPoint in _chunkPoints)
            {
                EditorChunk chunk;
                Point worldSpaceChunkPoint = chunkPoint.Add(cameraChunkPosition);
                if (!ChunkSource.TryGetValue(worldSpaceChunkPoint, out chunk))
                {
                    point = chunkPoint;
                    return true;
                }
            }

            // all chunks generated
            point = new Point();
            return false;
        }

        void GenerateChunkPoints()
        {
            var distance = ViewDistance;
            var points = new List<Point>(ViewDistance * ViewDistance * 4);
            //create points
            for (var i = -ViewDistance; i < ViewDistance; i++)
            {
                for (var j = -ViewDistance; j < ViewDistance; j++)
                {
                    points.Add(new Point(i, j));
                }
            }

            _chunkPoints = from point in points
                           // order by hypotenuse distance to (0,0)
                           orderby point.X * point.X + point.Y * point.Y
                           select point;
        }

        static void AddTriangleQuad(Vector3 location, Matrix direction, ICollection<VertexPositionColor> triangleMesh, byte luminance)
        {
            // 0-15 to 0-255
            luminance *= 16;

            foreach (var vertex in QuadVertices)
            {
                // rotate the vector to face the direction specified
                var transformed = Vector3.TransformCoordinate(vertex, direction);

                // translate the vector into world space
                transformed += location;

                // add the vector to the list as a vertex
                triangleMesh.Add(new VertexPositionColor(transformed, new Color(luminance, luminance, luminance, 255)));
                //triangleMesh.Add(new VertexPositionColor(transformed, new Color(luminance)));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            _camera.Update(_keyboard.GetState());
            _mouse.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            lock (_drawLock)
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

                // Draw chunk
                //GraphicsDevice.SetVertexBuffer(_chunkVertices);
                //GraphicsDevice.SetVertexInputLayout(_chunkInputLayout);
                //_basicEffect.CurrentTechnique.Passes[0].Apply();
                //GraphicsDevice.Draw(PrimitiveType.TriangleList, _chunkVertices.ElementCount);

                // Draw chunk using ModelMesh
                _chunkEffect.Parameters["TransformMatrix"].SetValue(Matrix.Identity * _camera.ViewMatrix * _camera.ProjectionMatrix);
                _chunkMesh.Draw(GraphicsDevice);
                foreach (var chunk in _chunks)
                {
                    chunk.Draw(GraphicsDevice, _camera);
                }
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

        public Dictionary<Point, EditorChunk> ChunkSource { get; set; }

        /// <summary>
        /// Must be greater than 0. Anything over 10 is pushing it.
        /// </summary>
        public int ViewDistance
        {
            get { return _viewDistance; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("ViewDistance");

                _viewDistance = value;
                GenerateChunkPoints();
            }
        }

        /// <summary>
        /// A list of all whole number points in a square of (√Length) width and height with the center of the square being the (0,0) point.
        /// This represents all of the visible chunks
        /// </summary>
        IEnumerable<Point> _chunkPoints;

        int _viewDistance;
    }
}
