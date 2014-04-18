using System;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Editor.Models
{
    /// <summary>
    /// A simple wrapper for a SharpDx vertex buffer, and its Effect and VertexInputLayout.
    /// </summary>
    public class Mesh : IDisposable
    {
        public VertexInputLayout VertexInputLayout;

        /// <summary>
        /// The vertex buffer range for this mesh part.
        /// </summary>
        public Buffer<VertexPositionColor> VertexBuffer;

        /// <summary>Gets or sets the material Effect for this mesh part.  Reference page contains code sample.</summary>
        public Effect Effect;

        bool _disposed;

        /// <summary>
        /// Draws this <see cref="Mesh"/>. See remarks for difference with XNA.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <remarks>
        /// Unlike XNA, a <see cref="Mesh"/> is not bound to a specific Effect. The effect must have been setup prior calling this method.
        /// This method is only responsible to setup the VertexBuffer, IndexBuffer and call the appropriate <see cref="GraphicsDevice.DrawIndexed"/> method on the <see cref="GraphicsDevice"/>.
        /// </remarks>
        public void Draw(GraphicsDevice graphicsDevice)
        {
            if (_disposed)
                throw new ObjectDisposedException("Mesh");

            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.SetVertexInputLayout(VertexInputLayout);

            var passCount = Effect.CurrentTechnique.Passes.Count;
            for (var j = 0; j < passCount; j++)
            {
                Effect.CurrentTechnique.Passes[j].Apply();
                graphicsDevice.Draw(PrimitiveType.TriangleList, VertexBuffer.ElementCount);
            }
        }

        /// <summary>
        /// Disposes the VertexBuffer, but not the Effect(!)
        /// </summary>
        public void Dispose()
        {
            if (VertexBuffer != null)
            {
                VertexBuffer.Dispose();
                VertexBuffer = null;
            }

            _disposed = true;
        }

    }
}