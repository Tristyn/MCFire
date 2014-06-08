using System.Collections.Generic;
using System.ComponentModel.Composition;
using MCFire.Core.Modules.Startup.Models;

namespace MCFire.Core.Modules.Startup
{
    [Export(typeof(IModule))]
    class Module : ModuleBase
    {
        // create all startup objects
        // ReSharper disable once UnusedMember.Local
        // ReSharper disable once ValueParameterNotUsed
        [ImportMany]
        IEnumerable<ICreateAtStartup> StartupObjects { set { } }
    }
}
