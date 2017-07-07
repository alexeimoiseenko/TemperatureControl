using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace TemperatureControl
{
    public class TemperatureSensor : ObservableObject
    {
        private string _sensorID;
        public string SensorID
        {
            get
            {
                return _sensorID;
            }
            set
            {
                _sensorID = value;
                RaisePropertyChanged(() => SensorID);
            }
        }

        private double _temperature;
        public double Temperature
        {
            get
            {
                return _temperature;
            }
            set
            {
                _temperature = value;
                RaisePropertyChanged(() => Temperature);
            }
        }

        private string _aboutSensor;
        public string AboutSensor
        {
            get
            {
                return _aboutSensor;
            }
            set
            {
                _aboutSensor = value;
                RaisePropertyChanged(() => AboutSensor);
            }
        }

        public bool FlagLow { get; set; }
        public bool FlagUp { get; set; }

        public TemperatureSensor(string str, double temp, string about)
        {
            FlagLow = false;
            FlagUp = false;
            SensorID = str;
            Temperature = temp;
            AboutSensor = about;
        }
        public TemperatureSensor(string str)
        {
            FlagLow = false;
            FlagUp = false;
            SensorID = str;
        }
    }
}
