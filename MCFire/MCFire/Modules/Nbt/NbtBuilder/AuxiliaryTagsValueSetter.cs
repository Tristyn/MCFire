using System;
using System.Collections.Generic;
using System.Reflection;
using MCFire.Modules.Nbt.fNbt;

namespace MCFire.Modules.Nbt.NbtBuilder
{
    internal class AuxiliaryTagsValueSetter : ValueSetterBase
    {
        public AuxiliaryTagsValueSetter(MemberInfo info)
            : base(info)
        {
            if (!AssigningType.IsAssignableFrom(typeof(List<NbtTag>)))
                throw new InvalidOperationException("Members marked with [AuxiliaryTags] must be a List of NbtTag");
        }

        public void AddTag(object instance, NbtTag tag)
        {
            var tags = (List<NbtTag>)GetValue(instance);
            if (tags == null)
            {
                tags = new List<NbtTag>();
                TrySetValue(instance, tags);
            }
            tags.Add(tag);
        }
    }
}
