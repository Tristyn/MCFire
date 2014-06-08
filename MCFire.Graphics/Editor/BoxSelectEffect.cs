using JetBrains.Annotations;

namespace MCFire.Graphics.Modules.Editor.Models
{
    public class BoxSelectEffect : EffectWrapper
    {
        readonly EffectParameter _transformMatrix;
        readonly EffectParameter _main;
        readonly EffectParameter _mainShift;
        readonly EffectParameter _mainScale;
        readonly EffectParameter _sampler;
        readonly EffectParameter _mainTransform;

        public BoxSelectEffect([NotNull] Effect effect)
            : base(effect)
        {
            _transformMatrix = effect.Parameters["TransformMatrix"];
            _main = effect.Parameters["Main"];
            _mainTransform = effect.Parameters["MainTransform"];
            //_mainShift = effect.Parameters["MainShift"];
            //_mainScale = effect.Parameters["MainScale"];
            _sampler = effect.Parameters["Sampler"];
        }

        public Matrix TransformMatrix
        {
            set { _transformMatrix.SetValue(value); }
        }

        public Texture2D Main
        {
            set { _main.SetResource(value); }
        }

        public Vector4 MainTransform
        {
            set { _mainTransform.SetValue(value); }
        }

        public Vector2 MainShift
        {
            set { _mainShift.SetValue(value); }
        }

        public Vector2 MainScale
        {
            set { _mainScale.SetValue(value); }
        }

        public SamplerState Sampler
        {
            set { _sampler.SetResource(value); }
        }
    }
}
