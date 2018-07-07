using System;
using System.IO;
using System.Xml.Linq;

namespace EC_Launcher
{
    public class SettingsXML
    {
        private string settingsXML_File;
        private XDocument xDoc;

        public SettingsXML()
        {
            settingsXML_File = "Settings.xml";

            //если не существует файл, то создать файл и запольнить 
            if (!File.Exists("Settings.xml") || File.ReadAllText("Settings.xml") == String.Empty)
            {
                File.Create("Settings.xml").Close();
                this.SetDefaultSettings(App.globalVars.GameDirectory, App.globalVars.ModDirectory);
            }
            xDoc = XDocument.Load(settingsXML_File);
        }

        public string GamePath
        {
            get
            {
                var gamePath = xDoc.Element("General_Settings").Element("Game_Path").Value;
                return gamePath;
            }
            set
            {               
                xDoc.Element("General_Settings").Element("Game_Path").Value = value;
                xDoc.Save(settingsXML_File);
            }
        }
        public string ModPath
        {
            get
            {
                var modPath = xDoc.Element("General_Settings").Element("Mod_Path").Value;
                return modPath;
            }
            set
            {
                xDoc.Element("General_Settings").Element("Mod_Path").Value = value;
                xDoc.Save(settingsXML_File);
            }
        }
        public string AppLanguage
        {
            get
            {
                var language = xDoc.Element("General_Settings").Element("Language").Value;
                return language;
            }
            set
            {
                xDoc.Element("General_Settings").Element("Language").Value = value;
                xDoc.Save(settingsXML_File);
            }
        }
        
        private void SetDefaultSettings(string GamePath, string ModPath, string Language = "English")
        {
            XDocument SettingsDoc = new XDocument(
                                            new XElement("General_Settings",
                                                new XElement("Game_Path", GamePath),
                                                new XElement("Mod_Path", ModPath),
                                                new XElement("Language", Language)
                                            )
                                        );
            SettingsDoc.Save(settingsXML_File);
        }
    }
}
