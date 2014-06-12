using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Infrastructure.Extensions
{
    public static class GraphicsDeviceExtensions
    {
        public static float AspectRatio(this GraphicsDevice graphicsDevice)
        {
            return (float)graphicsDevice.BackBuffer.Width / graphicsDevice.BackBuffer.Height;
        }
    }
}
