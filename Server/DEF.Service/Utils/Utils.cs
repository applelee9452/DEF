using System.Globalization;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using CommandLine;
using Microsoft.Extensions.Hosting;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DEF;

public static class Utils
{
    // 获取DEF.json所在目录
    public static string GetConfigPath()
    {
        string dir_prefix = "../";
        string filename1 = "Config/DEF.json";
        string path = AppDomain.CurrentDomain.BaseDirectory;
        string[] args = Environment.GetCommandLineArgs();

        CmdlineOptions cmd_options = null;
        var cmdline_parseresult = Parser.Default.ParseArguments<CmdlineOptions>(args);
        if (cmdline_parseresult.Tag == ParserResultType.Parsed)
        {
            var parsed = (Parsed<CmdlineOptions>)cmdline_parseresult;
            cmd_options = parsed.Value;
        }

        if (cmd_options != null && !string.IsNullOrEmpty(cmd_options.SolutionDir))
        {
            path = cmd_options.SolutionDir;
        }

        for (int i = 0; i < 8; i++)
        {
            string p1 = Path.Combine(path, filename1);
            string p2 = Path.GetFullPath(p1);

            if (File.Exists(p2))
            {
                return p2.Replace("DEF.json", "");
            }

            filename1 = dir_prefix + filename1;
        }

        return path;
    }

    // 获取本地内网ip地址
    public static IPAddress GetLocalIpAddress(string local_ip_prefix)
    {
        string host_name = Dns.GetHostName();                    // 获取主机名称  
        IPAddress[] addresses = Dns.GetHostAddresses(host_name); // 解析主机IP地址  

        for (int i = 0; i < addresses.Length; i++)
        {
            string ip = addresses[i].ToString();
            if (ip.StartsWith(local_ip_prefix))
            {
                return addresses[i];
            }
        }

        return IPAddress.Loopback;
    }

    // 获得13位的时间戳
    public static string GetTimeStamp()
    {
        DateTime time = DateTime.UtcNow;
        long ts = ConvertDateTimeToInt(time);
        return ts.ToString();
    }

    // 将c# DateTime时间格式转换为Unix时间戳格式  
    public static long ConvertDateTimeToInt(DateTime time)
    {
        long unixTimestamp = time.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;
        unixTimestamp /= TimeSpan.TicksPerMillisecond;
        return unixTimestamp;
    }

    // 生成签名所需的字符
    public static string GenSignOrigin(Dictionary<string, string> request)
    {
        StringBuilder signOrigin = new();

        foreach (var item in request.OrderBy(a => a.Key))
        {
            if (item.Key.ToLower() == "sign")// sign不参与计算
            {
                continue;
            }
            if (item.Value == string.Empty) continue;// 参数值为空，不参与计算
            if (signOrigin.Length > 0) signOrigin.Append('&');

            signOrigin.Append(item.Key);
            signOrigin.Append('=');
            signOrigin.Append(item.Value);
        }
        return signOrigin.ToString();
    }

    // 生成Url所需的字符串
    public static string GenUrlStrings(Dictionary<string, string> request)
    {
        StringBuilder signOrigin = new();

        foreach (var item in request)
        {
            if (signOrigin.Length > 0) signOrigin.Append('&');

            signOrigin.Append(item.Key);
            signOrigin.Append('=');
            signOrigin.Append(item.Value);
        }
        return signOrigin.ToString();
    }

    // 本周的周日判定
    public static DateTime GetSundayOfThisWeek()
    {
        DateTime sunday_of_this_week = DateTime.UtcNow;

        int today_of_week = (int)DateTime.UtcNow.DayOfWeek;
        if (today_of_week != (int)DayOfWeek.Sunday)
        {
            int delta_day = 7 - today_of_week;
            sunday_of_this_week = sunday_of_this_week.AddDays(delta_day);
        }

        return sunday_of_this_week;
    }

    // 是否是同一天，判定年月日是否相同
    public static bool IsSameDay(DateTime dt, DateTime dt_other)
    {
        if (dt.Year == dt_other.Year
            && dt.Month == dt_other.Month
            && dt.Day == dt_other.Day)
        {
            return true;
        }

        return false;
    }

    // 获取本周开始和结束时间点，从周一开始，返回Utc时间
    public static (DateTime, DateTime) GetCurrentWeekDtRange()
    {
        DateTime today_local = DateTime.Today;

        // 计算本周开始时间（周一00:00:00）
        int diff = (today_local.DayOfWeek - DayOfWeek.Monday + 7) % 7;
        DateTime start_of_week_local = today_local.AddDays(-diff).Date;

        // 计算本周结束时间（周日23:59:59.999）
        DateTime end_of_week_local = start_of_week_local.AddDays(7).AddTicks(-1);

        DateTime star_of_week_utc = start_of_week_local.ToUniversalTime();
        DateTime end_of_week_utc = end_of_week_local.ToUniversalTime();

        return (star_of_week_utc, end_of_week_utc);
    }

    // 获取上周开始和结束时间点，从周一开始，返回Utc时间
    public static (DateTime, DateTime) GetLastWeekDtRange()
    {
        DateTime today = DateTime.Today;

        // 计算本周开始时间（周一00:00:00）
        int diff = (today.DayOfWeek - DayOfWeek.Monday + 7) % 7;
        DateTime startOfWeek = today.AddDays(-diff).Date;

        // 计算上周开始时间（周一00:00:00）
        DateTime startOfLastWeek = startOfWeek.AddDays(-7);

        // 计算上周结束时间（周日23:59:59.999）
        DateTime endOfLastWeek = startOfWeek.AddTicks(-1);

        DateTime star_of_week_utc = startOfLastWeek.ToUniversalTime();
        DateTime end_of_week_utc = endOfLastWeek.ToUniversalTime();

        return (star_of_week_utc, end_of_week_utc);

    }

    // 获取本月开始和结束时间点，返回Utc时间
    public static (DateTime, DateTime) GetCurrentMonthDtRange()
    {
        DateTime today_local = DateTime.Today;

        // 本月开始时间（第一天00:00:00）
        DateTime start_of_month_local = new DateTime(today_local.Year, today_local.Month, 1);

        // 本月结束时间（最后一天23:59:59.999）
        DateTime end_of_month_local = start_of_month_local.AddMonths(1).AddTicks(-1);

        DateTime star_of_month_utc = start_of_month_local.ToUniversalTime();
        DateTime end_of_month_utc = end_of_month_local.ToUniversalTime();

        return (star_of_month_utc, end_of_month_utc);
    }

    // 获取上月开始和结束时间点，返回Utc时间
    public static (DateTime, DateTime) GetLastMonthDtRange()
    {
        DateTime today = DateTime.Today;

        // 本月的第一天00:00:00
        DateTime startOfThisMonth = new DateTime(today.Year, today.Month, 1);

        // 上月开始时间（第一天00:00:00）
        DateTime startOfLastMonth = startOfThisMonth.AddMonths(-1);

        // 上月结束时间（最后一天23:59:59.999）
        DateTime endOfLastMonth = startOfThisMonth.AddTicks(-1);

        DateTime star_of_month_utc = startOfLastMonth.ToUniversalTime();
        DateTime end_of_month_utc = endOfLastMonth.ToUniversalTime();

        return (star_of_month_utc, end_of_month_utc);
    }

    //  判定2个日期是否属于同一周，每周的第一天是周一
    public static bool IsSameWeek(DateTime dt1, DateTime dt2, DayOfWeek start_of_week)
    {
        DateTime start1 = GetStartOfWeek(dt1, start_of_week);
        DateTime start2 = GetStartOfWeek(dt2, start_of_week);
        return start1 == start2;
    }

    // 判定2个日期是否属于同一周，使用当前文化的周起始日
    public static bool IsSameWeek(DateTime date1, DateTime date2)
    {
        CultureInfo culture = CultureInfo.CurrentCulture;
        DayOfWeek startOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
        return IsSameWeek(date1, date2, startOfWeek);
    }

    // 获取指定日期所在周的周起始日
    static DateTime GetStartOfWeek(DateTime date, DayOfWeek start_of_week)
    {
        int diff = (7 + (date.DayOfWeek - start_of_week)) % 7;
        return date.AddDays(-diff).Date;
    }

    // 查找可用的端口
    public static int FindNextAvailableTCPPort(int start_port)
    {
        int port = start_port;
        bool is_available = true;

        const string PortReleaseGuid = "8875BD8E-4D5B-11DE-B2F4-691756D89593";
        var mutex = new Mutex(false, string.Concat("Global/", PortReleaseGuid));
        mutex.WaitOne();
        try
        {
            IPGlobalProperties ip_global_properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] end_points = ip_global_properties.GetActiveTcpListeners();

            do
            {
                if (!is_available)
                {
                    port++;
                    is_available = true;
                }

                foreach (IPEndPoint end_point in end_points)
                {
                    if (end_point.Port != port) continue;
                    is_available = false;
                    break;
                }

            } while (!is_available && port < IPEndPoint.MaxPort);

            if (!is_available)
            {
                throw new ApplicationException("Not able to find a free TCP port.");
            }

            return port;
        }
        finally
        {
            mutex.ReleaseMutex();
        }
    }
}