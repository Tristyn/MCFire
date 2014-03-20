using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;

namespace MCFire.Modules.Files.Services
{
    public interface IFormat
    {
        #region Methods

        [NotNull]
        IFile CreateFile([NotNull] IFolder parent, [NotNull] FileInfo info);
        bool TryAddExtension([NotNull] string extension);
        bool TryRemoveExtension([NotNull] string extension);

        #endregion

        #region Properties

        [NotNull]
        IEnumerable<string> DefaultExtensions { get; }

        [NotNull]
        IEnumerable<string> Extensions { get; }

        #endregion
    }

    public interface IFormat<out TFile> : IFormat
        where TFile : IFile
    {
        #region Methods

        [NotNull]
        new TFile CreateFile([NotNull] IFolder parent, [NotNull] FileInfo info);

        #endregion
    }
}