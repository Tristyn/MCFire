using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using MCFire.Modules.Editor.Extensions;
using MCFire.Modules.Infrastructure.Extensions;
using MCFire.Modules.Infrastructure.Models;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Substrate;
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
#if DEBUG
        DebugCube _debugCube;
#endif

        // content
        public SpriteFont Font { get; private set; }
        Texture ErrorTexture { get; set; }

        /// <summary>
        /// A list of all whole number points in a square of (√Length) wide with the center of the square being the (0,0) point.
        /// This represents all of the visible chunks
        /// </summary>
        IEnumerable<Point> _chunkPoints;
        int _viewDistance;
        readonly List<VisualChunk> _chunkVisuals = new List<VisualChunk>();
        readonly Stack<VisualChunk> _chunkVisualsToBeAdded = new Stack<VisualChunk>();
        readonly object _chunksToBeAddedLock = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorGame" /> class.
        /// </summary>
        /// <param name="sharpDxElement">The control used to listen to mouse drag events.</param>
        public EditorGame(SharpDXElement sharpDxElement)
        {
            ToDispose(new GraphicsDeviceManager(this));
            SharpDxElement = sharpDxElement;
            Content.RootDirectory = @"Modules/Editor/Content";
            ViewDistance = 10;
            Disposing += DisposeAllChunks;
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
            Keyboard = ToDispose(new Keyboard(this));
            Mouse = ToDispose(new Mouse(new MouseManager(this)));
            Camera = ToDispose(new Camera(this) { Position = new Vector3(0, 0, -5), Fov = MathUtil.PiOverTwo });
            Camera.LookAt(new Vector3(0, 0, 0));
            GameUser = new GameUser(this);

            // content
            Font = ToDisposeContent(Content.Load<SpriteFont>("Segoe12"));
#if DEBUG
            _debugCube = ToDisposeContent(new DebugCube(this));
#endif

            base.LoadContent();
        }

        public T LoadContent<T>(string assetName) where T : class,IDisposable
        {
            T asset;
            try
            {
                asset = Content.Load<T>(assetName);
            }
            catch (AssetNotFoundException)
            {
                return ErrorTexture as T;
            }

            // return asset, if its null then safe cast the error texture to T and return.
            return asset != null ? ToDisposeContent(asset) : ErrorTexture as T;
        }

        public new T ToDisposeContent<T>(T asset) where T : IDisposable
        {
            return base.ToDisposeContent(asset);
        }

        #region Chunks

        /// <summary>
        /// Adds a VisualChunk to this editor. 
        /// It will be disposed of properly when the time comes.
        /// This is a threadsafe call.
        /// </summary>
        /// <param name="visual"></param>
        public void AddChunk(VisualChunk visual)
        {
            /* TODO: make the editor not rely on any ChunkRefs, interaction code (hit tests, ect) should be in VisualChunk.
             * therefor, EditorBridge can infer the data of the chunk based on VisualChunk.
             * I guess the only other data needed in VisualChunk other than meshes, is the collision data required for selections.
             * If _chunkVisuals is a thread safe dictionary, locking wont be necessary (enumerating through values tho? enumerations are evil)
             * */
            lock (_chunksToBeAddedLock)
            {
                _chunkVisualsToBeAdded.Push(visual);
            }
        }

        void IntegrateNewChunks()
        {
            var isCullingChunks = false;
            lock (_chunkVisuals)
            {
                lock (_chunksToBeAddedLock)
                {
                    if (_chunkVisualsToBeAdded.Any())
                        isCullingChunks = true;

                    while (_chunkVisualsToBeAdded.Any())
                    {
                        // this would be faster if the last element is removed, but im lazy.
                        var chunkVisual = _chunkVisualsToBeAdded.Pop();

                        var chunkPoint = chunkVisual.ChunkPosition;

                        // If a chunk with the same coords is found, replace it.
                        for (var j = 0; j < _chunkVisuals.Count; j++)
                        {
                            var existingChunkPoint = _chunkVisuals[j].ChunkPosition;
                            if (chunkPoint != existingChunkPoint) continue;

                            // replace the chunk
                            _chunkVisuals[j] = chunkVisual;
                            return;
                        }

                        // no old chunk found, just add it.
                        _chunkVisuals.Add(chunkVisual);
                    }
                }
                if (isCullingChunks)
                    RemoveDistantChunks();
            }
        }

        /// <summary>
        /// Removes chunks that are out of range of ViewDistance
        /// </summary>
        void RemoveDistantChunks()
        {
            if (_chunkVisuals.Count <= ViewDistance * ViewDistance * 4 * 1.1f)
                return;

            var cameraPos = Camera.ChunkPosition;

            for (var i = 0; i < _chunkVisuals.Count; i++)
            {
                var chunk = _chunkVisuals[i];
                if (!(Math.Abs(chunk.ChunkPosition.X - cameraPos.X) > ViewDistance ||
                      Math.Abs(chunk.ChunkPosition.Y - cameraPos.Y) > ViewDistance))
                    continue;

                // remove it
                chunk.Dispose();
                _chunkVisuals.RemoveAt(i);
            }
            Console.WriteLine("Culled chunks, {0} chunks active.", _chunkVisuals.Count);
        }

        /// <summary>
        /// Disposes of and removes all chunks in the editor.
        /// </summary>
        void DisposeAllChunks(object s, EventArgs e)
        {
            lock (_chunkVisuals)
            {
                // go backwards cause preformance :)
                for (var i = _chunkVisuals.Count - 1; i >= 0; i--)
                {
                    _chunkVisuals[i].Dispose();
                    _chunkVisuals.RemoveAt(i);
                }
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
            lock (_chunkVisuals)
            {
                foreach (var chunkPoint in _chunkPoints)
                {
                    var worldSpaceChunkPoint = chunkPoint.Add(Camera.ChunkPosition);
                    // TODO: replace _chunks with Dictionary of Point, CurrentChunk for fast lookup.
                    // if a chunk with the position already exists, continue
                    if (_chunkVisuals.Any(testChunk => testChunk.ChunkPosition == worldSpaceChunkPoint))
                        continue;

                    // chunk doesn't exist yet, return its position
                    point = worldSpaceChunkPoint;
                    return true;
                }
            }

            // all chunks within ViewDistance are visible, return false
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

        public AlphaBlock GetBlock(Point3 position)
        {
            return GetBlock(position.X, position.Y, position.Z);
        }

        /// <summary>
        /// Returns the block at the specified position.
        /// Returns air (0) if the coord is in the void, above the sky limit, if the chunk doesn't exist or hasn't been created.
        /// </summary>
        public AlphaBlock GetBlock(int x, int y, int z)
        {
            if (y < 0) return new AlphaBlock(BlockType.AIR);
            var chunkPos = new Point(x / 16, z / 16);
            var chunk = _chunkVisuals.FirstOrDefault(chnk => chnk.ChunkPosition == chunkPos);
            if (chunk == null) return new AlphaBlock(BlockType.AIR);
            var chunkRef = chunk.ChunkRef;
            if (chunkRef == null) return new AlphaBlock(BlockType.AIR);
            var blocks = chunkRef.Blocks;
            if (y >= blocks.YDim) return new AlphaBlock(BlockType.AIR);
            return blocks.GetBlock(Mod(x, 16), y, Mod(z, 16));
        }

        static int Mod(int x, int m)
        {
            var r = x % m;
            return r < 0 ? r + m : r;
        }

        #endregion

        protected override void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();
            Camera.Update(gameTime);
            GameUser.Update(gameTime);

            IntegrateNewChunks();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Calculate matrices
            _basicEffect.World = Matrix.Identity;
            _basicEffect.View = Camera.ViewMatrix;
            _basicEffect.Projection = Camera.ProjectionMatrix;

            // Draw chunk
            // TODO: 
            lock (_chunkVisuals)
            {
                foreach (var chunk in _chunkVisuals)
                {
                    chunk.Draw(this);
                }
            }

#if DEBUG
            // Draw the cube
            _debugCube.Draw(this);
#endif

            // Draw debug
            _spriteBatch.Begin();
            _spriteBatch.DrawString(Font, String.Format("Controls: WASD to move, QE to control yaw, RF to control pitch"), new Vector2(0, 0), Color.Black);
            _spriteBatch.DrawString(Font, String.Format("Camera: {0}", Camera.Position), new Vector2(0, 15), Color.Black);
            _spriteBatch.DrawString(Font, String.Format("LookAt: {0}", Camera.Direction), new Vector2(0, 30), Color.Black);
            _spriteBatch.End();

            // Reset
            GraphicsDevice.Flush();

            // Handle base.Draw
            base.Draw(gameTime);
        }

        /// <summary>
        /// Must be greater than 0. Anything over 10 is pushing it.
        /// </summary>
        public int ViewDistance
        {
            get { return _viewDistance; }
            set
            {
                if (value <= 0)
                    value = 1;

                _viewDistance = value;
                GenerateChunkPoints();
            }
        }

        public Camera Camera { get; private set; }

        public Mouse Mouse { get; private set; }

        public Keyboard Keyboard { get; private set; }

        public GameUser GameUser { get; private set; }

        public SharpDXElement SharpDxElement { get; private set; }
    }
}
