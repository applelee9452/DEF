using System.Globalization;

namespace DEF.UCenter;

public static class StringExtensions
{
    public static string MapNullToEmpty(this string str)
    {
        return str ?? string.Empty;
    }

    public static string FormatInvariant(this string template, params object[] args)
    {
        return string.Format(CultureInfo.InvariantCulture, template, args);
    }

    public static string JoinToString(this IEnumerable<string> items, string separator)
    {
        return string.Join(separator, items);
    }

    public static string JoinToString<T>(this IEnumerable<T> items, string separator, Func<T, string> selector)
    {
        return string.Join(separator, items.Select(i => selector(i)));
    }

    public static string FirstCharacterToLower(this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
        {
            return str;
        }

        return char.ToLowerInvariant(str[0]) + str.Substring(1);
    }
}