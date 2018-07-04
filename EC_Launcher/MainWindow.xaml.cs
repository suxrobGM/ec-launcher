using System;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.ComponentModel;

namespace EC_Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SettingsWindow SettingsWind; //Новая окна Settings
        ReportBugWindow ReportBugWind;
        UpdaterClient client;
        HashFile hashFile;       
        Progress<int> progress;

        public MainWindow()
        {
            InitializeComponent();
            WBrowser.Navigate("https://vk.com/ec_hoi_mod");

            SettingsWind = new SettingsWindow();
            ReportBugWind = new ReportBugWindow();          

            if (!File.Exists("HashList.md5"))
            {
                File.Create("HashList.md5").Close();
            }

            //если не существует файл, то создать файл и запольнить 
            if (!File.Exists("Settings.xml"))
            {
                File.Create("Settings.xml").Close();
                SettingsXML.SetDefaultSettings(GlobalVariables.GameDirectory, GlobalVariables.ModDirectory);
            }
            //Загрузить данные из xml файла
            SettingsWind.SetUIValues(SettingsXML.GamePath, SettingsXML.ModPath, SettingsXML.AppLanguage);

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

            LauncherVersionLabel.Content = "Launcher Version: " + VersionXML.AppVersion;
            ModVersionLabel.Content = "Mod Version: " + VersionXML.ModVersion;
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
        }

        

        private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                client = new UpdaterClient();
                if (client.CheckAppUpdate())
                {
                    MessageBox.Show(this, $"Available update for launcher. New version is  {client.RemoteAppVersion}.\nDo you want to download the update?", "Available update for launcher!", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(this, "You are using the last version of launcher", "No update for launcher", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (client.CheckModUpdate())
                {
                    MessageBox.Show(this, $"Available update for mod. New version is  {client.RemoteModVersion}.\nDo you want to download the update?", "Available update for mod!", MessageBoxButton.OKCancel, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(this, "You are using the last version of mod", "No update for mod", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch(Exception)
            {
                MessageBox.Show(this, $"Network connection error, please check the network conection", "ERROR" , MessageBoxButton.OK, MessageBoxImage.Error);
            }     
        }

        private void GenerateHashButton_Click(object sender, RoutedEventArgs e)
        {           
            hashFile = new HashFile();
            //Обновление ProgressBar1 с асинхронного потока
            progress = new Progress<int>(value => { ProgressBar1.Dispatcher.Invoke(() => { ProgressBar1.Value = ++value; }); });        
            hashFile.GetGameFileHashesAsync(progress);
        }      
        



        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsWind.Owner = this;
            SettingsWind.ShowDialog();           
        }

        private void ReportBugButton_Click(object sender, RoutedEventArgs e)
        {
            ReportBugWind.Owner = this;
            ReportBugWind.ShowDialog();
        }

        private void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(hashFile.ProgressPercent.ToString());
        }

        private void FacebookButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.facebook.com/HOI.Economic.Crisis");
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
    }
}
