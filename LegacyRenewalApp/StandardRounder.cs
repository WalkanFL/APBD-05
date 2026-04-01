using System;

namespace LegacyRenewalApp;

public class StandardRounder
{
    public static decimal round(decimal num)
    {// ustandaryzowanie zaokrąglania
        return Math.Round(num, 2, MidpointRounding.AwayFromZero);
    }
}