using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.Win32;
using Prism.Mvvm;

namespace EC_Launcher.Models
{
    public class SettingsXml : BindableBase
    {
        private readonly string xmlFile;
        private string gamePath;
        private string modPath;
        private string appLanguage;
        private bool isSteamVersion;
        private bool firstExec;
        private XDocument xDoc;

        public SettingsXml()
        {
            xmlFile = "Settings.xml";
                  
            if (!File.Exists("Settings.xml") || File.ReadAllText("Settings.xml") == String.Empty)
            {
                File.Create("Settings.xml").Close();
                gamePath = GetSteamGamePath();
                modPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                if(modPath.EndsWith("\\launcher"))
                    modPath = modPath.Remove(modPath.IndexOf("\\launcher"), "\\launcher".Length);

                appLanguage = "English";
                isSteamVersion = CheckGameSteamVersion();
                firstExec = true;
                SetDefaultSettings();
            }
            xDoc = XDocument.Load(xmlFile);
        }

        public string GamePath
        {
            get
            {
                if(!firstExec)
                {
                    gamePath = xDoc.Element("General_Settings").Element("Game_Path").Value;
                }
                return gamePath;
            } 
            set
            {               
                SetProperty(ref gamePath, value);
                xDoc.Element("General_Settings").Element("Game_Path").Value = value;
                xDoc.Save(xmlFile);
            }
        }
        public string ModPath
        {
            get
            {
                if (!firstExec)
                {
                    modPath = xDoc.Element("General_Settings").Element("Mod_Path").Value;
                }
                return modPath;
            }
            set
            {
                SetProperty(ref modPath, value);
                xDoc.Element("General_Settings").Element("Mod_Path").Value = value;
                xDoc.Save(xmlFile);
            }
        }
        public string AppLanguage
        {
            get
            {
                if (!firstExec)
                {
                    appLanguage = xDoc.Element("General_Settings").Element("Language").Value;
                }
                return appLanguage;
            }
            set
            {
                SetProperty(ref appLanguage, value);
                xDoc.Element("General_Settings").Element("Language").Value = value;
                xDoc.Save(xmlFile);
            }
        }

        public bool IsSteamVersion
        {
            get
            {
                if (!firstExec)
                {
                    var xValue  = xDoc.Element("General_Settings").Element("Is_Steam_Version").Value;
                    isSteamVersion = xValue == "True" ? true : false;
                }
                return isSteamVersion;
            }
            set
            {
                if(CheckGameSteamVersion())
                {
                    SetProperty(ref isSteamVersion, value);
                    xDoc.Element("General_Settings").Element("Is_Steam_Version").Value = value ? "True" : "False";
                }
                else
                {
                    SetProperty(ref isSteamVersion, false);
                    xDoc.Element("General_Settings").Element("Is_Steam_Version").Value = "False";
                }

                xDoc.Save(xmlFile);
            }
        }

        private bool CheckGameSteamVersion()
        {
            // Проверят что есть ли стим у клиента, если нету тогда строка возвращает String.Empty
            string steamPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", String.Empty).ToString();
            string gameSteamPath = steamPath + @"\steamapps\common\Hearts of Iron IV";
           
            // Проверяем что есть ли стим версия игры у клиента           
            return Directory.Exists(gameSteamPath);
        }

        private string GetSteamGamePath()
        {
            string steamPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", String.Empty).ToString();
            if (steamPath != String.Empty)
            {
                string gameSteamPath = steamPath + @"\steamapps\common\Hearts of Iron IV";
                if (Directory.Exists(gameSteamPath))
                    return gameSteamPath;
            }
            return String.Empty;
        }
        
        private void SetDefaultSettings()
        {          
            XDocument SettingsDoc = new XDocument(
                                            new XElement("General_Settings",
                                                new XElement("Game_Path", gamePath),
                                                new XElement("Mod_Path", modPath),
                                                new XElement("Is_Steam_Version", isSteamVersion ? "True" : "False"),
                                                new XElement("Language", appLanguage)
                                            )
                                        );
            SettingsDoc.Save(xmlFile);
        }
    }
}
