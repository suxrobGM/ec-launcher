using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EC_Launcher
{
    public static class GlobalVariables
    {
        private static string modDir;
        private static string gameDir;
        private static Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
        private static Version modVersion = new Version("0.6.3.0"); //default start version
        private static bool devMode = false;

        public static string ModDirectory  { get => modDir; set => modDir = value; }
        public static string GameDirectory { get => gameDir; set => gameDir = value; }
        public static bool DevMode { get => devMode; set => devMode = value; }
        public static Version ApplicationVersion { get => appVersion; }
        public static Version ModVersion { get => modVersion; set => modVersion = value; }
    }
}
