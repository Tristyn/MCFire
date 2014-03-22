using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MCFire.Modules.Nbt.fNbt;

namespace MCFire.Modules.Nbt.NbtTagAttribute
{
    internal class NbtTaggedClass
    {
        public readonly Type Type;
        readonly ReadOnlyDictionary<string, NbtTagValueSetter> _tagNameDict;

        public NbtTaggedClass(Type type)
        {
            // get public and private members.
            var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public);

            var tagNameDict = new Dictionary<string, NbtTagValueSetter>();
            foreach (var member in members)
            {
                var tag = member.GetCustomAttribute<NbtAttribute>(true);
                if(tag==null) continue;

                // TODO: you can access private setters, but they are hidden from the search if they are inherited, but you can get them with recursion. ban all private setters.
                if(member.MemberType==MemberTypes.Property && !((PropertyInfo)member).SetMethod.IsPublic) 
                    throw new InvalidOperationException("Property does not have setter.");

                tagNameDict.Add(tag.TagName ?? member.Name, new NbtTagValueSetter(member));
            }

            Type = type;
            _tagNameDict = new ReadOnlyDictionary<string, NbtTagValueSetter>(tagNameDict);
        }

        public object BuildInstance(NbtCompound compound)
        {
            if (Type.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException(
                    "The type does not have a parameterless constructor and therefor a new instance cant be created with Activator.");

            var instance = Activator.CreateInstance(Type);
            BuildExisting(compound, instance);
            return instance;
        }

        /// <summary>
        /// Uses an existing instance to assign values to.
        /// </summary>
        /// <param name="compound">The compound to use as a data source.</param>
        /// <param name="instance">The instance of an object. Its type must match NbtTaggedClass.type</param>
        public void BuildExisting([NotNull] NbtCompound compound, [NotNull] object instance)
        {
            if (compound.Name == "" && compound.Any() && compound.First().TagType == NbtTagType.Compound)
                compound = (NbtCompound)compound.First();
            foreach (var tag in compound)
            {
                NbtTagValueSetter setter;
                Debug.Assert(tag.Name != null, "tag.Name != null. Tags whos parents are compounds must have a name, right?");
                // ReSharper disable once AssignNullToNotNullAttribute - tags whos parents are compounds always have a name.
                if (_tagNameDict.TryGetValue(tag.Name, out setter))
                    setter.SetValue(instance, tag);
            }
        }
    }
}