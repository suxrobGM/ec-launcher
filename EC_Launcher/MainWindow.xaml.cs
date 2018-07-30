using System;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

namespace EC_Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {      
        private SettingsWindow SettingsWind; // Новая окна Settings
        private ReportBugWindow ReportBugWind;
        private DonateWindow donateWindow;
        private bool firstPageLoaded; // Проверять что лаунчер загрузил первая страница фейсбука

        public MainWindow()
        {
            InitializeComponent();           

            // Загрузить страница мода в Фейсбуке 
            WBrowser.Navigate("https://m.facebook.com/HOI.Economic.Crisis");
            firstPageLoaded = false;

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
            try
            {
                SetLastModInGameSettings();
                CheckModFile();
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, ex.Message, this.FindResource("m_ERROR").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Если имеется лиц. копия игры стим тогда запускать игру из лаунчера стима
            if (App.globalVars.IsSteamVersion)
            {
                try
                {
                    string steamPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", String.Empty).ToString() + "\\steam.exe";                
                    string steamArguments = "-applaunch 394360"; //id лицензионной версии хои из стима
                    Process.Start(steamPath, steamArguments);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(this, ex.Message, this.FindResource("m_ERROR").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                string exePath = App.globalVars.GameDirectory + @"\hoi4.exe";
                //string arguments = @"-mod=mod\Economic_Crisis.mod";
                try
                {
                    Process.Start(exePath);
                }
                catch (Exception)
                {
                    MessageBox.Show(this, this.FindResource("m_InvalidGamePath").ToString(), this.FindResource("m_ERROR").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }           
        }

        // Очистить остальные моды и добавить Economic Crisis к списке модов в файле конфиг игры settings.txt
        private void SetLastModInGameSettings()
        {
            string gameSettingsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Hearts of Iron IV");
            string gameSettingsFile = gameSettingsPath + "\\settings.txt";

            if (File.Exists(gameSettingsFile))
            {
                var buffer = new List<string>(File.ReadAllLines(gameSettingsFile).ToList());
                string[] lastModRows =
                {
                    "last_mods={",
                    "\t\"mod/Economic_Crisis.mod\"",
                    "}"
                };
                int count = 0;

                if(buffer.Contains("last_mods={"))
                {
                    int i = buffer.IndexOf("last_mods={");
                    while (!buffer[i].Contains("}"))
                    {
                        count++;
                        i++;
                    }
                    buffer.RemoveRange(buffer.IndexOf("last_mods={"), count + 1);
                }              
                buffer.AddRange(lastModRows);
                File.WriteAllLines(gameSettingsFile, buffer);
            }
        }

        //Проверка наличие файла Economic_Crisis.mod в пути Hearts of Iron IV/mod/ если нету файл тогда надо копировать файл
        private void CheckModFile()
        {
            string gameModFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Hearts of Iron IV", "mod");           

            if (!File.Exists(gameModFolderPath + "\\Economic_Crisis.mod"))
            {
                File.Copy("Economic_Crisis.mod", gameModFolderPath + "\\Economic_Crisis.mod");
            }
        }

        private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var client = new UpdaterClient(); //клиент для скачивание обновление

                // Проверка обновлении лаунчера
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

                // Проверка обновления мода
                if (client.CheckModUpdate())
                {
                    var mboxResult = MessageBox.Show(this, $"{this.FindResource("m_AvailableModUpdateText").ToString()}  {client.RemoteModVersion}.\n{this.FindResource("m_DownloadChooseOptionText").ToString()}", this.FindResource("m_AvailableModUpdateCaption").ToString(), MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (mboxResult == MessageBoxResult.OK)
                    {                     
                        var progress = new Progress<ProgressData>(progressData => 
                        {
                            ProgressBar1.Dispatcher.Invoke(() => ProgressBar1.Value = progressData.GetPercentage());
                            statusText.Dispatcher.Invoke(() => statusText.Text = $"{this.FindResource("m_UpdatingMod").ToString()} \t{progressData.statusText}");
                            statusCountText.Dispatcher.Invoke(() => statusCountText.Text = $"{progressData.value}/{progressData.max}");
                        });
                        client.DownloadModUpdateAsync(progress);                       
                    }
                }
                else // Сообщить что клиент использует самый последняя версия мода
                {
                    MessageBox.Show(this, this.FindResource("m_NoModUpdateText").ToString(), this.FindResource("m_NoModUpdateCaption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch(Exception)
            {
                MessageBox.Show(this, "Network connection error or server does not response", this.FindResource("m_ERROR").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }     
        }

        private void GenerateHashButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var hashFile = new HashFile();
                //Обновление ProgressBar1 из асинхронного потока
                statusText.Text = this.FindResource("m_GeneratingHash").ToString();
                var  progress = new Progress<ProgressData>(progressData =>
                {
                    ProgressBar1.Dispatcher.Invoke(() => ProgressBar1.Value = progressData.GetPercentage() + 1);
                    statusCountText.Dispatcher.Invoke(() => statusCountText.Text = $"{progressData.value}/{progressData.max}");
                });
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
            Process.Start("https://steamcommunity.com/groups/ec_hoi_mod");          
        }

        private void ModdbButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.moddb.com/mods/hearts-of-iron-iv-economic-crisis");
        }

        // При закрытие приложении надо удалять папка кеша
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
                statusText.Text = $"{this.FindResource("m_ProgressFinished")}";
                statusCountText.Text = String.Empty;
            }
        }

        // Покрутить скролл немного ниже в первом странице фейсбука  
        private void WBrowser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            if(!firstPageLoaded)
            {
                var html = WBrowser.Document as mshtml.HTMLDocument;
                html.parentWindow.scroll(0, 710);
                firstPageLoaded = true;
            }                      
        }
    }
}
