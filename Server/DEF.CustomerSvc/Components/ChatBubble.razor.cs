using DEF.CustomerSvc.Data;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DEF.CustomerSvc.Components;

public partial class ChatBubble : ComponentBase
{
    [Parameter]
    public Message Message { get; set; }

    [Parameter]
    public string CurrentUser { get; set; }

    private bool IsCurrentUser => Message.UserName == CurrentUser;
}