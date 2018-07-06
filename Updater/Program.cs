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
                string programmFromCache = args[0];
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

                File.Copy(programmFromCache, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + exeFileName, true);                
                Process.Start(exeFileName);                               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);                
            }
        }
    }
}
