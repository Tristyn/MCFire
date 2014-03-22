using System;
using System.Collections;
using System.Collections.Generic;
using MCFire.Modules.Nbt.fNbt;

namespace MCFire.Modules.Nbt.NbtTagAttribute
{
    internal static class NbtTagExtensions
    {
        /// <summary>
        /// Returns the value of this tag, cast to its corresponding .NET type.
        /// Returns the tag itself if the tag is a compound.
        /// Returns a list if the TagType is List, note that a list of compounds returns compounds.
        /// Returns null if the TagType is Compound or Unknown.
        /// Possible types that can be returned are byte, short, int, long, float, double, byte[], string, List of object, int[].
        /// This will fail if the tag is a nested NbtList of NbtLists, as nested lists of 
        /// different types are supported in the Nbt spec and not in C#.
        /// </summary>
        public static object ObjectValue(this NbtTag thisTag, Type targetType)
        {
            switch (thisTag.TagType)
            {
                case NbtTagType.Byte:
                    return ((NbtByte)thisTag).Value;
                case NbtTagType.ByteArray:
                    return ((NbtByteArray)thisTag).Value;
                case NbtTagType.Compound:
                    return (NbtBuilder.BuildNew(targetType, (NbtCompound)thisTag));
                case NbtTagType.Double:
                    return ((NbtDouble)thisTag).Value;
                case NbtTagType.Float:
                    return ((NbtFloat)thisTag).Value;
                case NbtTagType.Int:
                    return ((NbtInt)thisTag).Value;
                case NbtTagType.IntArray:
                    return ((NbtIntArray)thisTag).Value;
                case NbtTagType.List:
                    // This is a bit tricky. We cant instantiate a list of an unknown type.
                    // We create a List<> of targetType via reflection, then cast it to IList
                    var nbtList = (NbtList)thisTag;
                    var customListType = typeof(List<>).MakeGenericType(GetTranslatedType(nbtList.ListType));
                    var unknownTypeList = (IList)Activator.CreateInstance(customListType);
                    // using linq here would make us lose the exact generic type of the unknownTypeList
                    foreach (var tag in nbtList)
                    {
                        unknownTypeList.Add(tag);
                    }
                    return unknownTypeList;
                case NbtTagType.Long:
                    return ((NbtLong)thisTag).Value;
                case NbtTagType.Short:
                    return ((NbtShort)thisTag).Value;
                case NbtTagType.String:
                    return ((NbtString)thisTag).Value;
                default:
                    throw new InvalidCastException("Cannot get object from " + NbtTag.GetCanonicalTagName(thisTag.TagType));
            }
        }

        static Type GetTranslatedType(NbtTagType tagType)
        {
            switch (tagType)
            {
                case NbtTagType.Byte:
                    return typeof(byte);
                case NbtTagType.ByteArray:
                    return typeof(byte[]);
                case NbtTagType.Compound:
                    return typeof(NbtCompound);
                case NbtTagType.Double:
                    return typeof(double);
                case NbtTagType.Float:
                    return typeof(float);
                case NbtTagType.Int:
                    return typeof(int);
                case NbtTagType.IntArray:
                    return typeof(int[]);
                case NbtTagType.List:
                    // object generic is a limitation of c#, practically there should never be nested lists
                    return typeof(List<object>);
                case NbtTagType.Long:
                    return typeof(long);
                case NbtTagType.Short:
                    return typeof(short);
                case NbtTagType.String:
                    return typeof(string);
                default:
                    throw new InvalidCastException("Unknown nbt type " + tagType.GetType());
            }
        }
    }
}
