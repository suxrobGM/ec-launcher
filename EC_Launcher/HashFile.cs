using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace EC_Launcher
{
    public class HashFile
    {
        //список хеши файлов
        private List<KeyValuePair<string, string>> HashDict;
        public int ProgressPercent;
        public int MaxFiles;      

        public HashFile()
        {
            this.ProgressPercent = 0;
            this.MaxFiles = 0;
            this.HashDict = new List<KeyValuePair<string, string>>();            
        }

        public async void GetGameFileHashesAsync(IProgress<int> progress)
        {          
            //получить польный список файлов(где-то 17-18к шт.)
            string[] files = Directory.GetFiles(App.globalVars.ModDirectory, "*", SearchOption.AllDirectories);
            MaxFiles = Directory.GetFiles(App.globalVars.ModDirectory, "*", SearchOption.AllDirectories).Count((file) => { return !file.Contains(".git") ? true : false; });

            HashDict =  await GetHashListAsync(files, progress);
            
            //Запись данные в файле Client.md5
            WriteHashListAsync("HashList.md5", HashDict);
        }

        private async void WriteHashListAsync(string file_name, List<KeyValuePair<string, string>> HashList)
        {
            FileStream hash_file = new FileStream(file_name, FileMode.Create);
            StreamWriter writer = new StreamWriter(hash_file);

            Action action = new Action(() =>
            {
                foreach (var item in HashList)
                {
                    writer.WriteLine(item.Value + " - " + item.Key);
                }
                writer.Close();
            });

            await Task.Run(action);        
        }

        private async Task<List<KeyValuePair<string, string>>> GetHashListAsync(string[] files_dir, IProgress<int> progress)
        {            
            List<KeyValuePair<string, string>> HashList = new List<KeyValuePair<string, string>>();
            int checkedFile = 0;           

            //Многопоточный режим
            Action action = new Action(() => {
                Parallel.ForEach(files_dir, (file) => {
                    Thread.CurrentThread.Priority = ThreadPriority.Highest;

                    //не счытивать файлы гита, EC_Launcher.exe, конфиг и кеш файлы
                    if (!file.Contains(".git") && !file.Contains(".xml") && !file.Contains("HashList.md5") && !file.Contains("EC_Launcher.exe") && !file.Contains("_cache")) 
                    {
                        string file_name = file.Remove(0, App.globalVars.ModDirectory.Length);

                        using (var stream = File.OpenRead(file))
                        {
                            string md5Value = GetHash_MD5(stream);
                            KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(file_name, md5Value);                          
                            HashList.Add(keyValuePair);                          

                            if (progress != null)
                            {
                                progress.Report(ProgressPercent);
                            }

                            checkedFile++;
                            ProgressPercent = GetPercentage(checkedFile, MaxFiles);
                        }
                    }
                });

                HashList = HashList.OrderBy(pair => pair.Key).ToList();
            });           
            await Task.Run(action);           
            return HashList;
        }

        //получить md5 хеша
        public string GetHash_MD5(Stream stream)
        {
            byte[] bytes;
            using (var md5 = new MD5CryptoServiceProvider())
            {
                bytes = md5.ComputeHash(stream);
            }


            var buffer = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
            {
                buffer.AppendFormat("{0:x2}", b);
            }

            return buffer.ToString();
        }

        public static List<KeyValuePair<string, string>> GetHashListFromFile(FileStream HashListFile)
        {
            StreamReader reader = new StreamReader(HashListFile);
            List<KeyValuePair<string, string>> HashList = new List<KeyValuePair<string, string>>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                string key = line.Remove(0, line.IndexOf("-") + 1).Trim();
                string value = line.Substring(0, line.IndexOf("-")).Trim();
                if (key != String.Empty || value != String.Empty)
                {
                    HashList.Add(new KeyValuePair<string, string>(key, value));
                }
            }

            return HashList;
        }

        private static int GetPercentage(int current, int max)
        {
            return (current * 100) / max;
        }
    }
}
