using System;
using System.Reflection;

namespace EC_Launcher
{
    public static class GlobalVariables
    {
        //Поля Данных
        private static string modDir;
        private static string gameDir;
        private static Version appVersion = Assembly.GetExecutingAssembly().GetName().Version;
        private static Version modVersion = new Version("0.6.3.0"); //default start version
        private static bool devMode = false;
        private static string cacheFolder = "_cache\\Economic_Crisis";

        //Свойства
        public static string ModDirectory  { get => modDir; set => modDir = value; }
        public static string GameDirectory { get => gameDir; set => gameDir = value; }
        public static bool DevMode { get => devMode; set => devMode = value; }
        public static Version ApplicationVersion { get => appVersion; }
        public static Version ModVersion { get => modVersion; set => modVersion = value; }
        public static string CacheFolder { get => cacheFolder; set => cacheFolder = value; }
    }
}
