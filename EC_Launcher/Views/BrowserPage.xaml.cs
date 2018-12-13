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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EC_Launcher.Views
{
    /// <summary>
    /// Логика взаимодействия для BrowserPage.xaml
    /// </summary>
    public partial class BrowserPage : Page
    {
        private bool firstPageLoaded;

        public BrowserPage()
        {
            InitializeComponent();

            // Загрузить страница мода в Фейсбуке 
            WBrowser.Navigate("https://m.facebook.com/HOI.Economic.Crisis");           
        }


        // Покрутить скролл немного ниже в первом странице фейсбука  
        private void WBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            if (!firstPageLoaded)
            {
                var html = WBrowser.Document as mshtml.HTMLDocument;
                html.parentWindow.scroll(0, 710);
                firstPageLoaded = true;
            }
        }
    }
}
