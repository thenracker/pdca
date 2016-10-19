using System.Net.Mail;

namespace DataAccess.Models
{
    public class Email
    {
        public const string OutgoingAddress = "pdca@cz.swoboda.de";
        public const string SmtpAddress = "192.168.1.4";
        public const int SmtpPort = 25;

        public readonly SmtpClient ObjSmtpClient;
        public readonly MailMessage ObjeMailMessage;

        public string Subject
        {
            get { return ObjeMailMessage.Subject; }
            set { ObjeMailMessage.Subject = value; }
        }

        public string Body
        {
            get { return ObjeMailMessage.Body; }
            set { ObjeMailMessage.Body = value; }
        }

        public bool EnableSsl
        {
            get { return ObjSmtpClient.EnableSsl; }
            set { ObjSmtpClient.EnableSsl = value; }
        }
        
        public Email()
        {
            
            ObjSmtpClient = new SmtpClient(SmtpAddress, SmtpPort)
            {
                EnableSsl = false,
                UseDefaultCredentials = true
            };
            ObjeMailMessage = new MailMessage
            {
                IsBodyHtml = true,
                From = new MailAddress(OutgoingAddress)
            };
        }

        public void Send()
        {
            ObjSmtpClient.Send(ObjeMailMessage);
        }

        public void AddMailToAddress(string address)
        {
            ObjeMailMessage.To.Add(address);
        }

        public void ClearMailToAddressList()
        {
            ObjeMailMessage.To.Clear();
        }

        public void AddMailToCopyAddress(string address)
        {
            ObjeMailMessage.CC.Add(address);
        }

        public void ClearMailToCopyAddressList()
        {
            ObjeMailMessage.CC.Clear();
        }
    }
}
