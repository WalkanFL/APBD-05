namespace LegacyRenewalApp;

public interface IDiscount
{
    public decimal processDiscount();
    public string getNotes();
    public decimal getDiscountAmount();
}