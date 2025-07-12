using System.Threading.Tasks;

namespace Email_notification.Models
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(EmailRequest emailRequest);
    }
}
