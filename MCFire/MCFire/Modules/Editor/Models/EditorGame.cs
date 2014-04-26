using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Modules.Explorer.Models;
using MCFire.Modules.Infrastructure.Models;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using Substrate;
using Point = MCFire.Modules.Infrastructure.Models.Point;
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
        IEnumerable<IGameComponent> _components;
        // rendering
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

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorGame" /> class.
        /// </summary>
        /// <param name="sharpDxElement">The control used to listen to input events.</param>
        /// <param name="components">The game component services.</param>
        /// <param name="world">The Minecraft world to use as a data source.</param>
        /// <param name="substrateWorld"></param>
        /// <param name="dimension">The dimension of the world.</param>
        public EditorGame([NotNull] SharpDXElement sharpDxElement, [CanBeNull] IEnumerable<IGameComponent> components, [NotNull] MCFireWorld world,
            [NotNull] NbtWorld substrateWorld, int dimension)
        {
            _components = components;
            World = world;
            SubstrateWorld = substrateWorld;
            Dimension = dimension;

            ToDispose(new GraphicsDeviceManager(this));
            SharpDxElement = sharpDxElement;
            Content.RootDirectory = @"Modules/Editor/Content";
        }

        protected override void LoadContent()
        {
            ErrorTexture = ToDisposeContent(Content.Load<Texture2D>("Error"));

            // rendering
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _basicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
            });

            // input
            Keyboard = ToDispose(new Keyboard(this));
            Mouse = ToDispose(new Mouse(new MouseManager(this)));
            Camera = ToDispose(new Camera(this) { Position = new Vector3(0, 0, -5), Fov = MathUtil.PiOverTwo });
            Camera.IdleRotate(new Vector3(0, 82, 4), 43, MathUtil.Pi / 16, .44f);
            GameUser = new GameUser(this);

            foreach (var component in _components)
            {
                component.Game = this;
                component.LoadContent();
            }

            // content
            Font = ToDisposeContent(Content.Load<SpriteFont>("Segoe12"));
#if DEBUG
            _debugCube = ToDisposeContent(new DebugCube(this));
#endif

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            foreach (var component in _components)
            {
                component.Dispose();
            }
            _components = null;
            SharpDxElement = null;
            World = null;
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

        public AlphaBlock GetBlock(Point3 position)
        {
            return GetBlock(position.X, position.Y, position.Z);
        }

        // TODO: move this
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

            foreach (var component in _components)
            {
                component.Update(gameTime);
            }

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

            // draw components
            foreach (var component in _components)
            {
                component.Draw(gameTime);
            }

#if DEBUG
            // Draw the cube
            _debugCube.Draw(this);
#endif

            // Draw debug
            SpriteBatch.Begin();
            SpriteBatch.DrawString(Font, String.Format("Controls: WASD to move, QE to control yaw, RF to control pitch"), new Vector2(0, 0), Color.Black);
            SpriteBatch.DrawString(Font, String.Format("Camera: {0}", Camera.Position), new Vector2(0, 15), Color.Black);
            SpriteBatch.DrawString(Font, String.Format("LookAt: {0}", Camera.Direction), new Vector2(0, 30), Color.Black);
            SpriteBatch.End();

            // Reset
            GraphicsDevice.Flush();

            // Handle base.Draw
            base.Draw(gameTime);
        }

        [NotNull]
        public Camera Camera { get; private set; }

        [NotNull]
        public Mouse Mouse { get; private set; }

        [NotNull]
        public Keyboard Keyboard { get; private set; }

        [NotNull]
        public GameUser GameUser { get; private set; }

        [NotNull]
        public SharpDXElement SharpDxElement { get; private set; }

        [NotNull]
        public MCFireWorld World { get; private set; }

        [NotNull]
        public NbtWorld SubstrateWorld { get; private set; }
        public int Dimension { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
    }
}
