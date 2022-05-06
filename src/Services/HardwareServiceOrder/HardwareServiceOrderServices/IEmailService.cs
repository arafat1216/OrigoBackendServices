namespace HardwareServiceOrderServices
{
    public interface IEmailService
    {
        Task SendAsync(string subject,string to, string type, object data);
    }
}
