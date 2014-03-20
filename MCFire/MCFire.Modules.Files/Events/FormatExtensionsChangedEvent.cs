﻿using System;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Files.Services;

namespace MCFire.Modules.Files.Events
{
    public class FormatExtensionsChangedEvent<TFile> where TFile : IFile
    {
        public IFormat<TFile> Format { get; set; }

        [CanBeNull]
        public string AddedExtension { get; set; }

        [CanBeNull]
        public string RemovedExtension { get; set; }

        public FormatExtensionsChangedEvent(
            [NotNull] IFormat<TFile> format, 
            [CanBeNull] string addedExtension,
            [CanBeNull] string removedExtension)
        {
            if (format == null) throw new ArgumentNullException("format");
            Format = format;
            AddedExtension = addedExtension;
            RemovedExtension = removedExtension;
        }
    }
}
