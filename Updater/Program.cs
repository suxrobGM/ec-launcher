using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                foreach(var arg in args)
                {
                    Console.WriteLine(arg);
                }

                string programFromCache = args[0];
                string rootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string cachePath = rootPath + "\\" + Path.GetDirectoryName(args[0]);
                string pocessName = Path.GetFileNameWithoutExtension(args[0]);
                string exeFileName = Path.GetFileName(args[0]);

                Process[] myProcesses = Process.GetProcessesByName(pocessName);
                Console.WriteLine(myProcesses.Length);

                for (int i = 0; i < myProcesses.Length; i++)
                {
                    myProcesses[i].Kill();
                    Thread.Sleep(2000);
                }
                Console.WriteLine(myProcesses.Length);

                MoveFilesFromCache(cachePath, rootPath);
                Console.WriteLine("Moving files from cache...");

                Process.Start(exeFileName);                               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        static void MoveFilesFromCache(string cachePath, string destPath)
        {            
            string[] cacheFiles = Directory.GetFiles(cachePath, "*", SearchOption.AllDirectories);
            
            foreach (var file in cacheFiles)
            {
                if (file.Contains("Updater.exe"))
                    continue;

                string fileName = "\\" + Path.GetFileName(file);
                File.Copy(cachePath + fileName, destPath + fileName, true);
            }
           
        }
    }
}
