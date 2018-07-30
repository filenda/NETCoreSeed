using System.Threading.Tasks;

namespace NETCoreSeed.Shared.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(
                    string to,
                    string from,
                    string subject,
                    string plainTextMessage,
                    string htmlMessage,
                    string replyTo = null);

        Task SendMultipleEmailAsync(
                  string toCsv,
                  string from,
                  string subject,
                  string plainTextMessage,
                  string htmlMessage);
    }
}