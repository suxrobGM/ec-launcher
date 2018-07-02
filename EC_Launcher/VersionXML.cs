using System;
using System.Xml.Linq;

namespace EC_Launcher
{
    public static class VersionXML
    {
        private static string versionXML = "Version.xml";
        private static XDocument versionDoc = XDocument.Load(versionXML);
        public static string ModVersion
        {
            get
            {
                var modVersion = versionDoc.Element("Version").Element("Mod_Version").Value;
                return modVersion;
            }
            set
            {
                versionDoc.Element("Version").Element("Mod_Version").Value = value;
                versionDoc.Save(versionXML);
            }
        }

        public static string AppVersion
        {
            get
            {
                var appVersion = versionDoc.Element("Version").Element("App_Version").Value;
                return appVersion;
            }
            set
            {
                versionDoc.Element("Version").Element("App_Version").Value = value;
                versionDoc.Save(versionXML);
            }
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
            VersionDoc.Save(versionXML);
        }
    }
}
