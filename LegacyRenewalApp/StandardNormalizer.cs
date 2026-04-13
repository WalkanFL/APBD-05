namespace LegacyRenewalApp;

public class StandardNormalizer : INormalize
{
    public static string normalize(string word)
    {
        return word.Trim().ToUpperInvariant();
    }
}