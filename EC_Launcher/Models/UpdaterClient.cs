using System;
using System.Xml.Linq;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using Dropbox.Api;

namespace EC_Launcher.Models
{
    /// <summary>
    /// Update Class used Dropbox API
    /// </summary>
    public class UpdaterClient
    {
        private readonly string dropboxToken;      
        private readonly string rootFolder;
        private readonly string localModPath;
        private string cacheFolder;
        private DropboxClient dropboxClient;
     
        public ProgressData ProgressData { get; }       

        public UpdaterClient(string dropboxToken, string rootFolder, string cacheFolder, string localModPath)
        {
            this.dropboxToken = dropboxToken;
            this.rootFolder = rootFolder;
            this.cacheFolder = cacheFolder;
            this.localModPath = localModPath;
            ProgressData = new ProgressData();
            dropboxClient = new DropboxClient(dropboxToken);          
        }

        /// <summary>
        /// Checks application update in the host of DropBox
        /// </summary>
        /// <returns> true if application has an update otherwise false</returns>
        public async Task<bool> CheckAppUpdateAsync()
        {
            return await Task.Run(async () =>
            {
                var localAppVersion = VersionXml.ParseLauncherVersion(XDocument.Load("Version.xml"));
                var remoteAppVersion = await GetRemoteAppVersionAsync();

                if (localAppVersion < remoteAppVersion)                
                    return true;

                return false;              
            });                       
        }

        /// <summary>
        /// Checks mod update in the host of DropBox
        /// </summary>
        /// <returns> true if mod has an update otherwise false</returns>
        public async Task<bool> CheckModUpdateAsync()
        {
            return await Task.Run(async () =>
            {
                var localModVersion = VersionXml.ParseModVersion(XDocument.Load("Version.xml"));
                var remoteModVersion = await GetRemoteModVersionAsync();

                if (localModVersion < remoteModVersion)
                    return true;

                return false;
            });
        }

        // Скачать обновление, сначала скачается хеш-файл из сервера, потом сравнивается хеш-файл клиента с сервером
        public async Task DownloadModUpdateAsync()
        {
            await Task.Run(async () =>
            {
                ProgressData.CurrentValue = 0;
                ProgressData.MaxValue = 1;
                ProgressData.StatusText = "Downloading metadata HashList.md5";
             
                // Скачать хеш-файл сервера
                await DownloadStreamFromDropboxAsync(rootFolder, "/launcher/HashList.md5");

                // Получить список хеш-значение сервера который скачана и сохранена в папке кеша
                var remoteHashList = HashGenerator.GetHashListFromFile($"{cacheFolder}\\launcher\\HashList.md5");

                // Получить список хеш-значение клиента
                var localHashList = HashGenerator.GetHashListFromFile("HashList.md5");

                // UNIX path separator '/'
                // Windows path separator '\\'
                // All files convert to UNIX path separator
                // Все пути в хеш файле(HashList.md5) в виде windows разделитела, надо его конвертировать на UNIX разделителя чтобы сервер взаимодействовал с ним                                                            
                var localFilesList = (from item in localHashList select item.Key.Replace("\\", "/")).ToList();
                var remoteFilesList = (from item in remoteHashList select item.Key.Replace("\\", "/")).ToList();
                var newFilesList = remoteFilesList.Except(localFilesList).ToList();
                var deletedFilesList = localFilesList.Except(remoteFilesList).ToList();
                var changedFilesList = localHashList.Except(remoteHashList)
                                                    .Select(item => item.Key.Replace("\\", "/"))
                                                    .Except(deletedFilesList)
                                                    .ToList(); // Исключить файлы которые удалены из сервера

                // Общее количество файлов для скачивание
                ProgressData.MaxValue = changedFilesList.Count + newFilesList.Count;

                foreach (var file in changedFilesList)
                {
                    ProgressData.StatusText = file;
                    await DownloadStreamFromDropboxAsync(rootFolder, file);
                    ProgressData.CurrentValue++;
                }

                foreach (var file in newFilesList)
                {
                    ProgressData.StatusText = file;
                    await DownloadStreamFromDropboxAsync(rootFolder, file);
                    ProgressData.CurrentValue++;
                }

                foreach (var file in deletedFilesList)
                {
                    string fileNameWindows = file.Replace("/", "\\");

                    if (File.Exists(localModPath + fileNameWindows))
                        File.Delete(localModPath + fileNameWindows);
                }

                MoveFilesFromCacheFolder();
                ProgressData.StatusText = "Economic Crisis has successfully updated!";
            });          
        }                  

        // Скачиваем обновления для лаунчера
        public async Task DownloadAppUpdateAsync()
        {          
            await Task.Run(async () =>
            {
                ProgressData.CurrentValue = 0;
                ProgressData.MaxValue = 0;               
                
                var remoteLauncherFiles = await GetRemoteFiles($"{rootFolder}/launcher", true);
                ProgressData.MaxValue = remoteLauncherFiles.Count;

                foreach (var filePath in remoteLauncherFiles)
                {
                    var file = filePath.Substring(rootFolder.Length); // delete name of root folder in the path string
                    ProgressData.StatusText = $"Downloading {Path.GetFileName(file)}";
                    await DownloadStreamFromDropboxAsync(rootFolder, file);
                    ProgressData.CurrentValue++;
                }

                MoveFilesFromCacheFolder();
                NotifyUserThenClose();
            });           
        }

        public async Task<Version> GetRemoteModVersionAsync()
        {
            return await Task.Run(async () =>
            {
                using (var response = await dropboxClient.Files.DownloadAsync(rootFolder + "/launcher/Version.xml"))
                {
                    using (var streamXML = await response.GetContentAsStreamAsync())
                    {
                        return VersionXml.ParseModVersion(XDocument.Load(streamXML));
                    }
                }
            });
        }

        public async Task<Version> GetRemoteAppVersionAsync()
        {
            return await Task.Run(async () =>
            {
                using (var response = await dropboxClient.Files.DownloadAsync(rootFolder + "/launcher/Version.xml"))
                {
                    using (var streamXML = await response.GetContentAsStreamAsync())
                    {
                        return VersionXml.ParseLauncherVersion(XDocument.Load(streamXML));
                    }
                }
            });
        }

        private async Task<List<string>> GetRemoteFiles(string folderPath, bool recrusiveMode)
        {
            return await Task.Run(async () =>
            {
                var response = await dropboxClient.Files.ListFolderAsync(folderPath, recrusiveMode);               
                return response.Entries.Where(i => i.IsFile && !i.PathDisplay.Contains("Settings.xml") && !i.PathDisplay.Contains("HashList.md5")).Select(i => i.PathDisplay).ToList();
            });
        }

        private async Task DownloadBytesFromDropboxAsync(string rootFolder, string file)
        {
            await Task.Run(async () =>
            {
                using (var response = await dropboxClient.Files.DownloadAsync(rootFolder + file))
                {
                    byte[] data = await response.GetContentAsByteArrayAsync();
                    string fileNameWindows = file.Replace("/", "\\");

                    // Если не существует такой каталог в папке кеша, тогда создаем новый каталог
                    if (!Directory.Exists(cacheFolder + Path.GetDirectoryName(fileNameWindows)))
                        Directory.CreateDirectory(cacheFolder + Path.GetDirectoryName(fileNameWindows));

                    File.WriteAllBytes(cacheFolder + fileNameWindows, data);
                }
            });           
        }

        private async Task DownloadStreamFromDropboxAsync(string rootFolder, string filePath)
        {
            await Task.Run(async () =>
            {
                using (var response = await dropboxClient.Files.DownloadAsync(rootFolder + filePath))
                {
                    ulong fileSize = response.Response.Size;
                    const int bufferSize = 1024 * 1024;
                    byte[] buffer = new byte[bufferSize];
                    //ProgressData.MaxValue = (long)fileSize;

                    using (var stream = await response.GetContentAsStreamAsync())
                    {
                        if (!Directory.Exists(Path.GetDirectoryName(cacheFolder + filePath)))
                            Directory.CreateDirectory(Path.GetDirectoryName(cacheFolder + filePath));

                        using (var file = new FileStream(cacheFolder + filePath, FileMode.OpenOrCreate))
                        {
                            var length = stream.Read(buffer, 0, bufferSize);

                            while (length > 0)
                            {
                                file.Write(buffer, 0, length);
                                //ProgressData.CurrentValue = file.Length;
                                length = stream.Read(buffer, 0, bufferSize);
                            }
                        }
                    }
                }
            });            
        }

        private void CopyToFolder(string sourceFileFromCacheFolder, string destDirectory)
        {           
            int fullLength = sourceFileFromCacheFolder.Length;           

            string fileName = sourceFileFromCacheFolder.Remove(0, cacheFolder.Length);
            string dirName = Path.GetDirectoryName(fileName);

            if (!Directory.Exists(destDirectory + dirName))            
                Directory.CreateDirectory(destDirectory + dirName);           

            File.Copy(sourceFileFromCacheFolder, destDirectory + fileName, true);           
        }

        private void MoveFilesFromCacheFolder()
        {
            // Получаем список скачанных файлов в папке кеша
            string[] cacheFiles = Directory.GetFiles(cacheFolder, "*", SearchOption.AllDirectories);

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
                    CopyToFolder(file, localModPath);
                }
            }
        }

        // Сообщить пользователя что надо перезагрузить лаунчера после скачивание обновлении лаунчера
        // Активируем наш Updater.exe передаем только одна аргумент который указывает путь к обновленный лаунчер в кеш папке
        private void NotifyUserThenClose()
        {
            MessageBox.Show("The application update has successfully downloaded, please press OK for continue", "Downloaded", MessageBoxButton.OK, MessageBoxImage.Information);
            string arguments = cacheFolder + @"\launcher\EC_Launcher.exe";
            Process.Start("Updater.exe", arguments);
        }
    }
}
