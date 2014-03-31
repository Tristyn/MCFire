using System;
using JetBrains.Annotations;

namespace MCFire.Modules.Nbt.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false), MeansImplicitUse]
    public sealed class NbtAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// Instructs the NbtBuilder to use this member.
        /// </summary>
        public NbtAttribute()
        {

        }

        /// <summary>
        /// Instructs the NbtBuilder to use this member.
        /// </summary>
        /// <param name="tagName">The Nbt tag to use.</param>
        public NbtAttribute(string tagName)
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
