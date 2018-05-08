using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Windows;

namespace EC_Launcher
{
    public static class SettingsXML
    {
        
        //Getters
        public static string ReadGamePath()
        {            
            XDocument SettingsDoc = XDocument.Load("Settings.xml");

            var GamePath = SettingsDoc.Element("General_Settings").Element("Game_Path").Value;

            return GamePath.ToString();
        }

        public static string ReadModPath()
        {
            XDocument SettingsDoc = XDocument.Load("Settings.xml");

            var ModPath = SettingsDoc.Element("General_Settings").Element("Mod_Path").Value;
           
            return ModPath.ToString();
        }

        public static string ReadAppLanguage()
        {
            XDocument SettingsDoc = XDocument.Load("Settings.xml");

            var Language = SettingsDoc.Element("General_Settings").Element("Language").Value;

            return Language.ToString();
        }



        //Setters
        public static void SetGamePathValue(string GamePath)
        {
            XDocument SettingsDoc = XDocument.Load("Settings.xml");

            SettingsDoc.Element("General_Settings").Element("Game_Path").Value = GamePath;

            SettingsDoc.Save("Settings.xml");

        }

        public static void SetModPathValue(string ModPath)
        {
            XDocument SettingsDoc = XDocument.Load("Settings.xml");            

            SettingsDoc.Element("General_Settings").Element("Mod_Path").Value = ModPath;

            SettingsDoc.Save("Settings.xml");
        }

        public static void SetAppLanguageValue(string Language)
        {
            XDocument SettingsDoc = XDocument.Load("Settings.xml");

            SettingsDoc.Element("General_Settings").Element("Language").Value = Language;

            SettingsDoc.Save("Settings.xml");
        }



        public static void SetDefaultValues(string GamePath, string ModPath, string Language = "English")
        {
            XDocument SettingsDoc = new XDocument(
                 new XElement("General_Settings",
                    new XElement("Game_Path", GamePath),
                    new XElement("Mod_Path", ModPath),
                    new XElement("Language", Language)
                    )
            );
            
            SettingsDoc.Save("Settings.xml");
        }
    }
}
