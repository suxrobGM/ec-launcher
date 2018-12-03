using System;

namespace EC_Launcher
{
    public struct ProgressData
    {
        public int value;
        public int max;       
        public string statusText;

        public int GetPercentage()
        {
            return (this.value * 100) / this.max;
        }

        public static int GetPercentage(int value, int max)
        {
            return (value * 100) / max;
        }
    }
}
