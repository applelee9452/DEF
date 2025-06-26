using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;

namespace DEF.Manager.Components;

public partial class DataOrderChargeSearch
{
    [Parameter]
    public DataOrderChargeSearchModel Model { get; set; }

    [Parameter]
    public EventCallback<DataOrderChargeSearchModel> ModelChanged { get; set; }

    [Parameter]
    public Func<Task> OnSearchCallback { get; set; }

    [Inject]
    private ManagerSession ManagerSession { get; set; }

    private List<SelectedItem> Items { get; } = [
        new SelectedItem("0", "È«²¿"),
        new SelectedItem("1001", "1001"),
        new SelectedItem("1002", "1002"),
        new SelectedItem("1003", "1003"),
        new SelectedItem("1004", "1004")
    ];

    private async Task TriggerSearch(int offset)
    {
        Model.Start = DateTime.Today.AddDays(offset).AddHours(ManagerSession.TimezoneOffset);
        Model.End = DateTime.Today.AddDays(offset).AddHours(ManagerSession.TimezoneOffset);

        if (OnSearchCallback != null)
        {
            await OnSearchCallback();
        }
    }
}