using JetBrains.Annotations;

namespace MCFire.Graphics.Modules.Editor.Models
{
    public class FullColorEffect : EffectWrapper
    {
        readonly EffectParameter _color;
        readonly EffectParameter _transform;

        public FullColorEffect([NotNull] Effect effect)
            : base(effect)
        {
            _transform = effect.Parameters["TransformMatrix"];
            _color = effect.Parameters["Color"];
        }

        public Color4 Color
        {
            set { _color.SetValue(value); }
        }

        public Matrix Transform
        {
            set { _transform.SetValue(value); }
        }
    }
}
