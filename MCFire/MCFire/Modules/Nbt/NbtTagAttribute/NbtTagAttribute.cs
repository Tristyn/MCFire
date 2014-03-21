using System;

namespace MCFire.Modules.Nbt.NbtTagAttribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Enum, AllowMultiple = false)]
    public sealed class NbtTagAttribute : Attribute
    {
        #region Constructors

        public NbtTagAttribute()
        {

        }

        public NbtTagAttribute(string tagName)
        {
            TagName = tagName;
        }

        #endregion

        #region Properties

        public string TagName { get; set; }

        #endregion
    }
}
