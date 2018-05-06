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
        
        

        public MainWindow()
        {
            InitializeComponent();
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

            HashFile.GetGameFileHashes();
        }

        
    }
}
