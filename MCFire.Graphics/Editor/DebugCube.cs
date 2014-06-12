using System;
using JetBrains.Annotations;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;

namespace MCFire.Graphics.Editor
{
    /// <summary>
    /// An object that can draw a 2x2x2 cube at the position specified.
    /// </summary>
    public class DebugCube : IDisposable
    {
        Mesh<VertexPositionColor> _mesh;
        BasicEffect _effect;
        bool _disposed;

        public DebugCube([NotNull] EditorGame game)
        {
            if (game == null) throw new ArgumentNullException("game");

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
            });
            _mesh = new Mesh<VertexPositionColor>(vertices, _effect, true);
        }

        public void Draw([NotNull] EditorGame game)
        {
            if (game == null) throw new ArgumentNullException("game");
            if (_disposed)
                throw new ObjectDisposedException("DebugCube");

            _effect.World = Matrix.Translation(Position);
            _effect.View = game.Camera.ViewMatrix;
            _effect.Projection = game.Camera.ProjectionMatrix;
            _mesh.Draw(game.GraphicsDevice);
        }

        public void Dispose()
        {
            if (_disposed) return;
            if (_mesh != null)
                _mesh.Dispose();
            _disposed = true;
        }

        public Vector3 Position { get; set; }
    }
}