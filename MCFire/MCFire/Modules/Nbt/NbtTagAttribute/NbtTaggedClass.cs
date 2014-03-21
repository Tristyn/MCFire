using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using MCFire.Modules.Nbt.fNbt;

namespace MCFire.Modules.Nbt.NbtTagAttribute
{
    internal class NbtTaggedClass
    {
        readonly Type _type;
        readonly ReadOnlyDictionary<string, NbtTagValueSetter> _tagNameDict;

        public NbtTaggedClass(Type type)
        {
            if(type.GetConstructor(Type.EmptyTypes) == null)
                throw new ArgumentException("The type does not have a parameterless constructor.");

            // get public and private members.
            var members = type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic);

            var tagNameDict = new Dictionary<string, NbtTagValueSetter>();
            foreach (var member in members)
            {
                // if it is field or property, and is tagged
                if ((member.MemberType == MemberTypes.Field
                     || member.MemberType == MemberTypes.Property)
                    && member.GetCustomAttributes(typeof(NbtTagAttribute), true).Any())
                {
                    tagNameDict.Add(member.Name, new NbtTagValueSetter(member));
                }
            }

            _type = type;
            _tagNameDict=new ReadOnlyDictionary<string, NbtTagValueSetter>(tagNameDict);
        }

        public object BuildInstance(NbtCompound compound)
        {
            var instance = Activator.CreateInstance(_type);

            foreach (var tag in compound)
            {
                NbtTagValueSetter setter;
                Debug.Assert(tag.Name != null, "tag.Name != null. Tags whos parents are compounds must have a name, right?");
                if (_tagNameDict.TryGetValue(tag.Name, out setter))
                    setter.SetValue(instance, tag);
            }
            return instance;
        }
    }
}