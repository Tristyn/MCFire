namespace MCFire.Core.Modules
{
    /// <summary>
    /// Marks the class that it should be imported by MCFire during preinitialization, 
    /// so that it can do things such as registering itself to the EventAggregator.
    /// This marker interface is commonly inherited by services.
    /// </summary>
    public interface ICreateAtStartup
    {
        // Parts are imported either by a Module in MCFire.Client,
        // or in the bootstrapper of MCFire.Core
        // TODO: rename all mef parts that are consumed as ImportMany to be suffixed with Module (IEditorToolModule, IEditorModule)
        // TODO: considering that, "Modules" namespaces should be collapsed
    }
}
