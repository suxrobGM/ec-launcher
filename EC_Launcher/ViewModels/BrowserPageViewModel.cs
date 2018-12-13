using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Regions;

namespace EC_Launcher.ViewModels
{
    public class BrowserPageViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;

        public BrowserPageViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }
    }
}
