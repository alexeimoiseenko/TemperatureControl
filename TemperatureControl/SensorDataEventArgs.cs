using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureControl
{
    class SensorDataEventArgs : EventArgs
    {
        public string ID { get; set; }
        public double temp { get; set; }

        public SensorDataEventArgs(string str, double t)
        {
            temp = t;
            ID = str;
        }
    }
}
