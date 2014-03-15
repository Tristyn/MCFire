﻿using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MCFire.Modules.Infrastructure
{
    public interface IFolderItem
    {
        #region Methods
        Task<bool> Cut();
        Task<bool> Copy();
        Task<bool> Delete();
        Task<bool> Rename([NotNull] string name);
        Task Refresh();
        #endregion

        #region Properties

        bool Exists { get; }
        [NotNull]
        string Name { get; set; }
        [NotNull]
        string Path { get; }

        #endregion
    }
}