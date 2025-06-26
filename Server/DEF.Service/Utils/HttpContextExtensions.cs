using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace DEF;

public static class HttpContextExtensions
{
    public static string GetClientIpAddress(this HttpContext httpContext, bool tryUseXForwardHeader = true)
    {
        // Modify based on http://stackoverflow.com/questions/28664686/how-do-i-get-client-ip-address-in-asp-net-core
        // To make it 
        string ip = string.Empty;

        // todo support new "Forwarded" header (2014) https://en.wikipedia.org/wiki/X-Forwarded-For

        if (tryUseXForwardHeader)
        {
            ip = GetHeaderValueAs<string>(httpContext, "X-Forwarded-For").SplitCsv().FirstOrDefault();
        }

        if (string.IsNullOrWhiteSpace(ip) && httpContext?.Connection?.RemoteIpAddress != null)
        {
            ip = httpContext.Connection.RemoteIpAddress.ToString();
        }

        if (string.IsNullOrWhiteSpace(ip))
        {
            ip = GetHeaderValueAs<string>(httpContext, "REMOTE_ADDR");
        }

        return ip;
    }

    private static T GetHeaderValueAs<T>(HttpContext httpContext, string headerName)
    {
        StringValues values = StringValues.Empty;

        if (httpContext?.Request?.Headers?.TryGetValue(headerName, out values) ?? false)
        {
            string rawValues = values.ToString();   // writes out as Csv when there are multiple.

            if (!string.IsNullOrEmpty(rawValues))
            {
                return (T)Convert.ChangeType(values.ToString(), typeof(T));
            }
        }
        return default(T);
    }

    private static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
    {
        if (string.IsNullOrWhiteSpace(csvList))
            return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

        return csvList
            .TrimEnd(',')
            .Split(',')
            .AsEnumerable<string>()
            .Select(s => s.Trim())
            .ToList();
    }
}