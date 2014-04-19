using System;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;

#if DEBUG
namespace MCFire.Modules.Editor.Models
{
    public class DebugCube : IDisposable
    {
        Mesh _mesh;
        BasicEffect _effect;
        bool _disposed;

        public DebugCube(EditorGame game)
        {
            var vertices = game.ToDisposeContent(Buffer.Vertex.New(
                game.GraphicsDevice,
                new[]
                {
                    new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.Orange), // Front
                    new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.Orange),
                    new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange),
                    new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.Orange),
                    new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.Orange),
                    new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.Orange),
                    new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.Orange), // BACK
                    new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.Orange),
                    new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.Orange),
                    new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.Orange),
                    new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.Orange),
                    new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.Orange),
                    new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.OrangeRed), // Top
                    new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.OrangeRed),
                    new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.OrangeRed),
                    new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.OrangeRed),
                    new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.OrangeRed),
                    new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.OrangeRed),
                    new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.OrangeRed), // Bottom
                    new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.OrangeRed),
                    new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.OrangeRed),
                    new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.OrangeRed),
                    new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.OrangeRed),
                    new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.OrangeRed),
                    new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.DarkOrange), // Left
                    new VertexPositionColor(new Vector3(-1.0f, -1.0f, 1.0f), Color.DarkOrange),
                    new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.DarkOrange),
                    new VertexPositionColor(new Vector3(-1.0f, 1.0f, -1.0f), Color.DarkOrange),
                    new VertexPositionColor(new Vector3(-1.0f, 1.0f, 1.0f), Color.DarkOrange),
                    new VertexPositionColor(new Vector3(-1.0f, -1.0f, -1.0f), Color.DarkOrange),
                    new VertexPositionColor(new Vector3(1.0f, -1.0f, 1.0f), Color.DarkOrange), // Right
                    new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.DarkOrange),
                    new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.DarkOrange),
                    new VertexPositionColor(new Vector3(1.0f, 1.0f, 1.0f), Color.DarkOrange),
                    new VertexPositionColor(new Vector3(1.0f, 1.0f, -1.0f), Color.DarkOrange),
                    new VertexPositionColor(new Vector3(1.0f, -1.0f, -1.0f), Color.DarkOrange)
                }));

            _effect = game.ToDisposeContent(new BasicEffect(game.GraphicsDevice)
            {
                VertexColorEnabled = true,
                World = Matrix.Identity
            });
            _mesh = new Mesh
            {
                Effect = game.ToDisposeContent(new BasicEffect(game.GraphicsDevice)
                {
                    VertexColorEnabled = true,
                    World = Matrix.Identity
                }),
                VertexBuffer = vertices,
                VertexInputLayout = VertexInputLayout.FromBuffer(0, vertices)
            };
        }

        public void Draw(EditorGame game)
        {
            if (_disposed)
                throw new ObjectDisposedException("DebugCube");

            _effect.View = game.Camera.ViewMatrix;
            _effect.Projection = game.Camera.ProjectionMatrix;
            _mesh.Draw(game.GraphicsDevice);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _mesh.Dispose();
            _mesh = null;
            _effect.Dispose();
            _effect = null;
            _disposed = true;
        }
    }
}
#endif