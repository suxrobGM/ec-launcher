using System;
using System.Windows;
using System.Net.Mail;
using System.Net;

namespace EC_Launcher
{
    /// <summary>
    /// Логика взаимодействия для ReportBugWindow.xaml
    /// </summary>
    public partial class ReportBugWindow : Window
    {
        public ReportBugWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Visibility = Visibility.Hidden;
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if(Name_TBox.Text == "")
            {
                MessageBox.Show("Please set your name");
                return;
            }

            if (Body_TBox.Text == "")
            {
                MessageBox.Show("Please write about bug");
                return;
            }

            string to = "suxrobGM@gmail.com";
            string from = "DedSec94@mail.ru";
            string p = "suxrobbek";

            try
            {
                using (MailMessage mm = new MailMessage(from, to))
                {
                    mm.Subject = "Reported Bug From @" + Name_TBox.Text;
                    mm.Body = Body_TBox.Text;
                    mm.IsBodyHtml = false;
                    using (SmtpClient sc = new SmtpClient("smtp.mail.ru", 25))
                    {
                        sc.EnableSsl = true;
                        sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                        sc.UseDefaultCredentials = false;
                        sc.Credentials = new NetworkCredential(from, p);
                        sc.Send(mm);
                        MessageBox.Show("Your message was successfully sent to suxrobGM@gmail.com \nThanks for reported bug, we will fix it soon ;)");
                    }
                }
            }
            catch(Exception)
            {
                MessageBox.Show(this, $"Network connection error, please check the network conection", "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }      
    }
}
