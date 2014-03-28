using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MCFire.Modules.Nbt.Attributes;
using MCFire.Modules.Nbt.fNbt;

namespace MCFire.Modules.Nbt.NbtBuilder
{
    internal class NbtTagValueSetter : ValueSetterBase
    {
        readonly NbtAttribute _attribute;

        public NbtTagValueSetter(MemberInfo info)
            : base(info)
        {
            _attribute = (NbtAttribute)info.GetCustomAttributes(typeof(NbtAttribute), true).FirstOrDefault();
            if (_attribute == null) throw new InvalidOperationException("MemberInfo must have a TagNbtAttribute");
        }

        /// <summary>
        /// Sets the target of the property or field.
        /// If the types are not compatible, and the target is a List, 
        /// this method will try to build an object using the Lists contents as its parameters.
        /// </summary>
        /// <param name="target">The target object to assign the value to.</param>
        /// <param name="value">The NbtTag used to assign the value.</param>
        /// <returns>If the value could be set.</returns>
        public bool SetValue([NotNull] object target, [NotNull] NbtTag value)
        {
            // try setting it as a NbtTag
            if (TrySetValue(target, value)) return true;

            // try using the ObjectValue
            var objectValue = value.ObjectValue(AssigningType);
            if (TrySetValue(target, objectValue)) return true;

            // if its a List<Compound>, build an instance of each entry and assign that
            if (objectValue is IEnumerable<NbtCompound> && AssigningType.IsAssignableFrom(typeof(IEnumerable<object>)))
            {
                var builtObjects = (from compound in objectValue as IEnumerable<NbtCompound>
                                    select NbtBuilder.BuildNew(AssigningType, compound)).ToList();
                if (TrySetValue(target, builtObjects)) return true;
            }

            // TODO: if objectValue is byte, cast to bool
            // TODO: try casting to enums

            // Not able to assign any value to the field or property.
            // The default behaviour is to leave it null and print a message, 
            // NbtTaggedClass will add the NbtTag to the AuxiliaryTags field.
            // You could potentially throw an exception here.

            //Console.WriteLine("Unable to set property {0} on instance of type {1} using {2} due to incompatible types. Assignable types are {3} and {4}.",
            //    Info.Name, target.GetType(), value.TagType, value.GetType().Name, objectValue.GetType().Name);

            return false;
        }

        public string TagName
        {
            get { return _attribute.TagName; }
        }
    }
}