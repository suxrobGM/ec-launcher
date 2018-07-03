using System;
using Dropbox.Api;
using System.Xml.Linq;
using System.IO;
using System.Threading.Tasks;

namespace EC_Launcher
{
    /// <summary>
    /// Update Class used Dropbox API
    /// </summary>
    public class UpdateClient
    {
        private string tokenDropbox = "JCFYioFBHBAAAAAAAAAAFq4g6p6ZhtsYZJktjnNb_JFknLnJjKEMyASiPO7kKKK5";
        private DropboxClient dbx;
        private XDocument remoteVersionXML;
        private string rootFolder = "/Economic Crisis Files";
        private string versionXMLFile = "Version.xml";

        public Version RemoteAppVersion { get => VersionXML.ParseAppVersion(remoteVersionXML); }
        public Version RemoteModVersion { get => VersionXML.ParseModVersion(remoteVersionXML); }

        public UpdateClient()
        {
            dbx = new DropboxClient(tokenDropbox);
            var streamXML = dbx.Files.DownloadAsync(rootFolder + "/" + versionXMLFile).Result.GetContentAsStreamAsync().Result;
            remoteVersionXML = XDocument.Load(streamXML);
        }

        /// <summary>
        /// Checks application update in the host of DropBox
        /// </summary>
        /// <returns>returns true if application has an update otherwise false</returns>
        public bool CheckAppUpdate()
        {          
            if (GlobalVariables.ApplicationVersion > RemoteAppVersion)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks mod update in the host of DropBox
        /// </summary>
        /// <returns>returns true if mod has an update otherwise false</returns>
        public bool CheckModUpdate()
        {
            if (GlobalVariables.ModVersion > RemoteModVersion)
            {
                return true;
            }
            return false;
        }           
    }
}
