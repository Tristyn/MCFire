using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using MCFire.Modules.Editor.Models;
using MCFire.Modules.Infrastructure.Models;
using MCFire.Modules.Meshalyzer.Models;
using MoreLinq;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Buffer = SharpDX.Toolkit.Graphics.Buffer;

namespace MCFire.Modules.BoxSelector.Models
{
    class BoxVisual : IDrawable, ILoadContent
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

        public BoxVisual([NotNull] Texture2D texture)
        {
            _texture = texture;
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

            // TODO: texture allignment (up is good)
            //// up
            //_effect.TransformMatrix = Matrix.Scaling(Cuboid.XLength, 0, Cuboid.ZLength) *
            //                          Matrix.Translation(Cuboid.Left, Cuboid.Top + 1, Cuboid.Forward) *
            //                          viewProj;
            //_effect.MainTransform = new Vector4(Cuboid.XLength, Cuboid.ZLength, Cuboid.Left, Cuboid.Forward) / 16;
            //_topQuad.Draw(game.GraphicsDevice);

            //// down
            //_effect.TransformMatrix = Matrix.Scaling(Cuboid.XLength, 0, Cuboid.ZLength) *
            //                          Matrix.Translation(Cuboid.Left, Cuboid.Bottom, Cuboid.Forward) *
            //                          viewProj;
            //_effect.MainTransform = new Vector4(Cuboid.ZLength, Cuboid.XLength, Cuboid.Left, Cuboid.Forward) / 16;
            //_bottomQuad.Draw(game.GraphicsDevice);

            //// right
            //_effect.TransformMatrix = Matrix.Scaling(0, Cuboid.YLength, Cuboid.ZLength) *
            //                          Matrix.Translation(Cuboid.Right + 1, Cuboid.Bottom, Cuboid.Forward) *
            //                          viewProj;
            //_effect.MainTransform = new Vector4(Cuboid.ZLength, Cuboid.YLength, Cuboid.Forward, Cuboid.Bottom) / 16;
            //_rightQuad.Draw(game.GraphicsDevice);

            //// left
            //_effect.TransformMatrix = Matrix.Scaling(0, Cuboid.YLength, Cuboid.ZLength) *
            //                          Matrix.Translation(Cuboid.Left, Cuboid.Bottom, Cuboid.Forward) *
            //                          viewProj;
            //_effect.MainTransform = new Vector4(Cuboid.ZLength, Cuboid.YLength, Cuboid.Forward, Cuboid.Bottom) / 16;
            //_leftQuad.Draw(game.GraphicsDevice);

            //// back
            //_effect.TransformMatrix = Matrix.Scaling(Cuboid.XLength, Cuboid.YLength, 0) *
            //                          Matrix.Translation(Cuboid.Left, Cuboid.Bottom, Cuboid.Backward + 1) *
            //                          viewProj;
            //_effect.MainTransform = new Vector4(Cuboid.XLength, Cuboid.YLength, Cuboid.Left, Cuboid.Bottom) / 16;
            //_backwardQuad.Draw(game.GraphicsDevice);
            //// forward
            //_effect.TransformMatrix = Matrix.Scaling(Cuboid.XLength, Cuboid.YLength, 0) *
            //                          Matrix.Translation(Cuboid.Left, Cuboid.Bottom, Cuboid.Forward) *
            //                          viewProj;
            //_effect.MainTransform = new Vector4(Cuboid.XLength, Cuboid.YLength, Cuboid.Left, Cuboid.Bottom) / 16;
            //_forwardQuad.Draw(game.GraphicsDevice);

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
                if (face==Faces.Right)
                {
                    transform.X = -transform.X;
                    transform.Y = -transform.Y;
                }
                else if (face == Faces.Forward)
                {
                    transform.X = -transform.X;
                    transform.Y = -transform.Y;
                }
                Matrix translation;

                switch (face)
                {
                    case Faces.Left:
                        _effect.TransformMatrix = Matrix.Scaling(0, Cuboid.Height, Cuboid.Width) *
                                                  Matrix.Translation(Cuboid.Left, Cuboid.Bottom, Cuboid.Forward) *
                                                  viewProj;
                        break;
                    case Faces.Bottom:
                        _effect.TransformMatrix = Matrix.Scaling(Cuboid.Length, 0, Cuboid.Width) *
                                                  Matrix.Translation(Cuboid.Left, Cuboid.Bottom, Cuboid.Forward) *
                                                  viewProj;
                        break;
                    case Faces.Forward:
                        _effect.TransformMatrix = Matrix.Scaling(Cuboid.Length, Cuboid.Height, 0) *
                                                  Matrix.Translation(Cuboid.Left, Cuboid.Bottom, Cuboid.Forward) *
                                                  viewProj;
                        break;
                    case Faces.Right:
                        _effect.TransformMatrix = Matrix.Scaling(0, Cuboid.Height, Cuboid.Width) *
                                                  Matrix.Translation(Cuboid.Left+Cuboid.Length, Cuboid.Bottom, Cuboid.Forward) *
                                                  viewProj;
                        break;
                    case Faces.Top:
                        _effect.TransformMatrix = Matrix.Scaling(Cuboid.Length, 0, Cuboid.Width) *
                                                  Matrix.Translation(Cuboid.Left, Cuboid.Bottom+Cuboid.Height, Cuboid.Forward) *
                                                  viewProj;
                        break;
                    case Faces.Backward:
                        _effect.TransformMatrix = Matrix.Scaling(Cuboid.Length, Cuboid.Height, 0) *
                                                  Matrix.Translation(Cuboid.Left, Cuboid.Bottom, Cuboid.Forward+Cuboid.Width) *
                                                  viewProj;
                        break;
                    default:
                        Debug.Fail("one of FaceUtils.AllFaces isnt set");
                        _effect.TransformMatrix = new Matrix();
                        break;
                }

                _effect.MainTransform = transform/16; 

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
