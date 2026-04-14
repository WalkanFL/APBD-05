namespace LegacyRenewalApp;

public interface IEmailer
{
    public void SendEmail(string email, string subject, string body)
    {
    }
}