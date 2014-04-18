using System;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Editor.Models
{
    public class VisualChunk : IDisposable
    {
        VertexLitEffect _vertexLitEffect;
        readonly int _cX;
        readonly int _cY;
        Mesh _mesh;
        bool _disposed;

        public VisualChunk(Buffer<VertexPositionColor> vertexBuffer, VertexLitEffect vertexLitEffect,int cX,int cY)
        {
            _vertexLitEffect = vertexLitEffect;
            _cX = cX;
            _cY = cY;
            _mesh = new Mesh
            {
                VertexBuffer = vertexBuffer,
                VertexInputLayout = VertexInputLayout.FromBuffer(0, vertexBuffer),
                Effect = _vertexLitEffect.Effect
            };
        }

        public void Draw(EditorGame game)
        {
            if (_disposed)
                throw new ObjectDisposedException("VisualChunk");

            _vertexLitEffect.TransformMatrix = Matrix.Translation(_cX * 16, 0, _cY * 16) * game.Camera.ViewMatrix * game.Camera.ProjectionMatrix;
            _mesh.Draw(game.GraphicsDevice);
        }

        public void Dispose()
        {
            _vertexLitEffect = null;
            _mesh.Dispose();
            _mesh = null;
            _disposed = true;
        }
    }
}
