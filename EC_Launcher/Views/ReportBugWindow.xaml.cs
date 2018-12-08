using System;
using System.Windows;
using System.Net.Mail;
using System.Net;
using System.Security;

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
                MessageBox.Show(this.FindResource("m_SetNameText").ToString());
                return;
            }

            if (Body_TBox.Text == "")
            {
                MessageBox.Show(this.FindResource("m_SetDescText").ToString());
                return;
            }

            string from = "DedSec94@mail.ru";
            string to = "suxrobGM@gmail.com";                       

            try
            {
                using (MailMessage mm = new MailMessage(from, to))
                {
                    mm.Subject = "EC: Reported Bug From @" + Name_TBox.Text;
                    mm.Body = Body_TBox.Text;
                    mm.IsBodyHtml = false;
                    using (SmtpClient sc = new SmtpClient("smtp.mail.ru", 25))
                    {
                        sc.EnableSsl = true;
                        sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                        sc.UseDefaultCredentials = false;
                        sc.Credentials = new NetworkCredential(from, GetS());
                        sc.Send(mm);
                        MessageBox.Show(this.FindResource("m_MessageSuccessfullySentText").ToString());
                    }
                }
            }
            catch(Exception)
            {
                MessageBox.Show(this, this.FindResource("m_NetworkErrorText").ToString(), this.FindResource("m_ERROR").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private SecureString GetS()
        {
            SecureString secureString = new SecureString();
            secureString.AppendChar('s');
            secureString.AppendChar('u');
            secureString.AppendChar('x');
            secureString.AppendChar('r');
            secureString.AppendChar('o');
            secureString.AppendChar('b');
            secureString.AppendChar('b');
            secureString.AppendChar('e');
            secureString.AppendChar('k');
            return secureString;
        }
    }
}
