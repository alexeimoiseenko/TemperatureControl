using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Mail;
using System.Net;

namespace TemperatureControl
{
    class EmailSender
    {
        private static readonly string fromAddress = "temperaturecontrolapp@mail.ru";
        private string toAddress = "";

        private SmtpClient sc = new SmtpClient("smtp.mail.ru", 25);

        private Queue<MailMessage> MessageQueue = new Queue<MailMessage>();

        public EmailSender()
        {
            sc.EnableSsl = true;
            sc.DeliveryMethod = SmtpDeliveryMethod.Network;
            sc.UseDefaultCredentials = false;
            sc.Credentials = new NetworkCredential("temperaturecontrolapp@mail.ru", "LbgkjvDS18B20");
        }

        public void SetToAddres(string str)
        {
            toAddress = str;
        }

        public void CreateNewEmailMessage(TemperatureSensor sensor)
        {
            MailMessage message = new MailMessage(fromAddress, toAddress);
            string num = sensor.SensorID.ToString();
            string temp = sensor.Temperature.ToString();
            message.Body = "Превышен порог допустимых температурных показаний.\n Текущая температура на датчике " + num + " " + temp + ".";
            message.Subject = "Критические показания";
            message.IsBodyHtml = false;

            MessageQueue.Enqueue(message);
            Send();
        }

        public async Task Send()
        {
            foreach(MailMessage msm in MessageQueue)
            {
                try
                {
                    MailMessage ms = MessageQueue.Dequeue();
                    await sc.SendMailAsync(ms);
                }
                catch { }
            }
        }
    }

}
