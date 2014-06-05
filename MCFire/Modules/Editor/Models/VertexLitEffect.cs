using System;
using JetBrains.Annotations;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Modules.Editor.Models
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
    // TODO: shouldnt effects load themselves, and those who inherit EffectWrapper have a parameterless constructor
    public abstract class EffectWrapper : IDisposable
    {
        public readonly Effect Effect;

        protected EffectWrapper([NotNull] Effect effect)
        {
            if (effect == null) throw new ArgumentNullException("effect");

            Effect = effect;
        }

        public void Dispose()
        {
            Effect.Dispose();
        }
    }
}
