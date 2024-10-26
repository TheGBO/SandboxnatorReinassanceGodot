namespace GBOUtils;

public static class StringUtils
{
    public static string TrimSuffix(this string s, string suffix)
    {
        if (s.EndsWith(suffix))
        {
            return s.Substring(0, s.Length - suffix.Length);
        }

        return s;
    }
}
