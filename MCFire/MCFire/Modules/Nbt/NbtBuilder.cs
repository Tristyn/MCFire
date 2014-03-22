using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MCFire.Modules.Nbt.fNbt;
using MCFire.Modules.Nbt.NbtTagAttribute;

namespace MCFire.Modules.Nbt
{
    /// <summary>
    /// Provides a service to set a objects properties to reflect that of an NbtCompound.
    /// Using this class is very similar to using Json.net, where fields and properties are decorated with [NbtTagAttribute].
    /// </summary>
    public static class NbtBuilder
    {
        static readonly Dictionary<Type, NbtTaggedClass> ClassBuilders = new Dictionary<Type, NbtTaggedClass>();
        static readonly object Lock = new object();

        [NotNull]
        public static T Build<T>([NotNull] NbtCompound compound) where T : class
        {
            return (T)Build(typeof(T), compound);
        }

        [NotNull]
        public static object Build([NotNull] Type type, [NotNull] NbtCompound compound)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (compound == null) throw new ArgumentNullException("compound");

            return GetOrCreateBuilder(type).BuildInstance(compound);
        }

        [NotNull]
        static NbtTaggedClass GetOrCreateBuilder([NotNull] Type type)
        {
            NbtTaggedClass builder;
            lock (Lock)
            {
                if (ClassBuilders.TryGetValue(type, out builder)) return builder;
                // create new builder
                builder = new NbtTaggedClass(type);
                ClassBuilders.Add(type, builder);
            }
            return builder;
        }
    }
}
