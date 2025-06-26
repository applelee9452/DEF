using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;

namespace DEF.Manager.Pages;

// Login 页面
public partial class Login
{
    [Inject]
    private IJSRuntime JSRuntime { get; set; }// 获得/设置 IJSRuntime 实例

    [Inject]
    public NavigationManager NavigationManager { get; set; }

    [SupplyParameterFromQuery]
    public string ReturnUrl { get; set; }

    JSModule _module;

    readonly LoginModel _model = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            _module = await JSRuntime.LoadModule("./Pages/Login.razor.js");
        }
    }

    private async Task OnLogin(EditContext context)
    {
        if (_module != null)
        {
            var result = await _module.InvokeAsync<LoginResult>("Login", _model.UserName, _model.Password, _model.RememberMe);
            if (result is { Code: 0 })
            {
                NavigationManager.NavigateTo(ReturnUrl ?? "/", true);
            }
        }
    }
}