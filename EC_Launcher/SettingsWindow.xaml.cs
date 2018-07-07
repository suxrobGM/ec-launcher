using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;

namespace EC_Launcher
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            DataContext = App.globalVars;

            string Language = App.settingsXML.AppLanguage;
            if (Language == "Russian")
            {
                Language_CBox.SelectedIndex = 1;
            }
            else //Language == "English"
            {
                Language_CBox.SelectedIndex = 0;
            }
            
        }                

        private void Language_CBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Language_CBox.SelectedIndex == 0)
            {
                App.settingsXML.AppLanguage = "English";
                App.Language = App.Languages[0]; //en-US           
            }
            else if(Language_CBox.SelectedIndex == 1)
            {
                App.settingsXML.AppLanguage = "Russian";               
                App.Language = App.Languages[1]; //ru-RU
            }            
        }

        private void GameDir_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = this.FindResource("m_SetGameDirDesc").ToString();                             
                dialog.ShowDialog();
                App.globalVars.GameDirectory = dialog.SelectedPath;                               
            }         
        }

        private void ModDir_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = this.FindResource("m_SetModDirDesc").ToString();
                string defaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Hearts of Iron IV", "mod");                              

                if (Directory.Exists(defaultPath))
                {
                    dialog.SelectedPath = defaultPath.ToString();                   
                }
                else
                {
                    dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                dialog.ShowDialog();
                App.globalVars.ModDirectory = dialog.SelectedPath;                               
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void SteamVer_ChkBox_Checked(object sender, RoutedEventArgs e)
        {
            string steamPath = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", String.Empty).ToString();
            string gameSteamPath = steamPath + @"\steamapps\common\Hearts of Iron IV";

            if (SteamVersion_ChkBox.IsChecked.Value)
            {
                if(!Directory.Exists(gameSteamPath))
                {
                    System.Windows.MessageBox.Show(this, this.FindResource("m_HaveNotSteamVersion").ToString(), this.FindResource("m_ERROR").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    SteamVersion_ChkBox.IsChecked = false;
                    App.globalVars.IsSteamVersion = false;
                }
                else
                {
                    App.globalVars.IsSteamVersion = true;
                }
            }
            else
            {
                App.globalVars.IsSteamVersion = false;
            }
        }
    }
}
