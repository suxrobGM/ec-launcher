using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;

namespace EC_Launcher.ViewModels
{
    public class DonatePageViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

        public DelegateCommand<string> GoToPaymentFormCommand { get; }
        public DelegateCommand BackCommand { get; }

        public DonatePageViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;

            GoToPaymentFormCommand = new DelegateCommand<string>((webUrl) =>
            {
                Process.Start(webUrl);
            });

            BackCommand = new DelegateCommand(() =>
            {
                regionManager.RequestNavigate("ViewsRegion", "BrowserPage");
            });
        }        
    }
}
