using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MCFire.Modules.Nbt.fNbt;

namespace MCFire.Modules.Nbt.NbtBuilder
{
    /// <summary>
    /// Provides a service to set a objects properties to reflect that of an NbtCompound.
    /// Using this class is very similar to using Json.net, where fields and properties are decorated with [TagNbtAttribute].
    /// </summary>
    public static class NbtBuilder
    {
        static readonly Dictionary<Type, NbtTaggedClass> ClassBuilders = new Dictionary<Type, NbtTaggedClass>();
        static readonly object Lock = new object();

        [NotNull]
        public static T BuildNew<T>([NotNull] NbtCompound compound) where T : class
        {
            return (T)BuildNew(typeof(T), compound);
        }

        [NotNull]
        public static object BuildNew([NotNull] Type type, [NotNull] NbtCompound compound)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (compound == null) throw new ArgumentNullException("compound");

            return GetOrCreateBuilder(type).BuildInstance(compound);
        }

        /// <summary>
        /// Assigns the fields and properties that are attributed with [NbtTag] of an existing object using the compound.
        /// </summary>
        /// <param name="instance">The instance of an object to use.</param>
        /// <param name="compound">The compound to use as the source of values.</param>
        public static void BuildExisting([NotNull] object instance, [NotNull] NbtCompound compound)
        {
            if (instance == null) throw new ArgumentNullException("instance");
            if (compound == null) throw new ArgumentNullException("compound");

            GetOrCreateBuilder(instance.GetType()).BuildExisting(compound, instance);
        }

        [NotNull]
        static NbtTaggedClass GetOrCreateBuilder([NotNull] Type type)
        {
            lock (Lock)
            {
                NbtTaggedClass builder;
                if (ClassBuilders.TryGetValue(type, out builder)) return builder;
                // create new builder
                builder = new NbtTaggedClass(type);
                ClassBuilders.Add(type, builder);
                return builder;
            }
        }
    }
}
