using System.ComponentModel.Composition;
using Gemini.Framework;
using MCFire.Modules.Infrastructure.D3D.Controls;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.HelloWorld.Test3D.ViewModels
{
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [Export]
    public class D3DViewModel : Document
    {
        GeometricPrimitive _primitive;
        BasicEffect _basicEffect;

        public void OnLoadContent(object sender, GraphicsDeviceEventArgs e)
        {
            _primitive = GeometricPrimitive.Teapot.New(e.GraphicsDevice);
            _basicEffect = new BasicEffect(e.GraphicsDevice)
            {
                View = Matrix.LookAtLH(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY),
                World = Matrix.Identity,
                PreferPerPixelLighting = true,
            };
            _basicEffect.EnableDefaultLighting();
            _basicEffect.Texture = Texture2D.Load(e.GraphicsDevice, "Test3D/Resources/Fabric.png");
            _basicEffect.TextureEnabled = true;
        }
    }
}
