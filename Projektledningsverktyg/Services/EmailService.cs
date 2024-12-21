using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Projektledningsverktyg.Services
{
    public class EmailService
    {
        private readonly string _smtpServer = "localhost";
        private readonly int _smtpPort = 1025; // MailHog's default SMTP port

        public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
        {
            using (var message = new MailMessage())
            {
                message.From = new MailAddress("noreply@yourapp.com");
                message.To.Add(toEmail);
                message.Subject = "Återställ ditt lösenord";
                message.Body = CreateEmailBody(resetToken);
                message.IsBodyHtml = true;

                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    // MailHog doesn't need credentials or SSL
                    client.UseDefaultCredentials = true;
                    await client.SendMailAsync(message);
                }
            }
        }

        private string CreateEmailBody(string resetToken)
        {
            return $@"
                <html>
                    <body>
                        <h2>Återställ ditt lösenord</h2>
                        <p>Klicka på länken nedan för att återställa ditt lösenord:</p>
                        <a href='projektverktyg://reset?token={resetToken}'>Återställ lösenord</a>
                        <p>Länken är giltig i 24 timmar.</p>
                    </body>
                </html>";
        }


    }
}
