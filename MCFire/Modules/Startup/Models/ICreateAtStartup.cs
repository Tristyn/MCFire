namespace MCFire.Modules.Startup.Models
{
    /// <summary>
    /// Marks the class that it should be imported by MCFire during preinitialization, 
    /// so that it can do things such as registering itself to the EventAggregator.
    /// This marker interface is commonly inherited by services.
    /// 
    /// </summary>
    public interface ICreateAtStartup
    {
    }
}
