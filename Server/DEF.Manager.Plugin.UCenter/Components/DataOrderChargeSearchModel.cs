using System;
using System.Collections.Generic;
using BootstrapBlazor.Components;

namespace DEF.Manager.Components;

public class DataOrderChargeSearchModel : ITableSearchModel
{
    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public ulong AgentId { get; set; }

    public IEnumerable<IFilterAction> GetSearches()
    {
        var ret = new List<IFilterAction>();
        if (Start > DateTime.MinValue)
        {
            ret.Add(new SearchFilterAction("Start", Start.Date, FilterAction.GreaterThanOrEqual));
        }
        if (End > DateTime.MinValue)
        {
            ret.Add(new SearchFilterAction("End", End.Date.AddDays(1).AddSeconds(-1), FilterAction.LessThanOrEqual));
        }
        if (AgentId > 0)
        {
            ret.Add(new SearchFilterAction("AgentId", AgentId, FilterAction.Equal));
        }
        return ret;
    }

    public void Reset()
    {
        Start = DateTime.MinValue;
        End = DateTime.MinValue;

        AgentId = 0;
    }
}