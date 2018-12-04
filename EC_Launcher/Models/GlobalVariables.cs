using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace EC_Launcher
{
    public sealed class GlobalVariables : INotifyPropertyChanged 
    {        
        private string modDirectory;
        private string gameDirectory;      
        private bool isSteamVersion;

        public GlobalVariables()
        {          
            ModVersion = Version.Parse("0.6.3.0"); //default start version
            modDirectory = String.Empty;
            gameDirectory = String.Empty;
            DevMode = false;
            isSteamVersion = false;
            CacheFolder = "_cache\\Economic_Crisis";

            try
            {
                // Проверят что есть ли стим у клиента, если нету тогда строка возвращает String.Empty
                string steamPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", String.Empty).ToString();
                string gameSteamPath = steamPath + @"\steamapps\common\Hearts of Iron IV";
                
                // Первый запуск приложений
                // Проверяем что есть ли стим версия игры у клиента
                if (Directory.Exists(gameSteamPath) && (!File.Exists("Settings.XML") || File.ReadAllText("Settings.XML") == String.Empty))
                {
                    gameDirectory = gameSteamPath;
                    isSteamVersion = true;
                }
            }
            catch (Exception) { }

            // Получаем каталог мода в виде: <My Documents>\Paradox Interactive\Hearts of Iron IV\mod\Economic_Crisis
            string EconomicCrisisModPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Hearts of Iron IV", "mod", "Economic_Crisis");

            // Проверяем что существует ли каталог мода <My Documents>\Paradox Interactive\Hearts of Iron IV\mod\Economic_Crisis
            if (Directory.Exists(EconomicCrisisModPath) && (!File.Exists("Settings.XML") || File.ReadAllText("Settings.XML") == String.Empty))
            {
                modDirectory = EconomicCrisisModPath;
            }

            // Сразу при запуска приложений создать каталог кеша: _cache\Economic_Crisis\launcher
            if (!Directory.Exists(CacheFolder + @"\launcher"))
            {
                Directory.CreateDirectory(CacheFolder + @"\launcher");
            }

            // Если не существует хеш файл тогда создаем пустой хеш файл
            if (!File.Exists("HashList.md5"))
            {
                File.Create("HashList.md5").Close();
            }                      
        }       

        //Свойства
        public bool DevMode { get; set; }       
        public Version ModVersion { get; set; }
        public Version ApplicationVersion { get => Assembly.GetExecutingAssembly().GetName().Version; }
        public string CacheFolder { get; set; }
        public bool IsSteamVersion
        {
            get => isSteamVersion;
            set
            {
                isSteamVersion = value;
                if (App.settingsXML != null)
                {
                    App.settingsXML.IsSteamVersion = value;
                }               
                RaisePropertyChanged("IsSteamVersion");
            }
        }
        public string ModDirectory
        {
            get => modDirectory;          
            set
            {
                modDirectory = value;
                if(App.settingsXML != null)
                {
                    App.settingsXML.ModPath = value;
                }                                            
                RaisePropertyChanged("ModDirectory");
            }
        }
        public string GameDirectory
        {
            get => gameDirectory;          
            set
            {
                gameDirectory = value;
                if(App.settingsXML != null)
                {
                    App.settingsXML.GamePath = value;
                }               
                RaisePropertyChanged("GameDirectory");
            }
        }

        #region INotifyPropertyChanged
        // Событие, которое нужно вызывать при изменении
        public event PropertyChangedEventHandler PropertyChanged;

        // Для удобства обернем событие в метод с единственным параметром - имя изменяемого свойства
        public void RaisePropertyChanged(string propertyName)
        {
            // Если кто-то на него подписан, то вызывем его
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
