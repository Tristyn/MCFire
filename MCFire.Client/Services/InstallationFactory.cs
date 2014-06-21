using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using JetBrains.Annotations;
using MCFire.Client.Components;
using MCFire.Client.Primitives.Installations;

namespace MCFire.Client.Services
{
    [Export(typeof(IInstallationFactory))]
    class InstallationFactory:IInstallationFactory
    {
        [ImportMany, UsedImplicitly] IEnumerable<IInstallationTypeFactory> _factories; 

        public IInstallation Create(string path)
        {
            return path == null ? null : _factories.Select(factory => factory.Create(path)).FirstOrDefault(install => install != null);
        }
    }
}
