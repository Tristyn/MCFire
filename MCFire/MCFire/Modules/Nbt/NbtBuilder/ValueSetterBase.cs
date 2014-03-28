using System;
using System.Reflection;

namespace MCFire.Modules.Nbt.NbtBuilder
{
    internal abstract class ValueSetterBase
    {
        protected readonly Type AssigningType;
        protected readonly MemberInfo Info;

        protected ValueSetterBase(MemberInfo info)
        {
            if (info.MemberType != MemberTypes.Field && info.MemberType != MemberTypes.Property)
                throw new InvalidOperationException("MemberInfo must be of type FieldInfo or PropertyInfo.");
            if (info.MemberType == MemberTypes.Property && !((PropertyInfo)info).CanWrite)
                throw new InvalidOperationException("Properties decorated with [NbtTag] must have a setter of any accessibility.");
            // readonly fields are OK

            Info = info;
            switch (Info.MemberType)
            {
                case MemberTypes.Property:
                    AssigningType = ((PropertyInfo)Info).PropertyType;
                    break;
                case MemberTypes.Field:
                    AssigningType = ((FieldInfo)Info).FieldType;
                    break;
                default:
                    throw new InvalidOperationException("MemberInfo does not point to a Field or Property");
            }
        }

        protected bool TrySetValue(object target, object value)
        {
            if (!AssigningType.IsInstanceOfType(value))
                return false;

            switch (Info.MemberType)
            {
                case MemberTypes.Property:
                    ((PropertyInfo)Info).SetValue(target, value);
                    break;
                case MemberTypes.Field:
                    ((FieldInfo)Info).SetValue(target, value);
                    break;
                default:
                    throw new InvalidOperationException("MemberInfo does not point to a Field or Property");
            }

            return true;
        }

        protected object GetValue(object target)
        {
            switch (Info.MemberType)
            {
                case MemberTypes.Property:
                    ((PropertyInfo)Info).GetValue(target);
                    break;
                case MemberTypes.Field:
                    ((FieldInfo)Info).GetValue(target);
                    break;
                default:
                    throw new InvalidOperationException("MemberInfo does not point to a Field or Property");
            }

            return true;
        }
    }
}