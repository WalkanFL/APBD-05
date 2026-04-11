using System;
using System.Collections.Generic;

namespace LegacyRenewalApp;

public class TaxProcessor : ITax
{
    private decimal baseCountryTax = 0.20m;
    
    private decimal initialPrice;
    private string normalizedPaymentMethod;
    private Customer customer;
    private string notes { get; set; }
    
    private decimal paymentMethodTax;
    private decimal countryTax;

    public TaxProcessor(decimal initialPrice, string normalizedPaymentMethod, Customer customer)
    {
        this.initialPrice = initialPrice;
        this.normalizedPaymentMethod = normalizedPaymentMethod;
        this.customer = customer;
        
        notes = string.Empty;
    }

    public void processTax()
    {
        //odpalamy metody które chcemy
        processPaymentMethodTax();
        processCountryTax();
        //
    }

    public string getNotes()
    {
        return notes;
    }

    public decimal getPaymentFee()
    {
        return paymentMethodTax;
    }

    public decimal getTaxAmount()
    {           
        return
            //previously known as taxBase
            initialPrice + paymentMethodTax
            *
            countryTax
            ;
    }

    private void processPaymentMethodTax()
    {
        decimal paymentFee = 0m;
        (decimal, string) payTypePackage = ITax.payTypeFeeDict.GetValueOrDefault(normalizedPaymentMethod);
        decimal paymentVariable = payTypePackage.Item1;

        notes += payTypePackage.Item2;
        
        paymentMethodTax = initialPrice * paymentVariable;
    }
    
    private void processCountryTax()
    {
        if (ITax.countryFeeDict.ContainsKey(customer.Country))
        {
            countryTax = ITax.countryFeeDict.GetValueOrDefault(customer.Country);
        }
        else
        {
            countryTax = baseCountryTax;
        }
    }
}