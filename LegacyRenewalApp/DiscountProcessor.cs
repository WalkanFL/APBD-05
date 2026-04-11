using System;

namespace LegacyRenewalApp;

public class DiscountProcessor : IDiscount
{
    private static decimal minimumSubtotal = 300m;
    
    private decimal baseAmount;
    public decimal discountAmount { get; private set; }

    private Customer customer;
    private SubscriptionPlan plan;
    private bool usePoints;
    private int seatCount;

    private string notes { get; set; }

    public DiscountProcessor(Customer customer, SubscriptionPlan plan, decimal baseAmount, int seatCount, bool usePoints)
    {
        this.customer = customer;
        this.plan = plan;
        
        this.baseAmount = baseAmount;
        this.seatCount = seatCount;
        this.usePoints = usePoints;

        discountAmount = 0m;
        notes = string.Empty;

    }

    public decimal processDiscount()
    {
        //składa się z procesów rozłożonych na kroki
        processTier();
        processTime();
        processSeats();
        processPoints();
        //
        
        decimal subtotalAfterDiscount = baseAmount - discountAmount;
        if (subtotalAfterDiscount < minimumSubtotal)
        {
            subtotalAfterDiscount = minimumSubtotal;
            notes += "minimum discounted subtotal applied; ";
        }
        
        return subtotalAfterDiscount;
    }

    public string getNotes()
    {
        return notes;
    }

    public decimal getDiscountAmount()
    {
        return discountAmount;
    }

    //discount for customer tier calc
    private void processTier()
    {
        decimal discountVariable = 0m;
            
            
        switch (customer.Segment)
        {
            case "Silver":
                discountVariable = 0.05m;
                notes += "silver discount; ";
                break;
            case "Gold":
                discountVariable = 0.10m;
                notes += "gold discount; ";
                break;
            case "Platinum":
                discountVariable = 0.15m;
                notes += "platinum discount; ";
                break;
            case "Education":
                if (plan.IsEducationEligible)
                {
                    discountVariable = 0.20m;
                    notes += "education discount; ";
                }
                break;
        }
            
        discountAmount += baseAmount * discountVariable;
        /*
        if (customer.Segment == "Silver")
        {
            discountAmount += baseAmount * 0.05m;
            notes += "silver discount; ";
        }
        else if (customer.Segment == "Gold")
        {
            discountAmount += baseAmount * 0.10m;
            notes += "gold discount; ";
        }
        else if (customer.Segment == "Platinum")
        {
            discountAmount += baseAmount * 0.15m;
            notes += "platinum discount; ";
        }
        else if (customer.Segment == "Education" && plan.IsEducationEligible)
        {
            discountAmount += baseAmount * 0.20m;
            notes += "education discount; ";
        }*/
    }
    //discount for customer loyalty time
    private void processTime()
    {
        if (customer.YearsWithCompany >= 5)
        {
            discountAmount += baseAmount * 0.07m;
            notes += "long-term loyalty discount; ";
        }
        else if (customer.YearsWithCompany >= 2)
        {
            discountAmount += baseAmount * 0.03m;
            notes += "basic loyalty discount; ";
        }


    }

    //discount for seats
    private void processSeats()
    {
        if (seatCount >= 50)
        {
            discountAmount += baseAmount * 0.12m;
            notes += "large team discount; ";
        }
        else if (seatCount >= 20)
        {
            discountAmount += baseAmount * 0.08m;
            notes += "medium team discount; ";
        }
        else if (seatCount >= 10)
        {
            discountAmount += baseAmount * 0.04m;
            notes += "small team discount; ";
        }
    }

    //discount for customer point calc
    private void processPoints()
    {
        if (usePoints && customer.LoyaltyPoints > 0)
        {
            int pointsToUse = customer.LoyaltyPoints > 200 ? 200 : customer.LoyaltyPoints;
            discountAmount += pointsToUse;
            notes += $"loyalty points used: {pointsToUse}; ";
        }
    }
    
}