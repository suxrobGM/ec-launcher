using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace EC_Launcher.ViewModels
{
    public class ReportBugPageViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private string senderName;
        private string text;


        public string SenderName { get => senderName; set { SetProperty(ref senderName, value); } }
        public string Text { get => text; set { SetProperty(ref text, value); } }
        public DelegateCommand SendCommand { get; }
        public DelegateCommand BackCommand { get; }

        public ReportBugPageViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            SendCommand = new DelegateCommand(() =>
            {
                SendMessageEmail("suxrobGM@gmail.com", "DedSec94@mail.ru");
                MessageBox.Show("Your message was successfully sent to suxrobGM@gmail.com Thanks for reported bug, we will fix it soon ;)");
            });

            BackCommand = new DelegateCommand(() =>
            {
                regionManager.RequestNavigate("ViewsRegion", "BrowserPage");
            });
        }


        private void SendMessageEmail(string from, string to)
        {
            using (MailMessage mm = new MailMessage(from, to))
            {
                mm.Subject = "EC: Reported Bug From @" + SenderName;
                mm.Body = Text;
                mm.IsBodyHtml = false;

                using (SmtpClient sc = new SmtpClient("smtp.mail.ru", 25))
                {
                    sc.EnableSsl = true;
                    sc.DeliveryMethod = SmtpDeliveryMethod.Network;
                    sc.UseDefaultCredentials = false;
                    sc.Credentials = new NetworkCredential(from, GetS());
                    sc.Send(mm);                    
                }
            }
        }

        private SecureString GetS()
        {
            var secureString = new SecureString();
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
