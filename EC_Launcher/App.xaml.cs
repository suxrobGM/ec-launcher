using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using EC_Launcher.Views;
using EC_Launcher.Models;

namespace EC_Launcher
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>    
    public partial class App : PrismApplication
    {            
        public static List<CultureInfo> Languages { get; private set; }
        public static CultureInfo Language
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                if (value == System.Threading.Thread.CurrentThread.CurrentUICulture)
                    return;

                //1. Меняем язык приложения:
                System.Threading.Thread.CurrentThread.CurrentUICulture = value;

                //2. Создаём ResourceDictionary для новой культуры
                ResourceDictionary dict = new ResourceDictionary();
                switch (value.Name)
                {
                    case "ru-RU":
                        dict.Source = new Uri(String.Format("Resources/Lang.{0}.xaml", value.Name), UriKind.Relative);
                        break;
                    default:
                        dict.Source = new Uri("Resources/Lang.xaml", UriKind.Relative);
                        break;
                }

                //3. Находим старую ResourceDictionary и удаляем его и добавляем новую ResourceDictionary
                ResourceDictionary oldDict = (from d in Application.Current.Resources.MergedDictionaries
                                              where d.Source != null && d.Source.OriginalString.StartsWith("Resources/Lang.")
                                              select d).First();
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }               
            }
        }

        public App()
        {
            Languages = new List<CultureInfo>();
            Languages.Clear();
            Languages.Add(new CultureInfo("en-US")); //Нейтральная культура для этого проекта
            Languages.Add(new CultureInfo("ru-RU"));
        }

        protected override Window CreateShell()
        {
            if (Environment.CommandLine.ToLower().Contains("-dev_mode"))
                SingletonModel.GetInstance().DevMode = true;

            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<BrowserPage>();
            containerRegistry.RegisterForNavigation<ReportBugPage>();
            containerRegistry.RegisterForNavigation<DonatePage>();
            containerRegistry.RegisterForNavigation<SettingsPage>();            
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //moduleCatalog.AddModule<ModuleA.ModuleAModule>();           
        }       
    }
}
