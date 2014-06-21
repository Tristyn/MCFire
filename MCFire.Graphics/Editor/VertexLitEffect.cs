using JetBrains.Annotations;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Editor
{
    public class VertexLitEffect : EffectWrapper
    {
        readonly EffectParameter _transformMatrix;
        public VertexLitEffect([NotNull] Effect effect)
            : base(effect)
        {
            _transformMatrix = effect.Parameters["TransformMatrix"];
        }

        public Matrix TransformMatrix { set { _transformMatrix.SetValue(value); } }
    }
}
