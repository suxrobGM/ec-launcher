using System;
using System.Xml.Linq;

namespace EC_Launcher
{
    public static class SettingsXML
    {
        private static string settingsXML = "Settings.xml";
        private static XDocument SettingsDoc = XDocument.Load(settingsXML);

        public static string GamePath
        {
            get
            {
                var gamePath = SettingsDoc.Element("General_Settings").Element("Game_Path").Value;
                return gamePath;
            }
            set
            {
                SettingsDoc.Element("General_Settings").Element("Game_Path").Value = value;         
                SettingsDoc.Save(settingsXML);
            }
        }
        public static string ModPath
        {
            get
            {
                var modPath = SettingsDoc.Element("General_Settings").Element("Mod_Path").Value;
                return modPath;
            }
            set
            {
                SettingsDoc.Element("General_Settings").Element("Mod_Path").Value = value;
                SettingsDoc.Save(settingsXML);
            }
        }
        public static string AppLanguage
        {
            get
            {
                var language = SettingsDoc.Element("General_Settings").Element("Language").Value;
                return language;
            }
            set
            {
                SettingsDoc.Element("General_Settings").Element("Language").Value = value;
                SettingsDoc.Save(settingsXML);
            }
        }
        
        public static void SetDefaultSettings(string GamePath, string ModPath, string Language = "English")
        {
            XDocument SettingsDoc = new XDocument(
                                            new XElement("General_Settings",
                                                new XElement("Game_Path", GamePath),
                                                new XElement("Mod_Path", ModPath),
                                                new XElement("Language", Language)
                                            )
                                        );
            SettingsDoc.Save(settingsXML);
        }
    }
}
