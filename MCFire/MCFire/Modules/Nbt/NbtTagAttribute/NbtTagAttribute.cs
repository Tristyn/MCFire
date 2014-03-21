using System;
using JetBrains.Annotations;

namespace MCFire.Modules.Nbt.NbtTagAttribute
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Enum, AllowMultiple = false)]
    public sealed class NbtTagAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Instructs the NbtBuilder to use this member.
        /// </summary>
        public NbtTagAttribute()
        {

        }

        /// <summary>
        /// Instructs the NbtBuilder to use this member.
        /// </summary>
        /// <param name="tagName">The Nbt tag to use.</param>
        public NbtTagAttribute(string tagName)
        {
            TagName = tagName;
        }

        #endregion

        #region Properties

        [CanBeNull]
        public string TagName { get; private set; }

        #endregion
    }
}
