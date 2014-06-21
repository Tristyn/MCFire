using System;
using JetBrains.Annotations;
using SharpDX.Toolkit.Graphics;

namespace MCFire.Graphics.Editor
{
    public abstract class EffectWrapper : IDisposable
    {
        // TODO: shouldnt effects load themselves, and those who inherit EffectWrapper have a parameterless constructor
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