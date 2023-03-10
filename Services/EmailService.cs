using Hoist.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Hoist.Services
{
    public class EmailService : IEmailSender
    {

        private readonly MailSettings _mailSettings; //This class gathers the object in our configuration to pass it around easier

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {




            var emailFrom = email;//Will look on the environment variables to find the email address //Using a var is when you don;t know the types being sent to you. This is implicit 
            var emailSender = MailboxAddress.Parse(_mailSettings.EmailAddress ?? Environment.GetEnvironmentVariable("EmailAddress")).ToString();


            MimeMessage newEmail = new MimeMessage();

            if (subject == "Confirm your email" && htmlMessage.Contains("Please confirm your account"))
            {
                newEmail.To.Add(MailboxAddress.Parse(email));
            }
            else
            {
                newEmail.To.Add(MailboxAddress.Parse(_mailSettings.EmailAddress ?? Environment.GetEnvironmentVariable("EmailAddress")));
            }


            newEmail.Sender = MailboxAddress.Parse(_mailSettings.EmailAddress ?? Environment.GetEnvironmentVariable("EmailAddress"));

            newEmail.ReplyTo.Add(MailboxAddress.Parse(emailFrom));
            newEmail.Subject = subject;

            BodyBuilder emailBody = new BodyBuilder();
            emailBody.HtmlBody = htmlMessage;


            newEmail.Body = emailBody.ToMessageBody();

            using SmtpClient smtpClient = new SmtpClient();

            try
            {
                var host = _mailSettings.EmailHost ?? Environment.GetEnvironmentVariable("EmailHost");
                var port = _mailSettings.EmailPort != 0 ? _mailSettings.EmailPort : int.Parse(Environment.GetEnvironmentVariable("EmailPort")!);
                var password = _mailSettings.EmailPassword ?? Environment.GetEnvironmentVariable("EmailPassword");

                await smtpClient.ConnectAsync(host, port, SecureSocketOptions.StartTls);
                await smtpClient.AuthenticateAsync(emailSender, password);

                await smtpClient.SendAsync(newEmail);
                await smtpClient.DisconnectAsync(true);
            }
            catch (Exception ex)
            {

                var error = ex.Message; //Not good for production, good for deducing errors and stuff.
                throw;
            }
        }
    }
}
