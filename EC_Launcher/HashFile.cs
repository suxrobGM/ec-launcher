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
        public List<KeyValuePair<string, string>> HashDict = new List<KeyValuePair<string, string>>();

        public async void GetGameFileHashesAsync()
        {          
            //получить польный список файлов(где-то 17-18к шт.)
            string[] files = Directory.GetFiles(GlobalVariables.ModDirectory, "*", SearchOption.AllDirectories);

            HashDict = await GetHashListAsync(files);

            //Запись данные в файле Client.md5
            await WriteHashListAsync("Client.md5", HashDict);
        }

        private async Task WriteHashListAsync(string file_name, List<KeyValuePair<string, string>> HashList)
        {
            FileStream hash_file = new FileStream(file_name, FileMode.Truncate);
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

        private async Task<List<KeyValuePair<string, string>>> GetHashListAsync(string[] files_dir)
        {            
            List<KeyValuePair<string, string>> HashList = new List<KeyValuePair<string, string>>();

            //Многопоточный режим
            Action action = new Action(() => {
                Parallel.ForEach(files_dir, (file) => {
                    Thread.CurrentThread.Priority = ThreadPriority.Highest;

                    if (!file.Contains(".git") && !file.Contains("GenerateHash.exe") && !file.Contains("EC_Launcher.exe")) //не счытивать файлы гита
                    {
                        string file_name = file.Remove(0, GlobalVariables.ModDirectory.Length);

                        using (var stream = File.OpenRead(file))
                        {
                            KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(file_name, GetHash_MD5(stream).ToString());
                            HashList.Add(keyValuePair);
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
    }
}
