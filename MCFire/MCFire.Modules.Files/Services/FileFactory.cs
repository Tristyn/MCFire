﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Modules.Files.Models;
using MCFire.Modules.Infrastructure;
using File = MCFire.Modules.Files.Models.File;

namespace MCFire.Modules.Files.Services
{
    [Export]
    public sealed class FileFactory
    {
        // TODO: merge this class with the FolderService class
        #region Fields

        [NotNull]
        readonly List<IFormat> _formats;
        [NotNull]
        readonly Dictionary<string, IFormat> _extensionDictionary = new Dictionary<string, IFormat>();

        #endregion

        #region Constructor

        [ImportingConstructor]
        public FileFactory([ImportMany] IEnumerable<IFormat> formats)
        {
            //_unknownFormat = new UnknownFormat(logger, oldFile => DetermineFormat(oldFile.Extension));
            _formats = formats.ToList();

            // resolve extensions
            foreach (var format in _formats)
            {
                foreach (var extension in format.DefaultExtensions.Select(ext => ext.ToLower()))
                {
                    // set extension if it hasn't been set yet
                    if (!_extensionDictionary.ContainsKey(extension))
                    {
                        _extensionDictionary.Add(extension, format);
                        continue;
                    }

                    // resolve extension by hierarchy
                    var setType = _extensionDictionary[extension].GetType();
                    var currentType = format.GetType();

                    // if current format doesn't derive from the set format, continue
                    if (!setType.IsAssignableFrom(currentType)) continue;

                    // current format inherits set format, set extension to current format
                    _extensionDictionary[extension] = format;
                }

                // find base format or null
                var baseType = format.GetType().BaseType;
                var baseFormat = (from baseFormat2 in _formats
                                  where baseFormat2.GetType() == baseType
                                  select baseFormat2).FirstOrDefault();

                // register base format
                if (baseFormat != null)
                    baseFormat.RegisterChildFormat(format);
            }
        }

        #endregion

        #region Methods

        public IFile GetFile(IFolder parent, FileInfo info)
        {
            var format = DetermineFormat(info.Extension);
            if (format != null)
                return format.CreateFile(parent, info);

            return new UnknownFile(parent, info, FindReplacementFile);
        }

        [CanBeNull]
        IFormat DetermineFormat([NotNull] string extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");
            extension = extension.ToLower();
            // recheck extension dictionary for format incase it changed, if it hasn't, use a UI to determine it
            IFormat format;
            return _extensionDictionary.TryGetValue(extension.ToLower(), out format) ? format : null;
        }

        IFile FindReplacementFile(IFile oldFile, FileInfo oldInfo)
        {
            var format = DetermineFormat(oldFile.Extension);
            if (format != null)
                return format.CreateFile(oldFile.Parent, oldInfo);

            // TODO: display dialog here asking which format to use
            throw new NotImplementedException("Asking the user what format to use has not been implemented yet.");
        }

        #endregion
    }
}
