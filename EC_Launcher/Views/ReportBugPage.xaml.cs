using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using EC_Launcher.ViewModels;

namespace EC_Launcher.Views
{
    /// <summary>
    /// Логика взаимодействия для ReportBugPage.xaml
    /// </summary>
    public partial class ReportBugPage : Page
    {
        public ReportBugPage()
        {
            InitializeComponent();
        }

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Assuming you have one file that you care about, pass it off to whatever
                // handling code you have defined.
                
                (DataContext as ReportBugPageViewModel).ScreenshotFilesPath.AddRange(files);
            }
        }
    }
}
