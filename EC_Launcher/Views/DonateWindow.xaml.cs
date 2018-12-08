using System;
using System.Diagnostics;
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

        private void WebMoneyBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.wmtransfer.com/eng/inout/topup.shtml");
        }

        private void YandexMoneyBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://money.yandex.ru/to/410017131230374");
        }

        private void QiwiBtn_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://qiwi.com/payment/form/99");
        }       
    }
}
