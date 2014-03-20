using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Services
{
    public interface IFormat
    {
        #region Fields

        IEnumerable<string> DefaultExtensions { get; }
        IEnumerable<string> Extensions { get; }

        #endregion

        #region Methods

        IFile CreateFile([NotNull] IFolder parent, [NotNull] FileInfo info);
        bool TryAddExtension([NotNull] string extension);
        bool TryRemoveExtension([NotNull] string extension);

        #endregion
    }

    public interface IFormat<out TFile> : IFormat
        where TFile : IFile
    {
        #region Methods

        new TFile CreateFile([NotNull] IFolder parent, [NotNull] FileInfo info);

        #endregion
    }
}