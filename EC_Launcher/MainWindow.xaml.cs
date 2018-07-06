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
        DonateWindow donateWindow;                    

        public MainWindow()
        {
            InitializeComponent();
            WBrowser.Navigate("https://vk.com/ec_hoi_mod");

            SettingsWind = new SettingsWindow();
            ReportBugWind = new ReportBugWindow();
            donateWindow = new DonateWindow();

            if (!Directory.Exists(GlobalVariables.CacheFolder + @"\launcher"))
            {
                Directory.CreateDirectory(GlobalVariables.CacheFolder + @"\launcher");
            }

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
            catch(Exception)
            {
                MessageBox.Show(this, "Please set right directory of game in the settings", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);                
            }
        }

        

        private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new UpdaterClient();
                if (client.CheckAppUpdate())
                {
                    var mboxResult = MessageBox.Show(this, $"Available update for launcher. New version is  {client.RemoteAppVersion}.\nDo you want to download the update?", "Available update for launcher!", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if(mboxResult == MessageBoxResult.OK)
                    {
                        statusText.Text = "Updating EC_Launcher.exe ...";
                        var progress = new Progress<int>(value => ProgressBar1.Dispatcher.Invoke(() => ProgressBar1.Value = ++value));
                        client.DownloadAppUpdateAsync(progress);
                    }
                }
                else
                {
                    MessageBox.Show(this, "You are using the last version of launcher", "No update for launcher", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                if (client.CheckModUpdate())
                {
                    var mboxResult = MessageBox.Show(this, $"Available update for mod. New version is  {client.RemoteModVersion}.\nDo you want to download the update?", "Available update for mod!", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (mboxResult == MessageBoxResult.OK)
                    {
                        statusText.Text = "Updating mod...";
                        var progress = new Progress<int>(value => ProgressBar1.Dispatcher.Invoke(() => ProgressBar1.Value = ++value));
                        client.DownloadModUpdateAsync(progress);                       
                    }
                }
                else
                {
                    MessageBox.Show(this, "You are using the last version of mod", "No update for mod", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR" , MessageBoxButton.OK, MessageBoxImage.Error);
            }     
        }

        private void GenerateHashButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hashFile = new HashFile();
                //Обновление ProgressBar1 из асинхронного потока
                statusText.Text = "Generating hash file...";
                var  progress = new Progress<int>(value => ProgressBar1.Dispatcher.Invoke(() => ProgressBar1.Value = ++value));
                hashFile.GetGameFileHashesAsync(progress);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }                                   
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
            donateWindow.Owner = this;
            donateWindow.ShowDialog();
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
            try
            {
                if (Directory.Exists("_cache"))
                {
                    Directory.Delete("_cache", true);
                }
                Environment.Exit(0);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }        

        private void ProgressBar1_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (ProgressBar1.Value == ProgressBar1.Maximum)
            {   
                ProgressBar1.Value = 0;
                statusText.Text = String.Empty;
            }
        }
    }
}
