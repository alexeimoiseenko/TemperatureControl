using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureControl
{
    class SerialDataIDEventArgs : EventArgs
    {
        public string message { get; set; }

        public SerialDataIDEventArgs(string str)
        {
            message = str;
        }
    }
}
