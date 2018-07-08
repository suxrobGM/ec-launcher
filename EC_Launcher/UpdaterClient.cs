using System;
using Dropbox.Api;
using System.Xml.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Diagnostics;

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

        public Version RemoteAppVersion { get => VersionXML.ParseAppVersion(remoteVersionXML); }
        public Version RemoteModVersion { get => VersionXML.ParseModVersion(remoteVersionXML); }

        public UpdaterClient()
        {
            try
            {
                dbx = new DropboxClient(tokenDropbox);
                var streamXML = dbx.Files.DownloadAsync(rootFolder + "/" + versionXMLFile).Result.GetContentAsStreamAsync().Result;
                this.remoteVersionXML = XDocument.Load(streamXML);
                this.remoteVersionXML.Save(App.globalVars.CacheFolder + @"\launcher\Version.xml");              
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
            if (App.globalVars.ApplicationVersion > RemoteAppVersion)
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
            if (App.globalVars.ModVersion > RemoteModVersion)
            {               
                return true;
            }
            //MoveFile(App.globalVars.CacheFolder + @"\launcher\Version.xml", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            return false;
        }
        
        public async void DownloadModUpdateAsync(IProgress<int> progress)
        {          
            await Task.Run(async () =>
            {
                try
                {
                    var remoteHashList = new List<KeyValuePair<string, string>>();
                    var localHashList = new List<KeyValuePair<string, string>>();

                    var streamRemoteHashFile = dbx.Files.DownloadAsync(rootFolder + "/launcher/HashList.md5").Result.GetContentAsStreamAsync().Result;
                    var streamLocalHashFile = new FileStream("HashList.md5", FileMode.Open);                  

                    remoteHashList = HashFile.GetHashListFromFile((FileStream)streamRemoteHashFile);
                    localHashList = HashFile.GetHashListFromFile(streamLocalHashFile);

                    // UNIX path separator '/'
                    // Windows path separator '\\'
                    // All files convert to UNIX path separator
                    var ChangedFilesList = localHashList.Except(remoteHashList).Select(item => item.Key.Replace("\\", "/")).ToList();
                    var LocalFilesList = (from item in localHashList select item.Key.Replace("\\", "/")).ToList();
                    var RemoteFilesList = (from item in remoteHashList select item.Key.Replace("\\", "/")).ToList();
                    var NewFilesList = RemoteFilesList.Except(LocalFilesList).ToList();
                    var DeletedFilesList = LocalFilesList.Except(RemoteFilesList).ToList();

                    // Add new HashList.md5 to downloading queue
                    NewFilesList.Add("/launcher/HashList.md5");                    

                    int maxDownloadedFiles = ChangedFilesList.Count + NewFilesList.Count;

                    int checkedFile = 0;
                    int progressPercent = 0;

                    foreach (var file in ChangedFilesList)
                    {
                        await DownloadFromDbx(rootFolder, file);

                        if (progress != null)
                        {
                            progress.Report(progressPercent);
                        }
                        checkedFile++;
                        progressPercent = GetPercentage(checkedFile, maxDownloadedFiles);
                    }

                    foreach (var file in NewFilesList)
                    {
                        await DownloadFromDbx(rootFolder, file);

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
                        File.Delete(App.globalVars.ModDirectory + fileNameWindows);
                        //File.Create(App.globalVars.CacheFolder + fileNameWindows + "_deleted");
                    }

                    //получаем список скачанных файлов в папке кеша
                    string[] cacheFiles = Directory.GetFiles(App.globalVars.CacheFolder, "*", SearchOption.AllDirectories);
                    string modsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Hearts of Iron IV", "mod");

                    //перемещаем файлы из папка кеша в папку мода
                    foreach (var file in cacheFiles)
                    {
                        if(!file.Contains("EC_Launcher.exe") && !file.Contains("Settings.xml"))
                        {
                            if(file.Contains("Economic_Crisis.mod")) //перемещать файл .mod в MyDocuments/Paradox Interacive/Hearts of Iron IV/mod
                            {
                                MoveFile(file, modsFolder);
                            }
                            else
                            {
                                MoveFile(file, App.globalVars.ModDirectory);
                            }                           
                        }
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Network connection error or Server does not response");
                }
            });
        }

        public async void DownloadAppUpdateAsync(IProgress<int> progress)
        {          
            await Task.Run(async () =>
            {
                try
                {
                    using (var response = await dbx.Files.DownloadAsync(rootFolder + "/launcher/EC_Launcher.exe"))
                    {
                        ulong fileSize = response.Response.Size;
                        const int bufferSize = 1024 * 1024;
                        byte[] buffer = new byte[bufferSize];
                        
                        using (var stream = await response.GetContentAsStreamAsync())
                        {
                            using (var file = new FileStream(App.globalVars.CacheFolder + @"\launcher\EC_Launcher.exe", FileMode.OpenOrCreate))
                            {
                                var length = stream.Read(buffer, 0, bufferSize);

                                while (length > 0)
                                {
                                    file.Write(buffer, 0, length);
                                    var percentage = GetPercentage((int)file.Length, (int)fileSize);
                                    length = stream.Read(buffer, 0, bufferSize);

                                    if (progress != null)
                                    {
                                        progress.Report(percentage);
                                    }
                                }
                            }
                        }
                    }
                    MessageToUserAndClose();
                }
                catch (Exception)
                {
                    throw new Exception("Network connection error or Server does not response");
                }               
            });           
        }

        private async Task DownloadFromDbx(string rootFolder, string file)
        {
            using (var response = await dbx.Files.DownloadAsync(rootFolder + "/"+ file))
            {              
                byte[] data = await response.GetContentAsByteArrayAsync();
                string fileNameWindows = file.Replace("/", "\\");            
                File.WriteAllBytes(App.globalVars.CacheFolder + fileNameWindows, data);
            }
        }

        private int GetPercentage(int current, int max)
        {
            return (current * 100) / max;
        }        

        private void MoveFile(string sourceFileFromCacheFolder, string destDirName)
        {           
            int fullLength = sourceFileFromCacheFolder.Length;
            int cacheFolderLength = App.globalVars.CacheFolder.Length;

            string fileName = sourceFileFromCacheFolder.Remove(0, cacheFolderLength);
            string dirName = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(destDirName+dirName))
            {
                Directory.CreateDirectory(destDirName + dirName);
            }

            File.Copy(sourceFileFromCacheFolder, destDirName + fileName, true);           
        }

        private void MessageToUserAndClose()
        {
            MessageBox.Show("The application update has successfully downloaded, please press OK for continue", "Downloaded", MessageBoxButton.OK, MessageBoxImage.Information);
            string arguments = App.globalVars.CacheFolder + @"\launcher\EC_Launcher.exe";
            Process.Start("Updater.exe", arguments);
        }
    }
}
