namespace LegacyRenewalApp;
//TO DO: niech to będzie standard normalizer który implementuje interfejs INormalize
public class Normalizer
{
    public static string normalize(string word)
    {
        return word.Trim().ToUpperInvariant();
    }
}