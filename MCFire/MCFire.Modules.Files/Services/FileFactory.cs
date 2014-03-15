using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Modules.Infrastructure;

namespace MCFire.Modules.Files.Services
{
    [Export]
    public sealed class FileFactory
    {
        // TODO: merge this class with the FileManager class
        #region Fields

        [NotNull]
        readonly List<IFormat> _formats;
        [NotNull]
        readonly Dictionary<string, IFormat> _extensionDictionary = new Dictionary<string, IFormat>();
        [NotNull]
        readonly UnknownFormat _unknownFormat;

        #endregion

        #region Constructor

        [ImportingConstructor]
        public FileFactory([ImportMany] IEnumerable<IFormat> formats)
        {
            //_unknownFormat = new UnknownFormat(logger, file => DetermineFormat(file.Extension));
            _formats = formats.ToList();

            // resolve extensions
            foreach (var format in _formats)
            {
                foreach (var extension in format.DefaultExtensions)
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
                    // override extension if current format derives from the set format
                    if (setType.IsAssignableFrom(currentType))
                    {
                        _extensionDictionary[extension] = format;
                        continue;
                    }

                    // continue if set format derives from current format
                    if (currentType.IsAssignableFrom(setType)) continue;

                    // if formats are from different hierarchies
                    Console.WriteLine("FileManager - Detected contention over {0} extension between {1} and {2}. Prioritizing {1} format.",
                        extension, setType, currentType);
                    // TODO: the desired action here is that the original format is selected, because mod assemblies are prioritized.
                    // TODO: do prioritized assemblies also prioritize their exports in an [ImportMany] situation?
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

        private IFormat DetermineFormat([NotNull] string extension)
        {
            if (extension == null) throw new ArgumentNullException("extension");

            // recheck extension dictionary for format incase it changed, if it hasn't, use a UI to determine it
            IFormat format;
            return _extensionDictionary.TryGetValue(extension, out format) ? format : _unknownFormat;
        }

        #endregion
    }
}
