using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using MCFire.Modules.Editor.Extensions;
using MCFire.Modules.Infrastructure.Extensions;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
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
    public sealed class EditorGame : Game
    {
        // rendering
        SpriteBatch _spriteBatch;
        BasicEffect _basicEffect;
        readonly SharpDXElement _sharpDx;
        public Meshalyzer Meshalyzer { get; private set; }
        public Texture ErrorTexture { get; private set; }

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
        readonly List<VisualChunk> _chunksOldOldOld = new List<VisualChunk>();
        readonly object _drawLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorGame" /> class.
        /// </summary>
        /// <param name="sharpDx">The control used to listen to mouse drag events.</param>
        public EditorGame(SharpDXElement sharpDx)
        {
            ToDispose(new GraphicsDeviceManager(this));
            _sharpDx = sharpDx;
            Content.RootDirectory = @"Modules/Editor/Content";
            ViewDistance = 10;
        }

        protected override void LoadContent()
        {
            ErrorTexture = ToDisposeContent(Content.Load<Texture2D>("Error"));

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
            Camera.LookAt(new Vector3(0, 0, 0));
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
                Camera.Pan(change * 2);

                // If mouse escapes the bounds of the SharpDxElement, loop it to the other side.
                // ReSharper disable CompareOfFloatsByEqualityOperator
                if (_ignoreNextMoveEvent)
                {
                    _ignoreNextMoveEvent = false;
                    return;
                }
                if (e.Position.X == 1)
                {
                    _mouse.MoveSilently(new Vector2(0.01f, e.Position.Y));
                    _ignoreNextMoveEvent = true;
                }
                else if (e.Position.X == 0)
                {
                    _mouse.MoveSilently(new Vector2(0.99f, e.Position.Y));
                    _ignoreNextMoveEvent = true;
                }
                if (e.Position.Y == 1)
                {
                    _mouse.MoveSilently(new Vector2(e.Position.X, 0.01f));
                    _ignoreNextMoveEvent = true;
                }
                else if (e.Position.Y == 0)
                {
                    _mouse.MoveSilently(new Vector2(e.Position.X, 0.99f));
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

            base.LoadContent();
        }

        public T LoadContent<T>(string assetName) where T : class,IDisposable
        {
            T asset = null;
            try
            {
                asset = Content.Load<T>(assetName);
            }
            catch (AssetNotFoundException ex)
            {
                if (typeof(T).IsAssignableFrom(typeof(Texture2D)))
                    return ErrorTexture as T;
            }

            // return asset, if its null then safe cast the error texture to T and return.
            return asset != null ? ToDisposeContent(asset) : ErrorTexture as T;
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

        public void AddChunk(MCFireChunk chunk)
        {
            lock (_chunks)
            {
                // If a chunk with the same coords is found, replace it.
                for (var i = 0; i < _chunks.Count; i++)
                {
                    var chunk2 = _chunks[i].SubstrateChunk;
                    if (chunk2.X != chunk.SubstrateChunk.X || chunk2.Z != chunk.SubstrateChunk.Z) continue;

                    // replace the chunk
                    _chunks[i] = chunk;
                    return;
                }

                // no old chunk found, just add it.
                _chunks.Add(chunk);
            }
        }
        // TODO: remove chunks that are out of range of ViewDistance
        /// <summary>
        /// Returns the position of the chunk that the game should generate next.
        /// </summary>
        /// <param name="point">The point in chunk space that should be generated</param>
        /// <returns>If any chunks should be generated</returns>
        public bool GetNextDesiredChunk(out Point point)
        {
            var cameraChunkPosition = new Point((int)Camera.Position.X / 16, (int)Camera.Position.Z / 16);
            foreach (var chunkPoint in _chunkPoints)
            {
                MCFireChunk chunk;
                Point worldSpaceChunkPoint = chunkPoint.Add(cameraChunkPosition);
                // TODO: replace _chunks with Dictionary of Point, Chunk for fast lookup.
                lock (_chunks)
                {
                    if (_chunks.Any(testChunk => testChunk.SubstrateChunk.X == worldSpaceChunkPoint.X && testChunk.SubstrateChunk.Z == worldSpaceChunkPoint.Y))
                        continue;
                }

                point = chunkPoint;
                return true;
            }

            // all chunks generated
            point = new Point();
            return false;
        }

        void GenerateChunkPoints()
        {
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
            Camera.Update(_keyboard.GetState());
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
                _basicEffect.View = Camera.ViewMatrix;
                _basicEffect.Projection = Camera.ProjectionMatrix;

                // Draw the cube
                GraphicsDevice.SetVertexBuffer(_vertices);
                GraphicsDevice.SetVertexInputLayout(_inputLayout);
                _basicEffect.CurrentTechnique.Passes[0].Apply();
                GraphicsDevice.Draw(PrimitiveType.TriangleList, _vertices.ElementCount);

                // Draw chunk
                lock (_chunks)
                {
                    foreach (var chunk in _chunks)
                    {
                        if (chunk.Visual != null)
                            chunk.Visual.Draw(this);
                    }
                }

                // Draw debug
                _spriteBatch.Begin();
                _spriteBatch.DrawString(_font, String.Format("Controls: WASD to move, QE to control yaw, RF to control pitch"), new Vector2(0, 0), Color.Black);
                _spriteBatch.DrawString(_font, String.Format("Camera: {0}", Camera.Position), new Vector2(0, 15), Color.Black);
                _spriteBatch.DrawString(_font, String.Format("LookAt: {0}", Camera.Direction), new Vector2(0, 30), Color.Black);
                _spriteBatch.End();

                // Reset
                //GraphicsDevice.Flush();

                // Handle base.Draw
                base.Draw(gameTime);
            }
        }

        public Dictionary<Point, MCFireChunk> ChunkSource { get; set; }

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

        public Camera Camera
        {
            get { return _camera; }
        }

        /// <summary>
        /// A list of all whole number points in a square of (√Length) width and height with the center of the square being the (0,0) point.
        /// This represents all of the visible chunks
        /// </summary>
        IEnumerable<Point> _chunkPoints;

        int _viewDistance;
        List<MCFireChunk> _chunks = new List<MCFireChunk>();
    }
}
