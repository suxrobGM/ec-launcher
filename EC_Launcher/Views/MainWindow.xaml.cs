using System;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Prism.Regions;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace EC_Launcher.Views
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {                  
        public MainWindow(IRegionManager regionManager)
        {
            InitializeComponent();           
            regionManager.RegisterViewWithRegion("ViewsRegion", typeof(BrowserPage));
        }

        private void Frame_Navigating(object sender, System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (e.Content == null || (e.Content as string) == string.Empty)
                return;

            var ta = new ThicknessAnimation
            {
                Duration = TimeSpan.FromSeconds(0.3),
                DecelerationRatio = 0.7,
                To = new Thickness(0, 0, 0, 0)
            };
            if (e.NavigationMode == NavigationMode.New)
            {
                ta.From = new Thickness(500, 0, 0, 0);
            }
            else if (e.NavigationMode == NavigationMode.Back)
            {
                ta.From = new Thickness(0, 0, 500, 0);
            }
            (e.Content as Page).BeginAnimation(MarginProperty, ta);
        }
    }
}
