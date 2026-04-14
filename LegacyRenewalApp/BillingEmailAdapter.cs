namespace LegacyRenewalApp;

public class BillingEmailAdapter : IEmailer
{
    public void SendEmail(string email, string subject, string body)
    {
        LegacyBillingGateway.SendEmail(email, subject, body);
    }
}