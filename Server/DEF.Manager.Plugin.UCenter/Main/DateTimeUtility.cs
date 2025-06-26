using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BootstrapBlazor.Components;

namespace DEF.UCenter;

public static class DateTimeUtility
{
    public enum SplitType
    {
        Day,
        Week,
        Month,
        Hour,
    }

    public class Segmentation(DateTime start, DateTime end, DateTimeUtility.SplitType type)
    {
        public DateTime Start { get; set; } = start.ToUniversalTime();
        public DateTime End { get; set; } = end.ToUniversalTime();
        SplitType Type { get; set; } = type;
        string Title { get; set; } = GetTitle(start, end, type);

        public bool IsInRange(DateTime dt)
        {
            return dt >= Start && dt <= End;
        }

        public bool BeforeEnd(DateTime dt)
        {
            return dt <= End;
        }

        private static string GetTitle(DateTime start, DateTime end, SplitType sptype)
        {
            string format = "yyyy年MM月dd日";
            switch (sptype)
            {
                case SplitType.Day:
                    return start.ToString(format);
                case SplitType.Hour:
                    return start.ToLocalTime().ToString($"{format}HH时");
            }
            if (start.Date == end.Date)
            {
                return start.ToString(format);
            }
            return $"{start.ToString(format)}-{end.ToString(format)}";
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class SegmentationList
    {
        public List<Segmentation> List { get; private set; } = [];
        public DateTime StartUtc { get; private set; }
        public DateTime EndUtc { get; private set; }
        public List<DateTime> BucketsDates { get; private set; }
        public int Count => List.Count;
        public SplitType SplitType;
        public IEnumerable<long> Ticks { get; private set; }

        public SegmentationList(DateTime start, DateTime end, SplitType st)
        {
            Func<DateTime, DateTime> d = st == SplitType.Hour ? dt => dt.AddHours(1) : st == SplitType.Day ? dt => dt.AddDays(1) : st == SplitType.Week ? dt => dt.AddDays(7) : dt => dt.AddMonths(1);
            var seg_start = start;
            while (seg_start < end)
            {
                var temp = d(seg_start);
                var seg_end = temp.AddMicroseconds(-1);
                if (seg_end > end)
                {
                    seg_end = end;
                }
                List.Add(new Segmentation(seg_start, seg_end, st));
                seg_start = temp;
            }
            BucketsDates = [List[0].Start, .. List.Select((t, i) => t.End)];
            Ticks = List.Select((t, i) => t.End.Ticks);
            StartUtc = start.ToUniversalTime();
            EndUtc = end.ToUniversalTime();
            SplitType = st;
        }

        public async Task<List<object>> ForeachSegmentation(Func<Segmentation, Task<object>> action)
        {
            List<object> ls = [];

            for (int i = 0; i < List.Count; ++i)
            {
                var seg = List[i];
                var res = await action(seg);
                ls.Add(res);
            }
            return ls;
        }

        public int Format(DateTime dt)
        {
            var f = List.FindIndex(t => t.IsInRange(dt));
            return f;
        }

        public async Task Foreach(Func<Segmentation, int, Task> action)
        {
            //List<object> ls = [];

            for (int i = 0; i < List.Count; ++i)
            {
                var seg = List[i];
                await action(seg, i);
            }
        }

        public async Task<List<string>> GetLabelsAsync()
        {
            List<string> days = [];

            await Foreach((seg, i) =>
            {
                days.Add(seg.ToString());
                return Task.CompletedTask;
            });

            return days;
        }
    }

    public static DateTime DefaultStart => DateTime.Today.AddDays(-6);
    public static DateTime DefaultEnd => DateTime.Today.AddDays(1).AddMicroseconds(-1);
    public static DateTimeRangeValue DefaultRangeValue => new() { Start = DefaultStart, End = DefaultEnd };

    public static SegmentationList GetSegmentation(DateTime start, DateTime end, SplitType st)
    {
        return new SegmentationList(start, end, st);
    }

    public static DateTimeRangeValue Today()
    {
        return new DateTimeRangeValue() { Start = DateTime.Today, End = DateTime.Today.AddDays(1).AddMicroseconds(-1) };
    }

    public static DateTimeRangeValue Yesterday()
    {
        return new DateTimeRangeValue() { Start = DateTime.Today.AddDays(-1), End = DateTime.Today.AddMilliseconds(-1) };
    }

    public static DateTimeRangeValue LastWeek()
    {
        return new DateTimeRangeValue() { Start = DateTime.Today.AddDays(-6), End = DateTime.Today.AddDays(1).AddMicroseconds(-1) };
    }

    public static DateTimeRangeValue LastMonth()
    {
        return new DateTimeRangeValue() { Start = DateTime.Today.AddDays(-30), End = DateTime.Today.AddDays(1).AddMicroseconds(-1) };
    }

    public static DateTimeRangeValue Format(DateTimeRangeValue v)
    {
        return new DateTimeRangeValue() { Start = v.Start.Date, End = v.End.Date.AddDays(1).AddMicroseconds(-1) };
    }
}