using DEF.CustomerSvc.Data;
using DEF.CustomerSvc.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEF.CustomerSvc.Pages;

public partial class Index : ComponentBase
{
    [Inject] private IJSRuntime Js { get; set; }
    [Inject] private MessageService MessageService { get; set; }

    private IList<Message> _messageList;
    private string _typingValue;
    private string _inputValue;

    private ElementReference windowRef;
    private ElementReference inputRef;
    private ElementReference pc_fileRef;
    private ElementReference mobile_fileRef;

    private bool _waitForScrollBottom;

    private string currentUser = "James";
    private string clerk = "客服001";
    private string uploadUrl = "/api/file";

    protected override Task OnInitializedAsync()
    {
        var history = MessageService.FetchMessages();
        _messageList = history.ToList();

        _waitForScrollBottom = true;
        return base.OnInitializedAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);
        if (firstRender)
        {
            await Js.InvokeVoidAsync("window.blazor_chart.blurAndFocus", inputRef);
            await Js.InvokeVoidAsync("window.blazor_chart.setupfileInput", pc_fileRef, uploadUrl, DotNetObjectReference.Create(this));
            await Js.InvokeVoidAsync("window.blazor_chart.setupfileInput", mobile_fileRef, uploadUrl, DotNetObjectReference.Create(this));
        }

        if (_waitForScrollBottom)
        {
            await Js.InvokeVoidAsync("window.blazor_chart.scrollToBottom", windowRef);
        }
    }

    private async Task KeyDown(KeyboardEventArgs args)
    {
        if (args.CtrlKey && args.Key == "Enter")
        {
            _typingValue = _inputValue;
            _inputValue = "";

            Send();

            await Js.InvokeVoidAsync("window.blazor_chart.blurAndFocus", inputRef);
        }
    }

    private void OnInput(ChangeEventArgs args)
    {
        _inputValue = args.Value as string;
    }

    private void Send()
    {
        if (string.IsNullOrWhiteSpace(_typingValue))
        {
            _typingValue = "";
            return;
        }

        AddMessage(currentUser, _typingValue);
        InvokeAsync(() => Reply(_typingValue));
        _typingValue = "";
    }

    private async Task Reply(string value)
    {
        var replyValue = value;
        if (replyValue.Contains("？") || replyValue.Contains("?"))
        {
            replyValue = replyValue.Replace("？", "！").Replace("?", "！");
        }

        if (value.Contains("吗") || value.Contains("?"))
        {
            replyValue = replyValue.Replace("吗", "");
        }

        await Task.Delay(2000);
        AddMessage(clerk, replyValue);
    }

    private void AddMessage(string user, string message, MessageType type = MessageType.Text)
    {
        _messageList.Add(new Message
        {
            UserName = user,
            Body = message.Trim(' '),
            Time = DateTime.Now,
            Type = type,
        });

        _waitForScrollBottom = true;
        StateHasChanged();
    }

    [JSInvokable]
    public async Task FileUploadCompleted(string filePath)
    {
        AddMessage(currentUser, filePath, MessageType.Image);
        await Reply("真好看！");
    }
}