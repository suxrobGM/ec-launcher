using System;
using System.Xml.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using Dropbox.Api;

namespace EC_Launcher
{
    /// <summary>
    /// Update Class used Dropbox API
    /// </summary>
    public class UpdaterClient
    {
        private string tokenDropbox = "JCFYioFBHBAAAAAAAAAAFq4g6p6ZhtsYZJktjnNb_JFknLnJjKEMyASiPO7kKKK5";       
        private string rootFolder = "/EC_Server_Files"; //Корневой папка сервера
        private string versionXMLFile = "launcher/Version.xml";
        private DropboxClient dbx;
        private XDocument remoteVersionXML;
        private List<KeyValuePair<string, string>> remoteHashList;
        private List<KeyValuePair<string, string>> clientHashList;
        private ProgressData progressData;

        public Version RemoteAppVersion { get => VersionXML.ParseAppVersion(remoteVersionXML); }
        public Version RemoteModVersion { get => VersionXML.ParseModVersion(remoteVersionXML); }
        
        /// <summary>
        /// Список все файлы клиента
        /// </summary>
        public List<string> ClientFilesList { get; private set; }

        /// <summary>
        /// Список все файлы сервера
        /// </summary>
        public List<string> RemoteFilesList { get; private set; }

        /// <summary>
        /// Список новые файлы которые добавлена в сервере
        /// </summary>
        public List<string> NewFilesList { get; private set; }

        /// <summary>
        /// Список файлов которые удалена из сервера
        /// </summary>
        public List<string> DeletedFilesList { get; private set; }

        /// <summary>
        /// Список изменённые файлы в сервере (файлы который различается md5 значение)
        /// </summary>
        public List<string> ChangedFilesList { get; private set; }


        public UpdaterClient()
        {           
            this.clientHashList = new List<KeyValuePair<string, string>>();
            this.remoteHashList = new List<KeyValuePair<string, string>>();
            this.progressData = new ProgressData();
            this.dbx = new DropboxClient(tokenDropbox);

            using (var response = dbx.Files.DownloadAsync(rootFolder + "/" + versionXMLFile).Result)
            {
                using (var streamXML = response.GetContentAsStreamAsync().Result)
                {
                    this.remoteVersionXML = XDocument.Load(streamXML);
                    this.remoteVersionXML.Save(App.globalVars.CacheFolder + @"\launcher\Version.xml");
                }
            }
        }

        /// <summary>
        /// Checks application update in the host of DropBox
        /// </summary>
        /// <returns> true if application has an update otherwise false</returns>
        public bool CheckAppUpdate()
        {          
            if (App.globalVars.ApplicationVersion < this.RemoteAppVersion)
            {              
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks mod update in the host of DropBox
        /// </summary>
        /// <returns> true if mod has an update otherwise false</returns>
        public bool CheckModUpdate()
        {
            if (App.globalVars.ModVersion < this.RemoteModVersion)
            {               
                return true;
            }        
            return false;
        }         

        public async Task CheckModFilesAsync(IProgress<ProgressData> progress)
        {           
            this.progressData.value = 0;
            this.progressData.max = 1;
            this.progressData.statusText = "HashList.md5";
            progress?.Report(progressData); //Сообщить пользователя что скачается хеш-файл           

            using (var response = await dbx.Files.DownloadAsync(rootFolder + "/launcher/HashList.md5"))
            {
                // Скачать хеш-файл сервера
                var remoteHashFile = await response.GetContentAsByteArrayAsync();

                // Сохранить хеш-файл сервера на папке кеша
                File.WriteAllBytes(App.globalVars.CacheFolder + "\\launcher\\HashList.md5", remoteHashFile);

                // Хеш-файл клиента
                using (var clientHashFile = new FileStream("HashList.md5", FileMode.Open))
                {
                    // Получить список хеш-значение сервера
                    remoteHashList = HashFile.GetHashListFromFile(new FileStream(App.globalVars.CacheFolder + "\\launcher\\HashList.md5", FileMode.Open));

                    // Получить список хеш-значение клиента
                    clientHashList = HashFile.GetHashListFromFile(clientHashFile);

                    // UNIX path separator '/'
                    // Windows path separator '\\'
                    // All files convert to UNIX path separator
                    // Все пути в хеш файле(HashList.md5) в виде windows разделитела, надо его конвертировать на UNIX разделителя чтобы сервер взаимодействовал с ним                                                            
                    this.ClientFilesList = (from item in clientHashList select item.Key.Replace("\\", "/")).ToList();
                    this.RemoteFilesList = (from item in remoteHashList select item.Key.Replace("\\", "/")).ToList();
                    this.NewFilesList = RemoteFilesList.Except(ClientFilesList).ToList();
                    this.DeletedFilesList = ClientFilesList.Except(RemoteFilesList).ToList();
                    this.ChangedFilesList = clientHashList
                                           .Except(remoteHashList)
                                           .Select(item => item.Key.Replace("\\", "/"))
                                           .Except(DeletedFilesList) // Исключить файлы которые удалены из сервера
                                           .ToList();
                }
            }
        }
        
        // Скачать обновление, сначала скачается хеш-файл из сервера, потом сравнивается хеш-файл клиента с сервером
        public async void DownloadModUpdateAsync(IProgress<ProgressData> progress)
        {          
            await Task.Run(async () =>
            {
                await CheckModFilesAsync(progress);

                // Общее количество файлов для скачивание
                this.progressData.max = this.ChangedFilesList.Count + this.NewFilesList.Count;

                foreach (var file in this.ChangedFilesList)
                {
                    this.progressData.statusText = Path.GetFileName(file);
                    this.progressData.value++;
                    progress?.Report(progressData);
                    await DownloadFromDbx(rootFolder, file);
                }

                foreach (var file in this.NewFilesList)
                {
                    this.progressData.statusText = Path.GetFileName(file);
                    this.progressData.value++;
                    progress?.Report(progressData);
                    await DownloadFromDbx(rootFolder, file);
                }

                foreach (var file in this.DeletedFilesList)
                {
                    string fileNameWindows = file.Replace("/", "\\");

                    if (File.Exists(App.globalVars.ModDirectory + fileNameWindows))
                    {
                        File.Delete(App.globalVars.ModDirectory + fileNameWindows);
                    }
                }

                // Получаем список скачанных файлов в папке кеша
                string[] cacheFiles = Directory.GetFiles(App.globalVars.CacheFolder, "*", SearchOption.AllDirectories);

                // Получаем путь к папке Hearts of Iron IV/mod
                string modsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Hearts of Iron IV", "mod");

                // Перемещаем файлы из папке кеша на папку мода
                foreach (var file in cacheFiles)
                {
                    if (!file.Contains("EC_Launcher.exe") && !file.Contains("Settings.xml"))
                    {
                        // Перемещать файл .mod в MyDocuments\Paradox Interacive\Hearts of Iron IV\mod
                        if (file.Contains("Economic_Crisis.mod"))
                        {
                            CopyToFolder(file, modsFolder);
                            continue;
                        }
                        CopyToFolder(file, App.globalVars.ModDirectory);
                    }
                }
            });
        }

        // Скачиваем обновленный файл EC_Launcher.exe по частям
        public async void DownloadAppUpdateAsync(IProgress<int> progress)
        {          
            await Task.Run(async () =>
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
                                var percentage = ProgressData.GetPercentage((int)file.Length, (int)fileSize);
                                length = stream.Read(buffer, 0, bufferSize);
                                progress?.Report(percentage);
                            }
                        }
                    }
                }
                MessageToUserAndClose();
            });           
        }

        private async Task DownloadFromDbx(string rootFolder, string file)
        {
            using (var response = await dbx.Files.DownloadAsync(rootFolder + file))
            {              
                byte[] data = await response.GetContentAsByteArrayAsync();
                string fileNameWindows = file.Replace("/", "\\");

                // Если не существует такой каталог в папке кеша, тогда создаем новый каталог
                if (!Directory.Exists(App.globalVars.CacheFolder + Path.GetDirectoryName(fileNameWindows)))
                {
                    Directory.CreateDirectory(App.globalVars.CacheFolder + Path.GetDirectoryName(fileNameWindows));
                }

                File.WriteAllBytes(App.globalVars.CacheFolder + fileNameWindows, data);
            }
        }              

        private void CopyToFolder(string sourceFileFromCacheFolder, string destDirectory)
        {           
            int fullLength = sourceFileFromCacheFolder.Length;
            int cacheFolderLength = App.globalVars.CacheFolder.Length;

            string fileName = sourceFileFromCacheFolder.Remove(0, cacheFolderLength);
            string dirName = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(destDirectory + dirName))
            {
                Directory.CreateDirectory(destDirectory + dirName);
            }

            File.Copy(sourceFileFromCacheFolder, destDirectory + fileName, true);           
        }

        // Сообщить пользователя что надо перезагрузить лаунчера после скачивание обновлении лаунчера
        // Активируем наш Updater.exe передаем только одна аргумент который указывает путь к обновленный лаунчер в кеш папке
        private void MessageToUserAndClose()
        {
            MessageBox.Show("The application update has successfully downloaded, please press OK for continue", "Downloaded", MessageBoxButton.OK, MessageBoxImage.Information);
            string arguments = App.globalVars.CacheFolder + @"\launcher\EC_Launcher.exe";
            Process.Start("Updater.exe", arguments);
        }
    }
}
