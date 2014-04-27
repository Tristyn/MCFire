using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Editor.Models
{
    // Use these namespaces here to override SharpDX.Direct3D11
    
    /// <summary>
    /// Simple MCFire game using SharpDX.Toolkit.
    /// </summary>
    public class CoolGame : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;

        private Matrix view;
        private Matrix projection;

        private Model model;

        private Effect bloomEffect;
        private RenderTarget2D renderTargetOffScreen;
        private RenderTarget2D[] renderTargetDownScales;
        private RenderTarget2D renderTargetBlurTemp;

        /// <summary>
        /// Initializes a new instance of the <see cref="MCFire" /> class.
        /// </summary>
        public CoolGame()
        {
            // Creates a graphics manager. This is mandatory.
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = @"Modules/Test3D/Content";
        }

        protected override void Initialize()
        {
            // Modify the title of the window
            Window.Title = "MCFire";

            base.Initialize();
        }

        protected override void LoadContent()
        {

            // Load a 3D model
            // The [Ship.fbx] file is defined with the build action [ToolkitModel] in the project
            model = Content.Load<Model>("Ship");

            // Enable default lighting on model.
            BasicEffect.EnableDefaultLighting(model, true);

            // Bloom Effect
            // The [Bloom.fx] file is defined with the build action [ToolkitFxc] in the project
            bloomEffect = Content.Load<Effect>("Bloom");

            // Creates render targets for bloom effect
            renderTargetDownScales = new RenderTarget2D[5];
            var backDesc = GraphicsDevice.BackBuffer.Description;
            renderTargetOffScreen = ToDisposeContent(RenderTarget2D.New(GraphicsDevice, backDesc.Width, backDesc.Height, 1, backDesc.Format));
            for (int i = 0; i < renderTargetDownScales.Length; i++)
            {
                renderTargetDownScales[i] = ToDisposeContent(RenderTarget2D.New(GraphicsDevice, backDesc.Width >> i, backDesc.Height >> i, 1, backDesc.Format));
            }
            renderTargetBlurTemp = ToDisposeContent((RenderTarget2D)renderTargetDownScales[renderTargetDownScales.Length - 1].Clone());

            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Calculates the world and the view based on the model size
            view = Matrix.LookAtLH(new Vector3(0.0f, 0.0f, -7.0f), new Vector3(0, 0.0f, 0), Vector3.UnitY);
            projection = Matrix.PerspectiveFovLH(0.9f, (float)GraphicsDevice.BackBuffer.Width / GraphicsDevice.BackBuffer.Height, 0.1f, 100.0f);

        }

        protected override void Draw(GameTime gameTime)
        {
            // Use time in seconds directly
            var time = (float)gameTime.TotalGameTime.TotalSeconds;

            // Make offline rendering
            GraphicsDevice.SetRenderTargets(GraphicsDevice.DepthStencilBuffer, renderTargetOffScreen);

            // Clears the screen with the Color.CornflowerBlue
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Constant used to translate 3d models
            float translateX = 0.0f;

            // ------------------------------------------------------------------------
            // Draw the 3d model
            // ------------------------------------------------------------------------
            var world = Matrix.Scaling(0.003f) *
                        Matrix.RotationY(time) *
                        Matrix.Translation(0, -1.5f, 2.0f);
            model.Draw(GraphicsDevice, world, view, projection);
            translateX += 3.5f;

            // ------------------------------------------------------------------------
            // Cheap bloom post effect
            // Blur applied only on latest downscale render target
            // ------------------------------------------------------------------------

            // Setup states for posteffect
            GraphicsDevice.SetRasterizerState(GraphicsDevice.RasterizerStates.Default);
            GraphicsDevice.SetBlendState(GraphicsDevice.BlendStates.Default);
            GraphicsDevice.SetDepthStencilState(GraphicsDevice.DepthStencilStates.None);

            // Apply BrightPass
            const float brightPassThreshold = 0.5f;
            GraphicsDevice.SetRenderTargets(renderTargetDownScales[0]);
            bloomEffect.CurrentTechnique = bloomEffect.Techniques["BrightPassTechnique"];
            bloomEffect.Parameters["Texture"].SetResource(renderTargetOffScreen);
            bloomEffect.Parameters["PointSampler"].SetResource(GraphicsDevice.SamplerStates.PointClamp);
            bloomEffect.Parameters["BrightPassThreshold"].SetValue(brightPassThreshold);
            GraphicsDevice.DrawQuad(bloomEffect.CurrentTechnique.Passes[0]);

            // Down scale passes
            for (int i = 1; i < renderTargetDownScales.Length; i++)
            {
                GraphicsDevice.SetRenderTargets(renderTargetDownScales[i]);
                GraphicsDevice.DrawQuad(renderTargetDownScales[0]);
            }

            // Horizontal blur pass
            var renderTargetBlur = renderTargetDownScales[renderTargetDownScales.Length - 1];
            GraphicsDevice.SetRenderTargets(renderTargetBlurTemp);
            bloomEffect.CurrentTechnique = bloomEffect.Techniques["BlurPassTechnique"];
            bloomEffect.Parameters["Texture"].SetResource(renderTargetBlur);
            bloomEffect.Parameters["LinearSampler"].SetResource(GraphicsDevice.SamplerStates.LinearClamp);
            bloomEffect.Parameters["TextureTexelSize"].SetValue(new Vector2(1.0f / renderTargetBlurTemp.Width, 1.0f / renderTargetBlurTemp.Height));
            GraphicsDevice.DrawQuad(bloomEffect.CurrentTechnique.Passes[0]);

            // Vertical blur pass
            GraphicsDevice.SetRenderTargets(renderTargetBlur);
            bloomEffect.Parameters["Texture"].SetResource(renderTargetBlurTemp);
            GraphicsDevice.DrawQuad(bloomEffect.CurrentTechnique.Passes[1]);

            // Render to screen
            GraphicsDevice.SetRenderTargets(GraphicsDevice.BackBuffer);
            GraphicsDevice.DrawQuad(renderTargetOffScreen);

            // Add bloom on top of it
            GraphicsDevice.SetBlendState(GraphicsDevice.BlendStates.Additive);
            GraphicsDevice.DrawQuad(renderTargetBlur);
            GraphicsDevice.SetBlendState(GraphicsDevice.BlendStates.Default);

            base.Draw(gameTime);
        }
    }
}
