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
using System.Windows.Shapes;
using System.Windows.Forms;

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
                SettingsXML.SetAppLanguageValue("English");
            }
            else if(Language_CBox.SelectedIndex == 1)
            {
                SettingsXML.SetAppLanguageValue("Russian");
            }
            
        }

        private void GameDir_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                GlobalVariables.GameDirectory = dialog.SelectedPath;
                SettingsXML.SetGamePathValue(GlobalVariables.GameDirectory);
                GameDir_TBox.Text = GlobalVariables.GameDirectory;
            }         
        }

        private void ModDir_Button_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                GlobalVariables.ModDirectory = dialog.SelectedPath;
                SettingsXML.SetModPathValue(GlobalVariables.ModDirectory);
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
