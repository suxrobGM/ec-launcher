using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
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
using System.Xml.Linq;
using System.Security.Cryptography;

namespace EC_Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        SettingsWindow SettingsWind = new SettingsWindow(); //Новая окна Settings
        ReportBugWindow ReportBugWind = new ReportBugWindow();

        public MainWindow()
        {
            InitializeComponent();
            WBrowser.Navigate("https://vk.com/ec_hoi_mod");

            if (!File.Exists("Client_Mod_Hashes.txt"))
            {
                File.Create("Client_Mod_Hashes.txt").Close();
            }

            //если не существует файл, то создать файл и запольнить 
            if (!File.Exists("Settings.xml"))
            {
                File.Create("Settings.xml").Close();
                SettingsXML.SetDefaultSettings(GlobalVariables.GameDirectory, GlobalVariables.ModDirectory);
            }
            //Загрузить данные из xml файла
            SettingsWind.SetUIValues(SettingsXML.ReadGamePath(), SettingsXML.ReadModPath(), SettingsXML.ReadAppLanguage());

            if (!File.Exists("Version.xml"))
            {
                File.Create("Version.xml").Close();
                VersionXML.SetDefaultVersion(GlobalVariables.ModVersion.ToString(), GlobalVariables.ApplicationVersion.ToString());
            }

            if (!GlobalVariables.DevMode)
            {
                GenerateHashButton.Visibility = Visibility.Hidden;
                DeveloperMode_Label.Visibility = Visibility.Hidden;
            }

            LauncherVersionLabel.Content = "Launcher Version: " + VersionXML.ReadAppVersion();
            ModVersionLabel.Content = "Mod Version: " + VersionXML.ReadModVersion();
        }


        private void OpenGameButton_Click(object sender, RoutedEventArgs e)
        {
            string exePath = GlobalVariables.GameDirectory + @"\hoi4.exe";
            //string arguments = @"-mod=mod\EC2013.mod";
            try
            {
                Process.Start(exePath);
            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR:<"+ex.Message+ "> Please set right directory of game in the settings");
            }
            
            //progressBar1.Maximum = Process.Start(exePath).StartTime.Second;
            //progressBar1.Value = Process.Start(exePath).;
            //MessageBox.Show(progressBar1.Value.ToString());
        }

        private void CheckUpdates()
        {
            try
            {
                if (File.Exists("launcher.update") && new Version(FileVersionInfo.GetVersionInfo("launcher.update").FileVersion) > GlobalVariables.ApplicationVersion)
                {
                    Process.Start("Updater.exe", "launcher.update \"" + Process.GetCurrentProcess().ProcessName + "\"");
                    Process.GetCurrentProcess().CloseMainWindow();
                }
                else
                {
                    if (File.Exists("launcher.update"))
                    {
                        File.Delete("launcher.update");
                    }
                    Download();
                }
            }
            catch (Exception)
            {
                if (File.Exists("launcher.update"))
                {
                    File.Delete("launcher.update");
                }
                Download();
            }
        }

        private void Download()
        {
            string ServerVersionFile = "https://mysite.com/Version.xml";

            XDocument doc = new XDocument(XDocument.Load(ServerVersionFile));
            string remoteAppVersion = VersionXML.ReadAppVersion(ServerVersionFile);
            string reomteModVersion = VersionXML.ReadModVersion(ServerVersionFile);

        }

        private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            WebClient client = new WebClient();
            Uri uri = new Uri("https://yadi.sk/i/Qu4uWh-E3Up9Ju");
            client.DownloadFileAsync(uri, "photo.txt");

            //client.DownloadFile("http://gitlab.ecrisis.su/nc/ec/tree/master/", "ec.jpg");

        }

        public void ProgressBar_Change()
        {
            ProgressBar1.Maximum = 1000;

            Action incPgBar = new Action(() => { ProgressBar1.Value++; });
            Task task = new Task(() => {
                for (var i = 0; i < 1000; i++)
                {
                    ProgressBar1.Dispatcher.Invoke(incPgBar);                        
                    Thread.Sleep(100);
                }
            });
            task.Start();
            ProgressBar1.Value = 0;
        }

        private void GenerateHashButton_Click(object sender, RoutedEventArgs e)
        {
            HashFile hashFile = new HashFile();
            hashFile.GetGameFileHashesAsync();           
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWind.Owner = this;
            SettingsWind.ShowDialog();           
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
            //Process.Start("https://www.steam.com");
            
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void ReportBugButton_Click(object sender, RoutedEventArgs e)
        {
            ReportBugWind.Owner = this;
            ReportBugWind.ShowDialog();
        }
    }
}
