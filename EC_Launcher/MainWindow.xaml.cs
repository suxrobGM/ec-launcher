using System;
using System.Windows;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;

namespace EC_Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {      
        private SettingsWindow SettingsWind; //Новая окна Settings
        private ReportBugWindow ReportBugWind;
        private DonateWindow donateWindow;                    

        public MainWindow()
        {
            InitializeComponent();
            WBrowser.Navigate("https://vk.com/ec_hoi_mod");

            SettingsWind = new SettingsWindow();
            ReportBugWind = new ReportBugWindow();
            donateWindow = new DonateWindow();                                                                     

            if (!App.globalVars.DevMode)
            {
                GenerateHashButton.Visibility = Visibility.Hidden;
                DeveloperMode_Label.Visibility = Visibility.Hidden;
            }

            LauncherVersionLabel.Content = $"{this.FindResource("m_LauncherVersionLabel")} {App.globalVars.ApplicationVersion}";
            ModVersionLabel.Content = $"{this.FindResource("m_ModVersionLabel")} {App.globalVars.ModVersion}" ;
        }


        private void OpenGameButton_Click(object sender, RoutedEventArgs e)
        {
            string exePath = App.globalVars.GameDirectory + @"\hoi4.exe";
            //string arguments = @"-mod=mod\EC2013.mod";
            try
            {
                Process.Start(exePath);
            }
            catch(Exception)
            {
                MessageBox.Show(this, this.FindResource("m_InvalidGamePath").ToString(), this.FindResource("m_ERROR").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);                
            }
        }

        

        private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new UpdaterClient();
                if (client.CheckAppUpdate())
                {
                    var mboxResult = MessageBox.Show(this, $"{this.FindResource("m_AvailableAppUpdateText").ToString()}  {client.RemoteAppVersion}.\n{this.FindResource("m_DownloadChooseOptionText").ToString()}", this.FindResource("m_AvailableAppUpdateCaption").ToString(), MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if(mboxResult == MessageBoxResult.OK)
                    {
                        statusText.Text = this.FindResource("m_UpdatingLauncher").ToString(); ;
                        var progress = new Progress<int>(value => ProgressBar1.Dispatcher.Invoke(() => ProgressBar1.Value = ++value));
                        client.DownloadAppUpdateAsync(progress);
                    }
                }
                /*else
                {
                    MessageBox.Show(this, this.FindResource("m_NoAppUpdateText").ToString(), this.FindResource("m_NoAppUpdateCaption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                }
                */

                if (client.CheckModUpdate())
                {
                    var mboxResult = MessageBox.Show(this, $"{this.FindResource("m_AvailableModUpdateText").ToString()}  {client.RemoteModVersion}.\n{this.FindResource("m_DownloadChooseOptionText").ToString()}", this.FindResource("m_AvailableModUpdateCaption").ToString(), MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (mboxResult == MessageBoxResult.OK)
                    {
                        statusText.Text = this.FindResource("m_UpdatingMod").ToString();
                        var progress = new Progress<int>(value => ProgressBar1.Dispatcher.Invoke(() => ProgressBar1.Value = ++value));
                        client.DownloadModUpdateAsync(progress);                       
                    }
                }
                else
                {
                    MessageBox.Show(this, this.FindResource("m_NoModUpdateText").ToString(), this.FindResource("m_NoModUpdateCaption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.FindResource("m_ERROR").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }     
        }

        private void GenerateHashButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hashFile = new HashFile();
                //Обновление ProgressBar1 из асинхронного потока
                statusText.Text = this.FindResource("m_GeneratingHash").ToString();
                var  progress = new Progress<int>(value => ProgressBar1.Dispatcher.Invoke(() => ProgressBar1.Value = ++value));
                hashFile.GetGameFileHashesAsync(progress);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.FindResource("m_ERROR").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(this, ex.Message, this.FindResource("m_ERROR").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
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
