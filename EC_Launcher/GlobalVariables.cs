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
        private bool isSteamVersion;

        public GlobalVariables()
        {
            appVersion = Assembly.GetExecutingAssembly().GetName().Version;
            modVersion = Version.Parse("0.6.3.0"); //default start version
            modDir = String.Empty;
            gameDir = String.Empty;
            devMode = false;
            isSteamVersion = false;
            cacheFolder = "_cache\\Economic_Crisis";

            // Проверят что есть ли стим у клиента, если нету тогда строка возвращает String.Empty
            string steamPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", String.Empty).ToString();     
            string gameSteamPath = steamPath + @"\steamapps\common\Hearts of Iron IV";

            // Получаем каталог мода в виде: <My Documents>\Paradox Interactive\Hearts of Iron IV\mod\Economic_Crisis
            string modPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Hearts of Iron IV", "mod", "Economic_Crisis");

            // Первый запуск приложений
            // Проверяем что есть ли стим версия игры у клиента
            if (Directory.Exists(gameSteamPath) && (!File.Exists("Settings.XML") || File.ReadAllText("Settings.XML") == String.Empty))
            {
                gameDir = gameSteamPath;
                isSteamVersion = true;
            }

            // Проверяем что существует ли каталог мода <My Documents>\Paradox Interactive\Hearts of Iron IV\mod\Economic_Crisis
            if (Directory.Exists(modPath) && (!File.Exists("Settings.XML") || File.ReadAllText("Settings.XML") == String.Empty))
            {
                modDir = modPath;
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
        public Version ModVersion { get => modVersion; set => modVersion = value; }
        public Version ApplicationVersion { get => appVersion; }
        public string CacheFolder { get => cacheFolder; }
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
            get => modDir;          
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
            get => gameDir;          
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
