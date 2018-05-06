using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;

namespace EC_Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string MOD_DIR = @"C:\Users\SuxrobGM\Documents\Paradox Interactive\Hearts of Iron IV\mod\EC2013";
        public static List<string> HashList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetGameFileHashes()
        {           
            FileStream hash_file = new FileStream("Client_Game_File_Hashes.txt", FileMode.Truncate);
            StreamWriter writer = new StreamWriter(hash_file);
            ProgressBar1.Maximum = Directory.EnumerateFiles(MOD_DIR + @"\history\countries").Count();

            foreach (var file in Directory.EnumerateFiles(MOD_DIR + @"\history\countries"))
            {
                string file_name = file.ToString().Remove(0, MOD_DIR.Length);
                using (var stream = File.OpenRead(file))
                {
                    writer.WriteLine(GetHash_MD5(stream).ToString() + " - " + file_name);
                    HashList.Add(GetHash_MD5(stream).ToString() + " - " + file_name);
                    ProgressBar1.Value++;
                }
            }
            writer.Close();
            //ProgressBar1.Value = 50;
        }

        private string GetHash_MD5(Stream stream)
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


        private void Window_Activated(object sender, EventArgs e)
        {
            WebBrowser1.Navigate("https://www.facebook.com/groups/HOI.Economic.Crisis");

            if (!File.Exists("Client_Game_File_Hashes.txt"))
            {
                File.Create("Client_Game_File_Hashes.txt").Close();
                //MessageBox.Show("Created Hash File ");
            }
        }


        private void OpenGameButton_Click(object sender, RoutedEventArgs e)
        {
            string exePath = @"D:\Games\Steam\steamapps\common\Hearts of Iron IV\hoi4.exe ";
            string arguments = @"-mod=mod\EC2013.mod";
            Process.Start(exePath, arguments);
            //progressBar1.Maximum = Process.Start(exePath).StartTime.Second;
            //progressBar1.Value = Process.Start(exePath).;
            //MessageBox.Show(progressBar1.Value.ToString());
        }

        private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            WebClient client = new WebClient();
            client.BaseAddress = "https://www.dropbox.com/sh/a3l30yu2ale22f6/AABOtNXGvsk6UYUzhNtfrmiba?dl=0";
            //client.DownloadFile("http://gitlab.ecrisis.su/nc/ec/tree/master/", "ec.jpg");

            GetGameFileHashes();
        }

        
    }
}
