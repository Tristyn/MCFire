namespace MCFire.Modules.TextEditor.Services
{
    //[Export(typeof(IFormat))]
    //[Export(typeof(IFormat<TextFile>))]
    //class TextFormat : Format, IFormat<TextFile>
    //{
    //    [ImportingConstructor]
    //    public TextFormat(IEventAggregator aggregator) : base(aggregator){}

    //    IFile IFormat.CreateFile(IFolder parent, FileInfo info)
    //    {
    //        return CreateFile(parent, info);
    //    }

    //    public new TextFile CreateFile(IFolder parent, FileInfo info)
    //    {
    //        if (parent == null) throw new ArgumentNullException("parent");
    //        if (info == null) throw new ArgumentNullException("info");

    //        if (!String.Equals(parent.Path, info.DirectoryName, StringComparison.CurrentCultureIgnoreCase))
    //            throw new ArgumentException("parent.Path must equal info.DirectoryInfo");

    //        var file = new TextFile(parent, info);
    //        Aggregator.Publish(new FileCreatedMessage<TextFile>(file));
    //        return file;
    //    }

    //    IEnumerable<string> IFormat.DefaultExtensions { get { yield return ".txt"; } }
    //}
}
