using System;

namespace LegacyRenewalApp;
//TO DO: niech to implementuje interface rounder ig
public class StandardRounder
{
    public static decimal round(decimal num)
    {// ustandaryzowanie zaokrąglania
        return Math.Round(num, 2, MidpointRounding.AwayFromZero);
    }
}