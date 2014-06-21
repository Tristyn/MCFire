using JetBrains.Annotations;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Editor
{
    public class VertexLitTransparentEffect : VertexLitEffect
    {
        readonly EffectParameter _opacity;

        public VertexLitTransparentEffect([NotNull] Effect effect)
            : base(effect)
        {
            _opacity = effect.Parameters["Opacity"];
        }

        public float Opacity { set { _opacity.SetValue(value); } }
    }
}