using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TemperatureControl
{
    class SerialDataParser
    {
        public static double ParseTemp(byte [] bytes)
        {
            byte buf;
            double data = 0;

            buf = (byte)(bytes[1] & 0x04);
            if (buf == 0x04)
            {
                data += 64;
            }
            buf = (byte)(bytes[1] & 0x02);
            if (buf == 0x02)
            {
                data += 32;
            }
            buf = (byte)(bytes[1] & 0x01);
            if (buf == 0x01)
            {
                data += 16;
            }
            buf = (byte)(bytes[0] & 0x80);
            if (buf == 0x80)
            {
                data += 8;
            }
            buf = (byte)(bytes[0] & 0x40);
            if (buf == 0x40)
            {
                data += 4;
            }
            buf = (byte)(bytes[0] & 0x20);
            if (buf == 0x20)
            {
                data += 2;
            }
            buf = (byte)(bytes[0] & 0x10);
            if (buf == 0x10)
            {
                data += 1;
            }
            buf = (byte)(bytes[0] & 0x08);
            if (buf == 0x08)
            {
                data += 0.5;
            }
            buf = (byte)(bytes[0] & 0x04);
            if (buf == 0x04)
            {
                data += 0.25;
            }
            buf = (byte)(bytes[0] & 0x02);
            if (buf == 0x02)
            {
                data += 0.125;
            }
            buf = (byte)(bytes[0] & 0x01);
            if (buf == 0x01)
            {
                data += 0.0625;
            }
            buf = (byte)(bytes[1] & 0x08);
            if (buf == 0x08)
            {
                data = data * (-1);
            }
            return data;
        }

        public static string ParseID(byte[] bytes)
        {
            string result;
            result = BitConverter.ToString(bytes);
            return result;
        }
    }
}
