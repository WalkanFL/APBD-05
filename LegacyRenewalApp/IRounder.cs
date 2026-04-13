using System;

namespace LegacyRenewalApp;

public interface IRounder //idea że mogą być inne sposoby na rounding a chcemy to jeszcze bardziej ustandaryzować
{
    public static decimal round(decimal num)
    {
        return Decimal.Round(num);
    }
}