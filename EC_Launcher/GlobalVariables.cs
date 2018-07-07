using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace EC_Launcher
{
    public sealed class GlobalVariables : INotifyPropertyChanged // Наследуемся от нужного интерфеса
    {   
        //Поля Данных
        private string modDir;
        private string gameDir;
        private Version appVersion;
        private Version modVersion;
        private bool devMode;
        private string cacheFolder;             

        public GlobalVariables()
        {
            appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            modVersion = new Version("0.6.3.0"); //default start version
            devMode = false;
            cacheFolder = "_cache\\Economic_Crisis";

            string steamPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", String.Empty).ToString();
            string gamePath = steamPath + @"\steamapps\common\Hearts of Iron IV";
            string modPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Hearts of Iron IV", "mod", "Economic_Crisis");

            if (Directory.Exists(gamePath) && (!File.Exists("Settings.XML") || File.ReadAllText("Settings.XML") == String.Empty))
            {
                gameDir = gamePath;
            }           

            if(Directory.Exists(modPath) && (!File.Exists("Settings.XML") || File.ReadAllText("Settings.XML") == String.Empty))
            {
                modDir = modPath;
            }          

            if (!Directory.Exists(CacheFolder + @"\launcher"))
            {
                Directory.CreateDirectory(CacheFolder + @"\launcher");
            }

            if (!File.Exists("HashList.md5"))
            {
                File.Create("HashList.md5").Close();
            }                      
        }

        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged; 

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void RaisePropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Свойства
        public bool DevMode { get => devMode; set => devMode = value; }
        public Version ApplicationVersion { get => appVersion; }
        public Version ModVersion { get => modVersion; set => modVersion = value; }
        public string CacheFolder { get => cacheFolder; set => cacheFolder = value; }
        public string ModDirectory
        {
            get
            {
                return modDir;
            }
            set
            {
                modDir = value;
                if(App.settingsXML != null)
                {
                    App.settingsXML.ModPath = value;
                }                                            
                RaisePropertyChanged("ModDirectory");
            }
        }
        public string GameDirectory
        {
            get
            {
                return gameDir;
            }
            set
            {
                gameDir = value;
                if(App.settingsXML != null)
                {
                    App.settingsXML.GamePath = value;
                }               
                RaisePropertyChanged("GameDirectory");
            }
        }   
    }
}
