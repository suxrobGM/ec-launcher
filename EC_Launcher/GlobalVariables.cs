using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EC_Launcher
{
    class GlobalVariables
    {
        public static string MOD_DIR = @"";
        public static string GAME_DIR = @"";
        public static string AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        public static bool DevMode = false;
    }
}
