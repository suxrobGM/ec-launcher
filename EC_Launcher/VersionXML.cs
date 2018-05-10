using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EC_Launcher
{
    class VersionXML
    {
        //Getters
        public static string ReadModVersion()
        {
            XDocument VersionDoc = XDocument.Load("Version.xml");

            var ModVersion = VersionDoc.Element("Version").Element("Mod_Version").Value;

            return ModVersion.ToString();
        }

        public static string ReadModVersion(string Remote_VersionXML_File)
        {
            XDocument VersionDoc = XDocument.Load(Remote_VersionXML_File);

            var ModVersion = VersionDoc.Element("Version").Element("Mod_Version").Value;

            return ModVersion.ToString();
        }

        public static string ReadAppVersion()
        {
            XDocument VersionDoc = XDocument.Load("Version.xml");

            var AppVersion = VersionDoc.Element("Version").Element("App_Version").Value;

            return AppVersion.ToString();
        }

        public static string ReadAppVersion(string Remote_VersionXML_File)
        {
            XDocument VersionDoc = XDocument.Load(Remote_VersionXML_File);

            var AppVersion = VersionDoc.Element("Version").Element("App_Version").Value;

            return AppVersion.ToString();
        }



        //Setters
        public static void SetModVersion(string ModVersion)
        {
            XDocument VersionDoc = XDocument.Load("Version.xml");

            VersionDoc.Element("Version").Element("Mod_Version").Value = ModVersion;

            VersionDoc.Save("Version.xml");
        }

        public static void SetAppVersion(string AppVersion)
        {
            XDocument VersionDoc = XDocument.Load("Version.xml");

            VersionDoc.Element("Version").Element("App_Version").Value = AppVersion;

            VersionDoc.Save("Version.xml");
        }

        public static void SetDefaultVersion(string ModVersion, string AppVersion)
        {
            XDocument VersionDoc = new XDocument(
                                            new XElement("Version",
                                                new XComment("Do not change these strings!!!"),
                                                new XElement("Mod_Version", ModVersion),
                                                new XElement("App_Version", AppVersion)
                                            )
                                        );

            VersionDoc.Save("Version.xml");
        }
    }
}
