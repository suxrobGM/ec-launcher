using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace EC_Launcher.Models
{
    public class HashGenerator
    {
        // Список хеш значений файлы мода
        private List<KeyValuePair<string, string>> hashList;
        private List<string> exceptionFiles; // list of files or folder which do not generate hash
        
        public string PathName { get; set; }
        public string OutputHashFile { get; set; }
        public ProgressData ProgressData { get; }

        public HashGenerator(string pathName, List<string> exceptionFiles)
        {
            this.exceptionFiles = exceptionFiles;
            hashList = new List<KeyValuePair<string, string>>();
            PathName = pathName;
            OutputHashFile = "HashList.md5";
            ProgressData = new ProgressData();
            exceptionFiles.Add(OutputHashFile);
        }

        public async Task GetGameFileHashesAsync()
        {
            await Task.Run(async () =>
            {
                // Получить польный список файлов мода (около 19к шт. файлов)
                string[] files = Directory.GetFiles(PathName, "*", SearchOption.AllDirectories);
                files = ExceptExceptionFiles(files, exceptionFiles).ToArray();

                // Получить список хеш значений файлов
                hashList = await GetHashListAsync(files);

                // Асинхронно записать данные на файле HashList.md5
                await WriteHashListAsync();

                if (ProgressData.CurrentValue != ProgressData.MaxValue)
                    ProgressData.CurrentValue = ProgressData.MaxValue;

                ProgressData.StatusText = "Generating hash file has successfully finished!";
            });
            
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
        public static List<KeyValuePair<string, string>> GetHashListFromFile(string hashFilePath)
        {
            using (var reader = new StreamReader(hashFilePath))
            {
                var HashList = new List<KeyValuePair<string, string>>();

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
        }

        // Метод возвращает список пар имя файла и md5 хеш значения
        private async Task<List<KeyValuePair<string, string>>> GetHashListAsync(string[] filesDirectory)
        {
            ProgressData.MaxValue = filesDirectory.Count();
            ProgressData.StatusText = "Generating hash file...";

            // Поток возвращает список пар "ключ-значение" здесь ключ имя файла, а значений это md5 хеш
            return await Task.Run(() =>
            {
             
                // Использование PLINQ
                return filesDirectory.AsParallel()
                .AsOrdered()
                //.Where(file => !exceptionFiles.Contains(file))
                .Select(file =>
                {
                    using (var stream = File.OpenRead(file))
                    {
                        ProgressData.CurrentValue++;                       

                        // Получить имя файла в виде например, /common/national_focus/russia.txt
                        string file_name = file.Remove(0, PathName.Length);
                        
                        // Получить md5 хеш значений
                        string md5_value = GetHash_MD5(stream);
                                                                    
                        return new KeyValuePair<string, string>(file_name, md5_value);
                    }                        
                })
                .OrderBy(pair => pair.Key)
                .ToList();              
            });                                                                 
        }
       
        private async Task WriteHashListAsync()
        {
            await Task.Run(() =>
            {
                using (var hashFile = new FileStream(OutputHashFile, FileMode.OpenOrCreate))
                {
                    using (var writer = new StreamWriter(hashFile))
                    {
                        foreach (var item in hashList)
                        {
                            writer.WriteLine(item.Value + " - " + item.Key);
                        }
                    }
                }
            });                          
        }
        
        private IEnumerable<string> ExceptExceptionFiles(IEnumerable<string> files, IEnumerable<string> exceptionFiles)
        {
            List<string> list = new List<string>();
            bool flag = false;
            foreach (var file in files)
            {
                foreach (var exceptFile in exceptionFiles)
                {
                    if (file.Contains(exceptFile))
                    {
                        flag = true;
                        break;
                    }              
                }
                if(!flag)
                    list.Add(file);

                flag = false; 
            }
            
            return list;
        }
    }
}
