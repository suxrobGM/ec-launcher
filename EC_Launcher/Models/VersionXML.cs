using System;
using System.IO;
using System.Xml.Linq;

namespace EC_Launcher
{
    public class VersionXML
    {
        private string versionXML_File;
        private XDocument xDoc;

        public VersionXML()
        {
            versionXML_File = "Version.xml";           

            if (!File.Exists("Version.xml") || File.ReadAllText("Version.xml") == String.Empty)
            {
                File.Create("Version.xml").Close();
                this.SetDefaultVersion(App.globalVars.ModVersion.ToString(), App.globalVars.ApplicationVersion.ToString());
            }
            xDoc = XDocument.Load(versionXML_File);
        }

        public Version ModVersion
        {
            get
            {
                var modVersion = xDoc.Element("Version").Element("Mod_Version").Value;
                return Version.Parse(modVersion);
            }
            set
            {
                xDoc.Element("Version").Element("Mod_Version").Value = value.ToString();
                xDoc.Save(versionXML_File);
            }
        }

        public Version AppVersion
        {
            get
            {
                var appVersion = xDoc.Element("Version").Element("App_Version").Value;
                return Version.Parse(appVersion);
            }
            set
            {
                xDoc.Element("Version").Element("App_Version").Value = value.ToString();
                xDoc.Save(versionXML_File);
            }
        }
        
        public static Version ParseAppVersion(XDocument serverVersionXML)
        {
            return Version.Parse(serverVersionXML.Element("Version").Element("App_Version").Value);
        }
        public static Version ParseModVersion(XDocument serverVersionXML)
        {
            return Version.Parse(serverVersionXML.Element("Version").Element("Mod_Version").Value);
        }

        private void SetDefaultVersion(string ModVersion, string AppVersion)
        {
            XDocument VersionDoc = new XDocument(
                                        new XElement("Version",
                                            new XComment("Do not change these strings!!!"),
                                            new XElement("Mod_Version", ModVersion),
                                            new XElement("App_Version", AppVersion)
                                            )
                                        );
            VersionDoc.Save(versionXML_File);
        }
    }
}
