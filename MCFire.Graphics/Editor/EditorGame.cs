using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GongSolutions.Wpf.DragDrop;
using JetBrains.Annotations;
using MCFire.Common;
using MCFire.Common.Infrastructure.DragDrop;
using MCFire.Graphics.Modules.Editor.Models;
using MCFire.Graphics.Properties;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace MCFire.Graphics.Editor
{
    /// <summary>
    /// A SharpDx Game used to render a minecraft world.
    /// </summary>
    public sealed class EditorGame : Game, IEditorGame, IEditorGameFacade
    {
        readonly IEnumerable<IGameComponent> _components;
        // rendering
        BasicEffect _basicEffect;

        // TODO: unified content system
        // content
        // TODO: have the ability to switch IEditors, each IEditor would have its own components and basic classes(Camera, Tasks)
        public SpriteFont Font { get; private set; }
        Texture ErrorTexture { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorGame" /> class.
        /// </summary>
        /// <param name="sharpDxElement">The control used to listen to input events.</param>
        /// <param name="components">The game component services.</param>
        /// <param name="world">The Minecraft world to use as a data source.</param>
        /// <param name="dimension">The dimension of the world.</param>
        public EditorGame([NotNull] SharpDXElement sharpDxElement, [NotNull] IEnumerable<IGameComponent> components, [NotNull] World world,
            int dimension)
        {
            _components = (from component in components
                           orderby component.DrawPriority
                           select component).ToArray();
            World = world;
            Dimension = dimension;
            ToDisposeContent(new GraphicsDeviceManager(this));
            SharpDxElement = sharpDxElement;
            Content.RootDirectory = @"Editor\Content";
        }

        #region Components

        [CanBeNull]
        public TComponent GetComponent<TComponent>()
            where TComponent : class
        {
            return (from component in _components
                    where component is TComponent
                    select component).FirstOrDefault() as TComponent;
        }

        #endregion

        protected override void Update(GameTime gameTime)
        {
            Keyboard.Update();
            Mouse.Update();
            Camera.Update(gameTime);
            Tasks.Update(gameTime);

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

            // Draw debug
            SpriteBatch.Begin();
            SpriteBatch.DrawString(Font, String.Format("Controls: WASD to move, QE to control yaw, RF to control pitch"), new Vector2(0, 0), Color.Black);
            SpriteBatch.DrawString(Font, String.Format("Camera: {0}", Camera.Position), new Vector2(0, 15), Color.Black);
            SpriteBatch.DrawString(Font, String.Format("LookAt: {0}", Camera.Direction), new Vector2(0, 30), Color.Black);
            SpriteBatch.End();

            // Handle base.Draw
            base.Draw(gameTime);
        }

        #region Content

        protected override void LoadContent()
        {
            GraphicsDevice = base.GraphicsDevice;
            ErrorTexture = ToDispose(Texture2D.Load(GraphicsDevice, new MemoryStream(Resources.Error)));

            // rendering
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            _basicEffect = ToDisposeContent(new BasicEffect(GraphicsDevice)
            {
                VertexColorEnabled = true,
            });

            // input
            Keyboard = ToDispose(new Keyboard(this));
            Mouse = ToDispose(new Mouse(new MouseManager(this)));
            var camera = ToDispose(new Camera(this) { Position = new Vector3(0, 0, -5), Fov = MathUtil.PiOverTwo });
            camera.IdleRotate(new Vector3(0, 82, 4), 43, MathUtil.Pi / 16, .44f);
            Camera = camera;
            Tasks = new Tasks(this);

            foreach (var component in _components)
            {
                component.LoadContent(this);
            }

            // content
            Font = ToDisposeContent(Content.Load<SpriteFont>("Segoe12"));

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            foreach (var component in _components)
                component.Dispose();
        }

        public T LoadContent<T>(string assetName) where T : class,IDisposable
        {
            /* TODO: A content system where textures, audio, effects, models and fonts can be loaded anywhere.
             * The game would compile them (.fx, .png, .xml) into .tkb and seamlessly recompile if the source changes.
             * If errors occur, the old version is used and a message is overlayed detailing the error.
             * Compiled output is saved to %appdata%\MC Fire.
             * Reverse engineer SharpDX.Toolkit.EffectCompilerSystem
             */
            try
            {
                return Content.Load<T>(assetName);
            }
            catch (AssetNotFoundException)
            {
                // check if the content exists, if it does, copy it with the extension .tkb
                var path = Path.GetFullPath(@"./Editor/Content/");
                var files = Directory.GetFiles(path, assetName + ".*");
                foreach (var file in files)
                {
                    var extension = Path.GetExtension(file);

                    if (extension == "tkb")
                        continue;

                    var tkbPath = Path.ChangeExtension(file, "tkb");
                    File.Copy(file, tkbPath);
                    return Content.Load<T>(tkbPath);
                }

                if (typeof(T) == typeof(Texture2D))
                    return ErrorTexture as T;
                throw new AssetNotFoundException();
            }
        }

        public new T ToDisposeContent<T>(T asset) where T : IDisposable
        {
            return base.ToDisposeContent(asset);
        }

        protected override void Dispose(bool disposeManagedResources)
        {
            Exit();
            base.Dispose(disposeManagedResources);
        }

        #endregion

        #region Drag Drop

        #region Drag Source

        public void StartDrag(IDragInfo dragInfo)
        {
            var info = new HandleableDragInfo(dragInfo);
            foreach (var component in _components)
            {
                component.StartDrag(info);
                if (info.Handled) return;
            }
        }

        public void Dropped(IDropInfo dropInfo)
        {
            foreach (var component in _components)
            {
                component.Dropped(dropInfo);
                if (!dropInfo.NotHandled) return;
            }
        }

        public void DragCancelled()
        {
            foreach (var component in _components)
            {
                component.DragCancelled();
            }
        }

        #endregion

        #region Drop Target

        public void DragOver(IDropInfo dropInfo)
        {
            foreach (var component in _components)
            {
                component.DragOver(dropInfo);
                if (!dropInfo.NotHandled) return;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            foreach (var component in _components)
            {
                component.Drop(dropInfo);
                if (!dropInfo.NotHandled) return;
            }
        }

        #endregion

        #endregion

        [NotNull]
        public Camera Camera { get; private set; }

        [NotNull]
        public Mouse Mouse { get; private set; }

        [NotNull]
        public Keyboard Keyboard { get; private set; }

        [NotNull]
        public Tasks Tasks { get; private set; }

        public new GraphicsDevice GraphicsDevice { get; private set; }

        [NotNull]
        public SharpDXElement SharpDxElement { get; private set; }

        [NotNull]
        public World World { get; private set; }

        public int Dimension { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }

        public void WpfKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            foreach (var component in _components)
            {
                component.WpfKeyDown(e);
                if (e.Handled) return;
            }
        }
    }
}
