using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Editor.Extensions
{
    public static class GraphicsDeviceExtensions
    {
        public static float AspectRatio(this GraphicsDevice graphicsDevice)
        {
            return (float)graphicsDevice.BackBuffer.Width / graphicsDevice.BackBuffer.Height;
        }
    }
}
