using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailConfiguration emailConfiguration;

        public EmailSender(EmailConfiguration emailConfiguration)
        {
            this.emailConfiguration = emailConfiguration;
        }

        public void SendEmail(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        public async Task SendEmailAsync(Message message)
        {
            var emailMessage = CreateEmailMessage(message);
            await SendAsync(emailMessage);
        }

        private MimeMessage CreateEmailMessage(Message message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(emailConfiguration.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            // If you want send any text content
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text) { Text = message.Content };
            // if you want send HTML content 
            //emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };
            var bodyBuilder = new BodyBuilder { HtmlBody = string.Format("<h2 style='color:red;'>{0}</h2>", message.Content) };

            if (message.Attachements != null && message.Attachements.Any())
            {
                byte[] fileBytes;
                foreach (var attachment in message.Attachements)
                {
                    using (var ms = new MemoryStream())
                    {
                        attachment.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                    bodyBuilder.Attachments.Add(attachment.FileName, fileBytes, ContentType.Parse(attachment.ContentType));
                }
            }
            emailMessage.Body = bodyBuilder.ToMessageBody();
            return emailMessage;
        }

        private void Send(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    client.Connect(emailConfiguration.SmtpServer, emailConfiguration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    client.Authenticate(emailConfiguration.UserName, emailConfiguration.Password);

                    client.Send(mailMessage);
                }
                catch 
                {

                    throw;
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        private async Task SendAsync(MimeMessage mailMessage)
        {
            using (var client = new SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(emailConfiguration.SmtpServer, emailConfiguration.Port, true);
                    client.AuthenticationMechanisms.Remove("XOAUTH2");
                    await client.AuthenticateAsync(emailConfiguration.UserName, emailConfiguration.Password);

                    client.Send(mailMessage);
                }
                catch
                {

                    throw;
                }
                finally
                {
                    await client.DisconnectAsync(true);
                    client.Dispose();
                }
            }
        }
    }
}
