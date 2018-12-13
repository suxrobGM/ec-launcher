using System;
using System.Windows;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using Prism.Regions;

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
    }
}
