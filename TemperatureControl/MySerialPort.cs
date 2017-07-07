using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Ports;

/*
    Набор передаваемых команд :
            Z - считать температуру
            n - добавить датчик
*/
namespace TemperatureControl
{
    class MySerialPort
    {
        private static Dictionary<int, byte[]> CommandDict = new Dictionary<int, byte[]>();
        private Queue<string> CommandQueue = new Queue<string>();
        private int SensorsCount;
        private int TotalSensorsCount;

        private SerialPort port = new SerialPort();

        private string name;
        private byte[] dataBytes = new byte[2];
        private byte[] sensorID = new byte[8];
        private byte systemByte = new byte();

        public void setPortName(string str)
        {
            name = str;
            port.PortName = name;
        }

        public MySerialPort()
        {
            SensorsCount = 0;
            TotalSensorsCount = 0;
            port.BaudRate = 4800;
            port.DataBits = 8;
            port.PortName = "COM1";
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            //port.ReadTimeout = 100;
            //port.WriteTimeout = 100;
            port.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
        }

        public event EventHandler UpdateCommandSended;
        public event EventHandler<SensorDataEventArgs> SensorDataRecievedEventHandler;
        public event EventHandler<SerialDataIDEventArgs> SensorIDRecivedEventHandler;


        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            //System.Threading.Thread.Sleep(100);
            var port = (SerialPort)sender;
            int bufSize = port.BytesToRead;
            string str;
            double temp;
            try
            {
                if (bufSize == 1)
                {
                    systemByte = (byte)port.ReadByte();
                }
                if (bufSize == 8)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        sensorID[i] = (byte)port.ReadByte();
                    }
                    str = SerialDataParser.ParseID(sensorID);

                    if (str != "FF-FF-FF-FF-FF-FF-FF-FF")
                    {
                        TotalSensorsCount += 1;
                        if (SensorIDRecivedEventHandler != null)
                        {
                            SensorIDRecivedEventHandler(this, new SerialDataIDEventArgs(str));
                        }
                    }
                    else
                    {
                        this.TakeCommand();
                    }                    
                }
                if (bufSize == 10)
                {
                    SensorsCount += 1;

                    for (int i = 0; i < 2; i++)
                    {
                            dataBytes[i] = (byte)port.ReadByte();   
                    }
                    for (int i = 0; i<8; i++)
                    {
                        sensorID[i] = (byte)port.ReadByte();
                    }
                    str = SerialDataParser.ParseID(sensorID);
                    temp = SerialDataParser.ParseTemp(dataBytes);
                    if (SensorDataRecievedEventHandler != null)
                    {
                        SensorDataRecievedEventHandler(this, new SensorDataEventArgs(str, temp));
                    }
                    if (SensorsCount == TotalSensorsCount)
                    {
                        SensorsCount = 0;
                        TakeCommand();
                    }
                }
            }
            catch { }
        }

        public string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public void Open()
        {
            if (port.IsOpen)
            {
                port.Close();
            }
            try
            {
                port.Open();
            }
            catch { }
            GenerateAddCommand();
            TakeCommand();
        }

        public void Close()
        {
            if(port.IsOpen)
            {
                CommandQueue.Clear();
                port.Close();
            }
        }

        private void WriteData(string data)
        {
            var str = ASCIIEncoding.ASCII.GetBytes(data);
            port.Write(str,0,1);
        }

        public void GenerateAddCommand()
        {
            CommandQueue.Enqueue("n");
        }

        public void TakeCommand()
        {
            string str = CommandQueue.Dequeue();
           
            if (!CommandQueue.Contains("Z"))
            {
                CommandQueue.Enqueue("Z");
            }
            WriteData(str);
            if (str == "n")
            {
                TotalSensorsCount = 0;
                UpdateCommandSended(this, new EventArgs());
            }
        }

        public void WriteSearch()
        {
            string data = "n";
            var str = ASCIIEncoding.ASCII.GetBytes(data);
            port.Write(str, 0, 1);
        }

        public void WriteTemp()
        {
            string data = "Z";
            var str = ASCIIEncoding.ASCII.GetBytes(data);
            port.Write(str, 0, 1);
        }

    }
}
