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
        // Список хеш значений файлы мода
        private List<KeyValuePair<string, string>> HashDict;
        private ProgressData progressData;      

        public HashFile()
        {      
            this.HashDict = new List<KeyValuePair<string, string>>();
            progressData = new ProgressData();
        }

        public async void GetGameFileHashesAsync(IProgress<ProgressData> progress)
        {   
            if(!Directory.Exists(App.globalVars.ModDirectory))
            {
                throw new Exception("Please set the right directory of the mod");
            }

            // Получить польный список файлов мода (около 17-18к шт. файлов)
            string[] files = Directory.GetFiles(App.globalVars.ModDirectory, "*", SearchOption.AllDirectories);                   

            // Получить список хеш значений файлов
            HashDict =  await GetHashListAsync(files, progress); 

            // Асинхронно записать данные на файле HashList.md5
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

        // Метод возвращает список пар имя файла и md5 хеш значения
        private async Task<List<KeyValuePair<string, string>>> GetHashListAsync(string[] files_dir, IProgress<ProgressData> progress)
        {            
            List<KeyValuePair<string, string>> HashList = new List<KeyValuePair<string, string>>();           

            // Многопоточный режим
            // Поток возвращает список пар "ключ-значение" здесь ключ имя файла, а значений это md5 хеш
            await Task.Run(() =>
            {
                // Получить общее количество файлов
                progressData.max = files_dir.Count(file => !file.Contains(".git") && !file.Contains("_cache") && !file.Contains("Settings.xml") && !file.Contains("HashList.md5") && !file.Contains("EC_Launcher.exe"));

                // Использование PLINQ
                HashList = files_dir.AsParallel()
                .AsOrdered()
                .Where(file => !file.Contains(".git") && !file.Contains("_cache") && !file.Contains("Settings.xml") && !file.Contains("HashList.md5") && !file.Contains("EC_Launcher.exe"))
                .Select(file =>
                {
                    var stream = File.OpenRead(file);

                    // Получить имя файла в виде например, /common/national_focus/russia.txt
                    string file_name = file.Remove(0, App.globalVars.ModDirectory.Length);

                    // Получить md5 хеш значений
                    string md5_value = GetHash_MD5(stream);
                    KeyValuePair<string, string> keyValuePair = new KeyValuePair<string, string>(file_name, md5_value);

                    progressData.value++;      

                    if (progress != null)
                    {
                        progress.Report(progressData);
                    }                   
                    return keyValuePair;
                })
                .OrderBy(pair => pair.Key)
                .ToList();              
            });                                                       
            return HashList;          
        }

        // Получить md5 хеш значений файла
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

        // Считать хеш значений из файла
        // Структура файл должна быть вот таком виде:
        // <md5 хеш значения> - <имя файла>
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
            reader.Close();

            return HashList;
        }      
    }
}
