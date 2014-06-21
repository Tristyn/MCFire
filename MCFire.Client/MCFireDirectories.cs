using System;
using System.IO;

namespace MCFire.Client
{
    public static class MCFireDirectories
    {
        public static readonly string Appdata =  Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        public static readonly string MCFireAppdata = Path.Combine(Appdata, "MCFire");

        public static readonly string AppdataMods = Path.Combine(MCFireAppdata, "Mods");

        public static readonly string MinecraftAppdata = Path.Combine(Appdata, ".minecraft");

        public static readonly string Install = Path.GetDirectoryName(new Uri(typeof(MCFireDirectories).Assembly.GetName().CodeBase).LocalPath);
    }
}
