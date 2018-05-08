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
using System.Windows.Media.Animation;

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

            if (!File.Exists("Client_Mod_Hashes.txt"))
            {
                File.Create("Client_Mod_Hashes.txt").Close();
            }
            
            if(!GlobalVariables.DevMode)
            {
                GenerateHashButton.Visibility = Visibility.Hidden;
                DeveloperMode_Label.Visibility = Visibility.Hidden;
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
            
        }

        public void ProgressBar_Change()
        {
            ProgressBar1.Maximum = 100;

            Action incPgBar = () => { ProgressBar1.Value++; };
            var task = new Task(
                () => 
                {
                    for (var i = 0; i < 10000; i++)
                    {
                        ProgressBar1.Dispatcher.Invoke(incPgBar);                        
                        System.Threading.Thread.Sleep(100);
                    }
                }
                );
            task.Start();
            ProgressBar1.Value = 0;
        }

        private void GenerateHashButton_Click(object sender, RoutedEventArgs e)
        {
            HashFile.GetGameFileHashes();
            ProgressBar_Change();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow SettingsWin = new SettingsWindow();
            
            SettingsWin.Owner = this;
            SettingsWin.Show();           
        }

        

        private void FacebookButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.facebook.com/groups/HOI.Economic.Crisis");
        }

        private void VKButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://vk.com/ec_hoi_mod");
        }

        private void YouTubeButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://www.youtube.com/c/HeartsofIronIVEconomicCrisis2013");
        }

        private void DiscordButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discordapp.com/invite/GPbPBe2");
        }

        private void SteamButton_Click(object sender, RoutedEventArgs e)
        {
            //Process.Start("https://www.facebook.com/groups/HOI.Economic.Crisis");
        }
    }
}
