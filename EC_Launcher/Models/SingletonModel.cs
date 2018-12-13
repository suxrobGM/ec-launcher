using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Prism.Mvvm;

namespace EC_Launcher.Models
{
    public class SingletonModel : BindableBase
    {
        private static SingletonModel instance;
        private bool devMode;
        
        
        public SettingsXml SettingsXml { get; }
        public VersionXml VersionXml { get; }
        public bool DevMode { get => devMode; set { SetProperty(ref devMode, value); } }
        public string CacheFolder { get; set; }


        private SingletonModel()
        {
            CacheFolder = "_cache";

            // Сразу при запуска приложений создать каталог кеша: _cache\Economic_Crisis\launcher
            if (!Directory.Exists(CacheFolder + "\\launcher"))           
                Directory.CreateDirectory(CacheFolder + "\\launcher");          

            SettingsXml = new SettingsXml();
            VersionXml = new VersionXml(); 
        }

        public static SingletonModel GetInstance()
        {
            if (instance == null)
                instance = new SingletonModel();

            return instance;
        }

        // Очистить остальные моды и добавить Economic Crisis к списке модов в файле конфиг игры settings.txt (добавить галочку)
        public void SetTickGameLauncher()
        {
            string gameSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Hearts of Iron IV");
            string gameSettingsFile = gameSettingsPath + "\\settings.txt";

            if (File.Exists(gameSettingsFile))
            {
                var buffer = new List<string>(File.ReadAllLines(gameSettingsFile).ToList());
                string[] lastModRows =
                {
                    "last_mods={",
                    "\t\"mod/Economic_Crisis.mod\"",
                    "}"
                };
                int count = 0;

                if (buffer.Contains("last_mods={"))
                {
                    int i = buffer.IndexOf("last_mods={");
                    while (!buffer[i].Contains("}"))
                    {
                        count++;
                        i++;
                    }
                    buffer.RemoveRange(buffer.IndexOf("last_mods={"), count + 1);
                }
                buffer.AddRange(lastModRows);
                File.WriteAllLines(gameSettingsFile, buffer);
            }
        }


        // Проверка наличие файла Economic_Crisis.mod в пути Hearts of Iron IV/mod/ если нету файл тогда надо копировать файл
        public void CheckModFile()
        {
            string gameModFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Hearts of Iron IV", "mod");

            if (!File.Exists(gameModFolderPath + "\\Economic_Crisis.mod"))
                File.Copy("Economic_Crisis.mod", gameModFolderPath + "\\Economic_Crisis.mod");
        }

        public void StartSteamGame()
        {
            string steamPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", String.Empty).ToString() + "\\steam.exe";
            string steamArguments = "-applaunch 394360"; //id лицензионной версии хои из стима
            Process.Start(steamPath, steamArguments);
        }

        public void StartNonSteamGame()
        {
            string exePath = SettingsXml.GamePath + @"\hoi4.exe";
            Process.Start(exePath);
        }
    }
}
