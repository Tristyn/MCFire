using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using MCFire.Modules.Nbt.fNbt;

namespace MCFire.Modules.Nbt.NbtTagAttribute
{
    internal class NbtTagValueSetter
    {
        readonly MemberInfo _info;
        readonly NbtTagAttribute _attribute;

        public NbtTagValueSetter(MemberInfo info)
        {
            if (info.MemberType != MemberTypes.Field && info.MemberType != MemberTypes.Property)
                throw new InvalidOperationException("MemberInfo must be of type FieldInfo or PropertyInfo");
            var attribute = (NbtTagAttribute)info.GetCustomAttributes(typeof(NbtTagAttribute), true).First();
            if (attribute == null) throw new InvalidOperationException("MemberInfo must have a NbtTagAttribute");

            _info = info;
            _attribute = attribute;
        }

        /// <summary>
        /// Sets the target of the property or field.
        /// If the types are not compatible, and the target is a List, 
        /// this method will try to build an object using the Lists contents as its parameters.
        /// </summary>
        /// <param name="target">The target object to assign the value to.</param>
        /// <param name="value">The NbtTag used to assign the value.</param>
        public void SetValue([NotNull] object target, [NotNull] NbtTag value)
        {
            // try setting it as a NbtTag
            if (TrySetValue(target, value)) return;

            // try using the ObjectValue
            var objectValue = value.ObjectValue();
            if (TrySetValue(target, objectValue)) return;

            // if its a List<Compound>, build an instance of each entry and assign that
            if (objectValue is IEnumerable<NbtCompound> && GetAssigningType().IsAssignableFrom(typeof(IEnumerable<object>)))
            {
                var builtObjects = (from compound in objectValue as IEnumerable<NbtCompound>
                    select NbtBuilder.Build(GetAssigningType(), compound)).ToList();
                if (TrySetValue(target, builtObjects)) return;
            }

            // Not able to assign any value to the field or property.
            // The default behaviour is to leave it null and print a message.
            // You could potentially throw an exception here.
            Console.WriteLine("Unable to set property {0} on instance of type {1} using {2} due to incompatible types. Assignable types are {3}, {4} and {5}.", 
                _info.Name, target.GetType(), value.TagType, value.GetType().Name, objectValue.GetType().Name, GetAssigningType());
        }

        private bool TrySetValue(object target, object value)
        {
            if (!GetAssigningType().IsInstanceOfType(value))
                return false;

            switch (_info.MemberType)
            {
                case MemberTypes.Property:
                    ((PropertyInfo)_info).SetValue(target, value);
                    break;
                case MemberTypes.Field:
                    ((FieldInfo)_info).SetValue(target, value);
                    break;
                default:
                    throw new InvalidOperationException("MemberInfo does not point to a Field or Property");
            }

            return true;
        }

        /// <summary>
        /// Gets the type of a field or property.
        /// eg a field like: string name; returns typeof(string)
        /// </summary>
        /// <returns></returns>
        private Type GetAssigningType()
        {
            switch (_info.MemberType)
            {
                case MemberTypes.Property:
                    return ((PropertyInfo)_info).PropertyType;
                case MemberTypes.Field:
                    return ((FieldInfo)_info).FieldType;
                default:
                    throw new InvalidOperationException("MemberInfo does not point to a Field or Property");
            }
        }

        public string TagName
        {
            get { return _attribute.TagName; }
        }
    }
}