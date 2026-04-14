namespace LegacyRenewalApp;

public class BillingInvoiceSaverAdapter : IInvoiceSaver
{
    public void SaveInvoice(RenewalInvoice invoice)
    {
        LegacyBillingGateway.SaveInvoice(invoice);
    }
}