using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using EC_Launcher.Models;
using System.Windows.Forms;

namespace EC_Launcher.ViewModels
{
    public class SettingsPageViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private int selectedLanguageIndex;
        private SingletonModel model;        


        public string GamePath
        {
            get => model.SettingsXml.GamePath;
            set
            {
                model.SettingsXml.GamePath = value;
                RaisePropertyChanged("GamePath");
            }
        }
        public string ModPath
        {
            get => model.SettingsXml.ModPath;
            set
            {
                model.SettingsXml.ModPath = value;
                RaisePropertyChanged("ModPath");
            }
        }
        public bool IsSteamVersion
        {
            get => model.SettingsXml.IsSteamVersion;
            set
            {
                model.SettingsXml.IsSteamVersion = value;
                RaisePropertyChanged("IsSteamVersion");
            }
        }
        public int SelectedLanguageIndex
        {
            get => selectedLanguageIndex;
            set
            {
                SetProperty(ref selectedLanguageIndex, value);

                if (selectedLanguageIndex == 0)
                {
                    App.Language = App.Languages[0]; //en-US
                    model.SettingsXml.AppLanguage = "English";
                }                    
                else if (selectedLanguageIndex == 1)
                {
                    App.Language = App.Languages[1]; //ru-RU 
                    model.SettingsXml.AppLanguage = "Russian";
                }                            
            }
        }
        public DelegateCommand SetGamePathCommand { get; }
        public DelegateCommand SetModPathCommand { get; }
        public DelegateCommand BackCommand { get; }

        public SettingsPageViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            model = SingletonModel.GetInstance();
            SelectedLanguageIndex = model.SettingsXml.AppLanguage == "English" ? 0 : 1;

            SetGamePathCommand = new DelegateCommand(() =>
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.SelectedPath = GamePath;
                    dialog.Description = "Please set the correct path of the Hearts of Iron IV";
                    dialog.ShowDialog();
                    GamePath = dialog.SelectedPath;
                }
            });

            SetModPathCommand = new DelegateCommand(() =>
            {
                using (var dialog = new FolderBrowserDialog())
                {
                    dialog.SelectedPath = ModPath;
                    dialog.Description = "Please set the correct path of the mod";
                    dialog.ShowDialog();
                    ModPath = dialog.SelectedPath;
                }
            });

            BackCommand = new DelegateCommand(() =>
            {
                regionManager.RequestNavigate("ViewsRegion", "BrowserPage");
            });
        }
    }
}
