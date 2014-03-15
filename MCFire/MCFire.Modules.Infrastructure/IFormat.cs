using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;

namespace MCFire.Modules.Infrastructure
{
    public interface IFormat
    {
        #region Fields

        IEnumerable<string> DefaultExtensions { get; }
        IEnumerable<string> Extensions { get; }

        event EventHandler FileCreated;
        event EventHandler FileOpened;
        event EventHandler FileClosing;
        event EventHandler FileClosed;
        event EventHandler ExtensionsChanged;

        #endregion

        #region Methods

        IFile CreateFile([NotNull] IFolder parent, [NotNull] FileInfo info);
        bool TryAddExtension([NotNull] string extension);
        bool TryRemoveExtension([NotNull] string extension);
        void RegisterChildFormat([NotNull] IFormat format);

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