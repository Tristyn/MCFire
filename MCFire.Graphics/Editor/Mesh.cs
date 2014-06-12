using System;
using JetBrains.Annotations;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Editor
{
    /// <summary>
    /// A simple wrapper for a SharpDx vertex buffer, its Effect and VertexInputLayout.
    /// </summary>
    public class Mesh<T> : IDisposable where T : struct
    {
        bool _disposed;
        readonly bool _disposeEffect;

        [NotNull]
        readonly VertexInputLayout _vertexInputLayout;

        /// <summary>
        /// The vertex buffer range for this mesh part.
        /// </summary>
        [NotNull]
        Buffer<T> _vertexBuffer;

        /// <summary>Gets or sets the material Effect for this mesh part.</summary>
        [NotNull]
        readonly Effect _effect;

        public Mesh([NotNull] Buffer<T> vertexBuffer, [NotNull] Effect effect, bool disposeEffect = false)
        {
            if (vertexBuffer == null) throw new ArgumentNullException("vertexBuffer");
            if (effect == null) throw new ArgumentNullException("effect");

            VertexBuffer = vertexBuffer;
            _vertexInputLayout = VertexInputLayout.FromBuffer(0, vertexBuffer);
            _effect = effect;
            _disposeEffect = disposeEffect;
        }


        /// <summary>
        /// Draws this Mesh. See remarks for difference with XNA.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <remarks>
        /// Unlike XNA, a Mesh is not bound to a specific Effect. The effect must have been setup prior calling this method.
        /// </remarks>
        public void Draw([NotNull] GraphicsDevice graphicsDevice)
        {
            if (_disposed)
                throw new ObjectDisposedException("Mesh");

            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.SetVertexInputLayout(_vertexInputLayout);

            var passCount = Effect.CurrentTechnique.Passes.Count;
            for (var i = 0; i < passCount; i++)
            {
                Effect.CurrentTechnique.Passes[i].Apply();
                graphicsDevice.Draw(PrimitiveType.TriangleList, VertexBuffer.ElementCount);
            }
        }

        /// <summary>
        /// Disposes the VertexBuffer, but not the Effect(!)
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
                return;

            VertexBuffer.Dispose();
            if (_disposeEffect)
                Effect.Dispose();
            _disposed = true;
        }

        /// <summary>Gets or sets the material Effect for this mesh part.</summary>
        public Effect Effect
        {
            get { return _effect; }
        }

        /// <summary>
        /// The vertex buffer range for this mesh part.
        /// </summary>
        [NotNull]
        public Buffer<T> VertexBuffer
        {
            get { return _vertexBuffer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                _vertexBuffer = value;
            }
        }
    }
}