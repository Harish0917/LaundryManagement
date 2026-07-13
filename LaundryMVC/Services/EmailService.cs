using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LaundryMVC.Services
{
    public class EmailService
    {
        public async Task SendInvoiceEmail(string toEmail, string subject, string body, byte[] pdfBytes)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress("Laundry App", "harishharish68305@gmail.com"));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = body;

            
            builder.Attachments.Add("Invoice.pdf", pdfBytes);

            email.Body = builder.ToMessageBody();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync("harishharish68305@gmail.com", "phagpoenkndsgoiw");
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
