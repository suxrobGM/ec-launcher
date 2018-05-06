using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;

namespace EC_Launcher
{
    class HashFile
    {
        public static List<string> HashList = new List<string>();
        public static void GetGameFileHashes()
        {
            FileStream hash_file = new FileStream("Client_Game_File_Hashes.txt", FileMode.Truncate);
            StreamWriter writer = new StreamWriter(hash_file);
            //ProgressBar1.Maximum = Directory.EnumerateFiles(MOD_DIR + @"\history\countries").Count();

            foreach (var file in Directory.EnumerateFiles(GlobalVariables.MOD_DIR + @"\history\countries"))
            {
                string file_name = file.ToString().Remove(0, GlobalVariables.MOD_DIR.Length);
                using (var stream = File.OpenRead(file))
                {
                    writer.WriteLine(GetHash_MD5(stream).ToString() + " - " + file_name);
                    HashList.Add(GetHash_MD5(stream).ToString() + " - " + file_name);
                    //ProgressBar1.Value++;
                }
            }
            writer.Close();
        }

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
