using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Prism.Mvvm;

namespace EC_Launcher.Models
{
    public class VersionXml : BindableBase
    {
        private readonly string xmlFile;       
        private Version modVersion;
        private Version launcherVersion;
        private XDocument xDoc;

        public VersionXml()
        {
            xmlFile = "Version.xml";
            launcherVersion = Assembly.GetExecutingAssembly().GetName().Version;
            modVersion = new Version("0.6.3.0"); // 0.6.3.0 - default start version
      
            if (!File.Exists("Version.xml") || File.ReadAllText("Version.xml") == String.Empty)
            {
                File.Create("Version.xml").Close();
                SetDefaultVersion();
            }
            xDoc = XDocument.Load(xmlFile);
        }

        public Version ModVersion
        {
            get => modVersion;
            set
            {
                SetProperty(ref modVersion, value);
                xDoc.Element("Version").Element("Mod_Version").Value = value.ToString();
                xDoc.Save(xmlFile);
            }
        }

        public Version LauncherVersion
        {
            get => launcherVersion;
            set
            {
                SetProperty(ref launcherVersion, value);
                xDoc.Element("Version").Element("App_Version").Value = value.ToString();
                xDoc.Save(xmlFile);
            }
        }
        
        public static Version ParseLauncherVersion(XDocument versionXml)
        {
            return Version.Parse(versionXml.Element("Version").Element("App_Version").Value);
        }
        public static Version ParseModVersion(XDocument versionXml)
        {
            return Version.Parse(versionXml.Element("Version").Element("Mod_Version").Value);
        }

        private void SetDefaultVersion()
        {
            XDocument VersionDoc = new XDocument(
                                        new XElement("Version",
                                            new XComment("Do not change these strings!!!"),
                                            new XElement("Mod_Version", modVersion.ToString()),
                                            new XElement("App_Version", launcherVersion.ToString())
                                            )
                                        );
            VersionDoc.Save(xmlFile);
        }
    }
}
