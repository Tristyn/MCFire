using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Test3D.Models
{
    /// <summary>
    /// Represents a batch of geometry information to submit to the graphics device during rendering. 
    /// Each <see cref="ModelMeshPart"/> is a subdivision of a <see cref="ModelMesh"/> object. The <see cref="ModelMesh"/> class is split into multiple <see cref="ModelMeshPart"/> objects, typically based on material information. 
    /// See remarks for differences with XNA.
    /// </summary>
    /// <remarks>
    /// Unlike XNA, a <see cref="ModelMeshPart"/> is not bound to a specific Effect. The effect must have been setup prior calling the <see cref="Draw"/> method on this instance.
    /// The <see cref="Draw"/> method is only responsible to setup the VertexBuffer, IndexBuffer and call the appropriate <see cref="GraphicsDevice.DrawIndexed"/> method on the <see cref="GraphicsDevice"/>.
    /// </remarks>
    public class ModelMeshPart
    {
        public VertexInputLayout VertexInputLayout;

        /// <summary>
        /// The vertex buffer range for this mesh part.
        /// </summary>
        public Buffer<VertexPositionColor> VertexBuffer;

        /// <summary>Gets or sets the material Effect for this mesh part.  Reference page contains code sample.</summary>
        public Effect Effect;

        /// <summary>
        /// Draws this <see cref="ModelMeshPart"/>. See remarks for difference with XNA.
        /// </summary>
        /// <param name="graphicsDevice">The graphics device.</param>
        /// <remarks>
        /// Unlike XNA, a <see cref="ModelMeshPart"/> is not bound to a specific Effect. The effect must have been setup prior calling this method.
        /// This method is only responsible to setup the VertexBuffer, IndexBuffer and call the appropriate <see cref="GraphicsDevice.DrawIndexed"/> method on the <see cref="GraphicsDevice"/>.
        /// </remarks>
        public void Draw(GraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.SetVertexInputLayout(VertexInputLayout);

            var passCount = Effect.CurrentTechnique.Passes.Count;
            for (var j = 0; j < passCount; j++)
            {
                Effect.CurrentTechnique.Passes[j].Apply();
                graphicsDevice.Draw(PrimitiveType.TriangleList, VertexBuffer.ElementCount);
            }
        }
    }
}