namespace MCFire.Modules.Files.Services
{
    //[Export(typeof(IFormat))]
    //public class Format : IFormat<File>
    //{
    //    #region Fields

    //    readonly List<string> _extensions = new List<string>();
    //    protected readonly IEventAggregator Aggregator;
    //    readonly object _lock = new object();

    //    #endregion

    //    #region Constructor

    //    [ImportingConstructor]
    //    public Format(IEventAggregator aggregator)
    //    {
    //        Aggregator = aggregator;
    //        // ReSharper disable once DoNotCallOverridableMethodsInConstructor
    //        _extensions = DefaultExtensions.ToList();
    //        // TODO: the extensions should be remembered and set from settings, if it doesn't exist, run that LOC
    //    }

    //    #endregion

    //    #region Methods

    //    public IFile CreateFile(IFolder parent, FileInfo info)
    //    {
    //        if (parent == null) throw new ArgumentNullException("parent");
    //        if (info == null) throw new ArgumentNullException("info");

    //        if (!String.Equals(parent.Path, info.DirectoryName, StringComparison.CurrentCultureIgnoreCase))
    //            throw new ArgumentException("parent.Path must equal info.DirectoryInfo");

    //        var file = new File(parent, info);
    //        Aggregator.Publish(new FileCreatedMessage<File>(file));
    //        return file;
    //    }

    //    File IFormat<File>.CreateFile(IFolder parent, FileInfo info)
    //    {
    //        // ReSharper disable once AssignNullToNotNullAttribute
    //        return CreateFile(parent, info) as File;
    //    }

    //    public virtual bool TryAddExtension(string extension)
    //    {
    //        if (extension == null) throw new ArgumentNullException("extension");

    //        lock (_lock)
    //        {
    //            if (_extensions.Contains(extension))
    //                return false;
    //            _extensions.Add(extension);
    //        }
    //        Aggregator.Publish(new FormatExtensionsChangedMessage<File>(this, extension, null));
    //        return true;
    //    }

    //    public virtual bool TryRemoveExtension(string extension)
    //    {
    //        if (extension == null) throw new ArgumentNullException("extension");
    //        lock (_lock)
    //        {
    //            if (!_extensions.Remove(extension)) return false;
    //        }

    //        Aggregator.Publish(new FormatExtensionsChangedMessage<File>(this, null, extension));
    //        return true;
    //    }

    //    #endregion

    //    #region Properties

    //    public IEnumerable<string> Extensions
    //    {
    //        get
    //        {
    //            lock (_lock)
    //            {
    //                return new List<string>(_extensions);
    //            }
    //        }
    //    }

    //    public IEnumerable<string> DefaultExtensions
    //    {
    //        get { yield break; }
    //    }

    //    #endregion
    //}
}
