using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace MCFire.Graphics.Modules.BoxSelector.Models
{
    class TiledBoxVisual : IDrawable, ILoadContent
    {
        Texture2D _texture;
        [NotNull]
        Mesh<VertexPositionTexture> _topQuad;
        [NotNull]
        Mesh<VertexPositionTexture> _bottomQuad;
        [NotNull]
        Mesh<VertexPositionTexture> _rightQuad;
        [NotNull]
        Mesh<VertexPositionTexture> _leftQuad;
        [NotNull]
        Mesh<VertexPositionTexture> _forwardQuad;
        [NotNull]
        Mesh<VertexPositionTexture> _backwardQuad;
        [NotNull]
        BoxSelectEffect _effect;

        readonly float _textureScaling;
        readonly float _zBias;

        public TiledBoxVisual([NotNull] Texture2D texture, float textureScaling = 1, float zBias = 0)
        {
            _texture = texture;
            _textureScaling=textureScaling;
            _zBias = zBias;
        }
        public void LoadContent(EditorGame game)
        {

            var effect = new BoxSelectEffect(game.LoadContent<Effect>("BoxSelect"))
            {
                Sampler = game.GraphicsDevice.SamplerStates.PointWrap,
                Main = _texture
            };
            _effect = effect;
            _texture = game.LoadContent<Texture2D>("Grid");

            foreach (var face in FacesUtils.AllFaces)
            {
                var mesh = new Mesh<VertexPositionTexture>(Buffer.Vertex.New(game.GraphicsDevice, CreateQuadUv(face)), effect.Effect);
                SetQuad(face, mesh);
            }
        }

        public void Draw(EditorGame game)
        {
            game.GraphicsDevice.SetBlendState(game.GraphicsDevice.BlendStates.AlphaBlend);
            var viewProj = game.Camera.ViewMatrix * game.Camera.ProjectionMatrix;

            // CullNone, if the camera is inside the selection.
            var cullNone = Cuboid.Within((Point3)game.Camera.Position);
            if (cullNone)
                game.GraphicsDevice.SetRasterizerState(game.GraphicsDevice.RasterizerStates.CullNone);

            foreach (var face in FacesUtils.AllFaces)
            {
                var quad = GetQuad(face);
                var leftFace = face.GetLeftVisualFace();
                var topFace = face.GetTopVisualFace();
                var transform = new Vector4(
                    Cuboid.GetLengthComponent(leftFace),
                    Cuboid.GetLengthComponent(topFace),
                    Cuboid.GetPositionComponent(leftFace),
                    Cuboid.GetPositionComponent(topFace));

                // TODO: texture scaling on the bottom face is fudged

                // invert in certain edge cases
                if (face.HasSideFaces())
                {
                    transform.Y = -transform.Y;
                    if (face == Faces.Right)
                    {
                        transform.X = -transform.X;
                    }
                    else if (face == Faces.Forward)
                    {
                        transform.X = -transform.X;
                    }
                }

                switch (face)
                {
                    case Faces.Left:
                        _effect.TransformMatrix = Matrix.Scaling(0, Cuboid.Height, Cuboid.Width) *
                                                  Matrix.Translation(Cuboid.Left-_zBias, Cuboid.Bottom, Cuboid.Forward) *
                                                  viewProj;
                        break;
                    case Faces.Bottom:
                        _effect.TransformMatrix = Matrix.Scaling(Cuboid.Length, 0, Cuboid.Width) *
                                                  Matrix.Translation(Cuboid.Left, Cuboid.Bottom-_zBias, Cuboid.Forward) *
                                                  viewProj;
                        break;
                    case Faces.Forward:
                        _effect.TransformMatrix = Matrix.Scaling(Cuboid.Length, Cuboid.Height, 0) *
                                                  Matrix.Translation(Cuboid.Left, Cuboid.Bottom, Cuboid.Forward-_zBias) *
                                                  viewProj;
                        break;
                    case Faces.Right:
                        _effect.TransformMatrix = Matrix.Scaling(0, Cuboid.Height, Cuboid.Width) *
                                                  Matrix.Translation(Cuboid.Left+Cuboid.Length+_zBias, Cuboid.Bottom, Cuboid.Forward) *
                                                  viewProj;
                        break;
                    case Faces.Top:
                        _effect.TransformMatrix = Matrix.Scaling(Cuboid.Length, 0, Cuboid.Width) *
                                                  Matrix.Translation(Cuboid.Left, Cuboid.Bottom+Cuboid.Height+_zBias, Cuboid.Forward) *
                                                  viewProj;
                        break;
                    case Faces.Backward:
                        _effect.TransformMatrix = Matrix.Scaling(Cuboid.Length, Cuboid.Height, 0) *
                                                  Matrix.Translation(Cuboid.Left, Cuboid.Bottom, Cuboid.Forward+Cuboid.Width+_zBias) *
                                                  viewProj;
                        break;
                    default:
                        Debug.Assert(false);
                        break;
                }

                _effect.MainTransform = transform*_textureScaling; 

                quad.Draw(game.GraphicsDevice);
            }

            // reset to default
            if (cullNone)
                game.GraphicsDevice.SetRasterizerState(game.GraphicsDevice.RasterizerStates.CullBack);
        }

        public void UnloadContent(EditorGame game)
        {
        }

        public void Dispose()
        {
            EnumerateFaces().ForEach(quad => quad.Dispose());
        }

        public void HighlightFace(Vector3 normal)
        {
            // TODO:
        }

        IEnumerable<Mesh<VertexPositionTexture>> EnumerateFaces()
        {
            yield return _leftQuad;
            yield return _bottomQuad;
            yield return _forwardQuad;
            yield return _rightQuad;
            yield return _topQuad;
            yield return _backwardQuad;
        }

        [NotNull]
        Mesh<VertexPositionTexture> GetQuad(Faces face)
        {
            switch (face)
            {
                case Faces.Left:
                    return _leftQuad;
                case Faces.Bottom:
                    return _bottomQuad;
                case Faces.Forward:
                    return _forwardQuad;
                case Faces.Right:
                    return _rightQuad;
                case Faces.Top:
                    return _topQuad;
                case Faces.Backward:
                    return _backwardQuad;

                default:
                    throw new ArgumentOutOfRangeException("face");
            }
        }

        void SetQuad(Faces face, [NotNull] Mesh<VertexPositionTexture> mesh)
        {
            if (mesh == null) throw new ArgumentNullException("mesh");

            switch (face)
            {
                case Faces.Left:
                    _leftQuad = mesh;
                    return;
                case Faces.Bottom:
                    _bottomQuad = mesh;
                    return;
                case Faces.Forward:
                    _forwardQuad = mesh;
                    return;
                case Faces.Right:
                    _rightQuad = mesh;
                    return;
                case Faces.Top:
                    _topQuad = mesh;
                    return;
                case Faces.Backward:
                    _backwardQuad = mesh;
                    return;

                default:
                    throw new ArgumentOutOfRangeException("face");
            }
        }

        static VertexPositionTexture[] CreateQuadUv(Faces face)
        {
            var quad = GeometricPrimitives.GetQuad(face);
            var mesh = new VertexPositionTexture[quad.Length];
            for (var i = 0; i < quad.Length; i++)
            {
                mesh[i] = new VertexPositionTexture(quad[i], GeometricPrimitives.QuadUv[i]);
            }
            return mesh;
        }

        public Cuboid Cuboid { get; set; }
    }
}
