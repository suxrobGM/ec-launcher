using System;
using Prism.Mvvm;

namespace EC_Launcher.Models
{
    public class ProgressData : BindableBase
    {
        private long currentValue;
        private long maxValue;
        private string statusText;

        public long CurrentValue
        {
            get => currentValue;
            set
            {
                SetProperty(ref currentValue, value);
                RaisePropertyChanged("Percentage");
            }
        }
        public long MaxValue
        {
            get => maxValue;
            set
            {
                SetProperty(ref maxValue, value);
                RaisePropertyChanged("Percentage");
            }
        }
        public int Percentage
        {
            get
            {
                if (MaxValue != 0)
                    return (int)(CurrentValue * 100 / MaxValue);
                else
                    return 0;
            }
        } 
        public string StatusText
        {
            get => statusText;
            set
            {
                SetProperty(ref statusText, value);
            }
        }       
    }
}
