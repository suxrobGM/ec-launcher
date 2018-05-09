using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace EC_Launcher
{
    class HashFile
    {

        //словарь хранить себя хеши
        //public static Dictionary<string, string> HashDict = new Dictionary<string, string>();

        public static void GetGameFileHashes()
        {
            FileStream hash_file = new FileStream("Client_Mod_Hashes.txt", FileMode.Truncate);
            StreamWriter writer = new StreamWriter(hash_file);

            //получить польный список файлов(где-то 17-18к шт.)
            string[] files = Directory.GetFiles(GlobalVariables.MOD_DIR, "*", SearchOption.AllDirectories); 

            foreach (var file in files)
            {
                if (!file.Contains(".git")) //не счытивать файлы гита
                {
                    string file_name = file.ToString().Remove(0, GlobalVariables.MOD_DIR.Length);
                    using (var stream = File.OpenRead(file))
                    {
                        writer.WriteLine(GetHash_MD5(stream).ToString() + " - " + file_name); //записать на файле                      
                        //HashDict.Add(file_name, GetHash_MD5(stream).ToString());
                    }
                }
                    
            }
            writer.Close();
        }

        //получить md5 хеша
        public static string GetHash_MD5(Stream stream)
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
