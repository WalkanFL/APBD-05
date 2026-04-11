using System.Collections.Generic;

namespace LegacyRenewalApp;

public interface ITax
{
    public static readonly Dictionary<string, (decimal, string)> payTypeFeeDict = new Dictionary<string, (decimal, string)>
    {
        { "CARD", (0.02m, "card payment fee; ") },
        { "BANK_TRANSFER", (0.01m, "bank transfer fee; ") },
        { "PAYPAL", (0.035m, "paypal fee; ") },
        { "INVOICE", (0m, "invoice payment; ") },
    };

    public static readonly Dictionary<string, decimal> countryFeeDict = new Dictionary<string, decimal>
    {
        {"Poland", 0.23m},
        {"Germany", 0.19m},
        {"Czech Republic", 0.21m},
        {"Norway", 0.25m}
    };
    
    public void processTax();
    public string getNotes();
    
    public decimal getPaymentFee();
    public decimal getTaxAmount();

}