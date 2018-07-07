using System;
using System.Windows;

namespace EC_Launcher
{
    /// <summary>
    /// Логика взаимодействия для DonateWindow.xaml
    /// </summary>
    public partial class DonateWindow : Window
    {
        public DonateWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }
    }
}
