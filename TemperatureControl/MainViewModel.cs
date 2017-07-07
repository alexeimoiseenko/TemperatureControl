using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;


namespace TemperatureControl
{
    public class MainViewModel : ViewModelBase
    {

        private ObservableCollection<TemperatureSensor> _sensors;
        public ObservableCollection<TemperatureSensor> Sensors
        {
            get
            {
                return _sensors;
            }
            set
            {
                _sensors = value;
                RaisePropertyChanged(() => Sensors);
            }
        }

        private MySerialPort port = new MySerialPort();

        private EmailSender ESender = new EmailSender();

        public ObservableCollection<string> PortNames { get; set; }

        private string _selectedPortName = "COM1";
        public string SelectedPortName
        {
            get { return _selectedPortName; }
            set
            {
                _selectedPortName = value;
                RaisePropertyChanged(() => SelectedPortName);
                port.setPortName(_selectedPortName);
            }
        }

        private bool _emailIsInserted;
        public bool EmailIsInserted
        {
            get
            {
                return _emailIsInserted;
            }
            set
            {
                _emailIsInserted = value;
                RaisePropertyChanged(() => EmailIsInserted);
            }
        }

        private bool _upTempIsChecked;
        public bool UpTempIsChecked
        {
            get
            {
                return _upTempIsChecked;
            }
            set
            {
                _upTempIsChecked = value;
                RaisePropertyChanged(() => UpTempIsChecked);
            }
        }

        private bool _lowTempIsChecked;
        public bool LowTempIsChecked
        {
            get
            {
                return _lowTempIsChecked;
            }
            set
            {
                _lowTempIsChecked = value;
                RaisePropertyChanged(() => LowTempIsChecked);
            }
        }

        private string _emailValue;
        public string EmailValue
        {
            get
            {
                return _emailValue;
            }
            set
            {
                _emailValue = value.ToLower();
                RaisePropertyChanged(() => EmailValue);
                SetAddres(value);
            }
        }

        private string _upTempValue;
        private double _upTempDouble;
        public string UpTempValue
        {
            get
            {
                return _upTempValue;
            }
            set
            {
                _upTempDouble = StringToDouble(value);
                _upTempValue = value;
                RaisePropertyChanged(() => UpTempValue);
            }
        }

        private string _lowTempValue;
        private double _lowTempDouble;
        public string LowTempValue
        {
            get
            {
                return _lowTempValue;
            }
            set
            {
                _lowTempDouble = StringToDouble(value);
                _lowTempValue = value;
                RaisePropertyChanged(() => LowTempValue);
            }
        }

        public MainViewModel()
        {
            LowTempValue = "20";
            UpTempValue = "35";
            EmailValue = "asm.tula@gmail.com";
            port.UpdateCommandSended += Port_UpdateCommandSended;
            port.SensorDataRecievedEventHandler += Port_SensorDataRecievedEventHandler;
            port.SensorIDRecivedEventHandler += Port_SensorIDRecivedEventHandler;
            Sensors = new ObservableCollection<TemperatureSensor>();
            PortNames = new ObservableCollection<string>();
            string[] newNames = port.GetPortNames();
            foreach (string newName in newNames)
            {
                PortNames.Add(newName);
            }
        }

        private void Port_UpdateCommandSended(object sender, EventArgs e)
        {
            Sensors.Clear();
        }

        private ICommand _startCommand;
        public ICommand StartCommand
        {
            get
            {
                return _startCommand ?? (_startCommand = new RelayCommand(() =>
                    port.Open()));
            }
        }

        private ICommand _endCommand;
        public ICommand EndCommand
        {
            get
            {
                return _endCommand ?? (_endCommand = new RelayCommand( () =>
                    {
                        port.Close();
                    }
                ));
            }
        }

        private ICommand _updateCommand;
        public ICommand UpdateCommand
        {
            get
            {
                return _updateCommand ?? (_updateCommand = new RelayCommand(() =>
                    port.GenerateAddCommand()
                    ));
            }
        }

        private void Port_SensorIDRecivedEventHandler(object sender, SerialDataIDEventArgs e)
        {
            bool flag = true;
            
            foreach(TemperatureSensor sens in Sensors)
            {
                if(sens.SensorID == e.message)
                {
                    flag = false;
                }
            }
            if(flag)
            {
                Sensors.Add(new TemperatureSensor(e.message));
            }
        }

        private double StringToDouble(string str)
        {
            return Convert.ToDouble(str);
        }

        private void SetAddres(string str)
        {
            ESender.SetToAddres(str);
        }

        private void Port_SensorDataRecievedEventHandler(object sender, SensorDataEventArgs e)
        {
            foreach(TemperatureSensor sens in Sensors)
            {                 
                if (sens.SensorID == e.ID)
                {                    
                    sens.Temperature = e.temp;
                    if (LowTempIsChecked)
                    {
                        if (sens.Temperature < _lowTempDouble && sens.FlagLow == false)
                        {
                            sens.FlagLow = true;
                            ESender.CreateNewEmailMessage(sens);
                        }
                        if (sens.Temperature > _lowTempDouble && sens.FlagLow == true)
                        {
                            sens.FlagLow = false;
                        }
                    }
                    if (UpTempIsChecked)
                    {
                        if (sens.Temperature > _upTempDouble && !sens.FlagLow )
                        {
                            sens.FlagUp = true;
                            ESender.CreateNewEmailMessage(sens);
                        }
                        if (sens.Temperature < _upTempDouble && sens.FlagUp)
                        {
                            sens.FlagUp = false;
                        }
                    }
                }
            }
        }
    }
}
