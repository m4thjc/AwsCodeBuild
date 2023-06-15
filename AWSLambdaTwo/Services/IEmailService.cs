using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;

namespace AWSLambdaTwo.Services
{
    public interface IEmailService
    {
        void Send(string to, string subject, string html, string from = null);
    }

    public class EmailService //: IEmailService
    {
        //private readonly AppSettings _appSettings;

        //public EmailService(IOptions<AppSettings> appSettings)
        //{
        //    _appSettings = appSettings.Value;
        //}

        public EmailService()
        {
            //_appSettings = appSettings.Value;
        }

        public void Send(string to, string subject, string html, string from = null)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? "john.mathias3@gmail.com"));
            email.To.Add(MailboxAddress.Parse("john.mathias3@gmail.com"));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect("email-smtp.us-east-1.amazonaws.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate("AKIA43P37QJQCBTKKE5H", "BB7USYb50Ectpw8aR0Z5+cTPWznJD0dodRFvz7CqidMp");
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
