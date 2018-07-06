using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

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
        }       

        public void SetUIValues(string GamePath, string ModPath, string Language)
        {
            GameDir_TBox.Text = GamePath;
            GlobalVariables.GameDirectory = GamePath;

            ModDir_TBox.Text = ModPath;
            GlobalVariables.ModDirectory = ModPath;

            if (Language == "English")
            {
                Language_CBox.SelectedIndex = 0;
            }               
            else if(Language == "Russian")
            {
                Language_CBox.SelectedIndex = 1;
            }
                            
        }        

        private void Language_CBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Language_CBox.SelectedIndex == 0)
            {
                SettingsXML.AppLanguage = "English";
                App.Language = App.Languages[0]; //en-US           

            }
            else if(Language_CBox.SelectedIndex == 1)
            {
                SettingsXML.AppLanguage = "Russian";               
                App.Language = App.Languages[1]; //ru-RU
            }
            
        }

        private void GameDir_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Please set the right directory of the game";                             
                dialog.ShowDialog();

                GlobalVariables.GameDirectory = dialog.SelectedPath;
                SettingsXML.GamePath = GlobalVariables.GameDirectory;
                GameDir_TBox.Text = GlobalVariables.GameDirectory;
            }         
        }

        private void ModDir_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Please set the right directory of the mod";
                string DefaultPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Paradox Interactive", "Hearts of Iron IV", "mod");
                DirectoryInfo HoiModDir = new DirectoryInfo(DefaultPath);               

                if (HoiModDir.Exists)
                {
                    dialog.SelectedPath = HoiModDir.ToString();                   
                }
                else
                {
                    dialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                }
                dialog.ShowDialog();

                GlobalVariables.ModDirectory = dialog.SelectedPath;
                SettingsXML.ModPath = GlobalVariables.ModDirectory;
                ModDir_TBox.Text = GlobalVariables.ModDirectory;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
    }
}
