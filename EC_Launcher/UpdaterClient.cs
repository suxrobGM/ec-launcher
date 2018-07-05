using System;
using Dropbox.Api;
using System.Xml.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EC_Launcher
{
    /// <summary>
    /// Update Class used Dropbox API
    /// </summary>
    public class UpdaterClient
    {
        private string tokenDropbox = "JCFYioFBHBAAAAAAAAAAFq4g6p6ZhtsYZJktjnNb_JFknLnJjKEMyASiPO7kKKK5";
        private DropboxClient dbx;
        private XDocument remoteVersionXML;
        private string rootFolder = "/EC_Server_Files";
        private string versionXMLFile = "launcher/Version.xml";   
        private bool hasModUpdate;
        private bool hasAppUpdate;                 

        public Version RemoteAppVersion { get => VersionXML.ParseAppVersion(remoteVersionXML); }
        public Version RemoteModVersion { get => VersionXML.ParseModVersion(remoteVersionXML); }

        public UpdaterClient()
        {
            try
            {
                dbx = new DropboxClient(tokenDropbox);
                var streamXML = dbx.Files.DownloadAsync(rootFolder + "/" + versionXMLFile).Result.GetContentAsStreamAsync().Result;
                this.remoteVersionXML = XDocument.Load(streamXML);
                this.remoteVersionXML.Save(GlobalVariables.CacheFolder + @"\Economic_Crisis\launcher\Version.xml");
                this.hasAppUpdate = false;
                this.hasModUpdate = false;
            }
            catch(Exception)
            {
                throw new Exception("Network connection error or Server does not response");
            }
        }

        /// <summary>
        /// Checks application update in the host of DropBox
        /// </summary>
        /// <returns>returns true if application has an update otherwise false</returns>
        public bool CheckAppUpdate()
        {          
            if (GlobalVariables.ApplicationVersion > RemoteAppVersion)
            {
                this.hasAppUpdate = true;
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
                this.hasModUpdate = true;
                return true;
            }
            //MoveFile(GlobalVariables.CacheFolder + @"\Economic_Crisis\launcher\Version.xml", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            return false;
        }
        
        public async void DownloadModUpdate(IProgress<int> progress)
        {
            if(!hasModUpdate)
            {
                throw new Exception("No update for mod");
            }

            try
            {
                var remoteHashList = new List<KeyValuePair<string, string>>();
                var localHashList = new List<KeyValuePair<string, string>>();

                var streamRemoteHashFile = dbx.Files.DownloadAsync(rootFolder + "/launcher/HashList.md5").Result.GetContentAsStreamAsync().Result;
                var streamLocalHashFile = new FileStream("HashList.md5", FileMode.Open);

                remoteHashList = HashFile.GetHashListFromFile((FileStream)streamRemoteHashFile);
                localHashList = HashFile.GetHashListFromFile(streamLocalHashFile);

                // UNIX style file path '/'
                // Windows style file path '\\'
                var ChangedFilesList = localHashList.Except(remoteHashList).Select(item => item.Key.Replace("\\", "/")).ToList();
                var LocalFilesList = (from item in localHashList select item.Key.Replace("\\", "/")).ToList();
                var RemoteFilesList = (from item in remoteHashList select item.Key.Replace("\\", "/")).ToList();
                var NewFilesList = RemoteFilesList.Except(LocalFilesList).ToList();
                var DeletedFilesList = LocalFilesList.Except(RemoteFilesList).ToList();

                int maxDownloadedFiles = ChangedFilesList.Count + NewFilesList.Count;                                

                int checkedFile = 0;
                int progressPercent = 0;

                foreach (var file in ChangedFilesList)
                {
                    await DownloadModFileFromDbx(rootFolder, file);

                    if (progress != null)
                    {
                        progress.Report(progressPercent);
                    }
                    checkedFile++;
                    progressPercent = GetPercentage(checkedFile, maxDownloadedFiles);
                }
              
                foreach (var file in NewFilesList)
                {
                    await DownloadModFileFromDbx(rootFolder, file);

                    if (progress != null)
                    {
                        progress.Report(progressPercent);
                    }
                    checkedFile++;
                    progressPercent = GetPercentage(checkedFile, maxDownloadedFiles);
                }

                foreach (var file in DeletedFilesList)
                {
                    string fileNameWindows = file.Replace("/", "\\");
                    //File.Delete(GlobalVariables.ModDirectory + fileNameWindows);
                    File.Create(GlobalVariables.CacheFolder + @"\Economic_Crisis" + fileNameWindows + "_deleted");
                }

                //Directory.Move(GlobalVariables.CacheFolder + @"\Economic_Crisis\" + versionXMLFile, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            }
            catch(Exception)
            {
                throw new Exception("Network connection error or Server does not response");
            }
        }

        private async Task DownloadModFileFromDbx(string rootFolder, string file)
        {
            using (var response = await dbx.Files.DownloadAsync(rootFolder + "/"+ file))
            {              
                byte[] data = await response.GetContentAsByteArrayAsync();
                string fileNameWindows = file.Replace("/", "\\");            
                File.WriteAllBytes(GlobalVariables.CacheFolder + @"\Economic_Crisis" + fileNameWindows, data);
            }
        }

        private int GetPercentage(int current, int max)
        {
            return (current * 100) / max;
        }
        
        private void MoveFile(string sourceFileFromCacheFolder, string destDirName)
        {           
            int fullLength = sourceFileFromCacheFolder.Length;
            int cacheFolderLength = (GlobalVariables.CacheFolder + @"\Economic_Crisis").Length;

            string fileName = sourceFileFromCacheFolder.Remove(0, cacheFolderLength);
            string dirName = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(destDirName+dirName))
            {
                Directory.CreateDirectory(destDirName + dirName);
            }

            File.Copy(sourceFileFromCacheFolder, destDirName + fileName, true);           
        }
    }
}
