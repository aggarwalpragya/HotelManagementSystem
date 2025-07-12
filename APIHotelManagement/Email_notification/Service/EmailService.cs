using MailKit.Net.Smtp;
using MimeKit;
using Email_notification.Models;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> SendEmailAsync(EmailRequest request)
    {
        try
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(
                _configuration["SmtpSettings:SenderName"],
                _configuration["SmtpSettings:SenderEmail"]));

            emailMessage.To.Add(new MailboxAddress("", request.ToEmail));
            emailMessage.Subject = request.Subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = request.Body };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            // SMTP Client setup and email sending

            using var client = new SmtpClient();
            await client.ConnectAsync(_configuration["SmtpSettings:Server"],
                                      int.Parse(_configuration["SmtpSettings:Port"]),
                                      MailKit.Security.SecureSocketOptions.StartTls);

            await client.AuthenticateAsync(_configuration["SmtpSettings:SenderEmail"],
                                           _configuration["SmtpSettings:Password"]);

            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);     // disconnection from smtp server

            return true; // Email sent successfully
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Email sending failed: {ex.Message}");
            return false;
        }
    }
}