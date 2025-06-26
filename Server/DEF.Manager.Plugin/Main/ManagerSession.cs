using Microsoft.AspNetCore.Components.Authorization;

namespace DEF.Manager;

public class ManagerSession
{
    public string CurrentServiceName { get; set; }
    public List<MenuItem> NavMenuItems { get; set; } = [];
    ManagerContext ManagerContext { get; set; }
    ManagerDb Db { get; set; }
    public int TimezoneOffset { get; set; }
    public User User { get; set; }

    public ManagerSession(ManagerContext manager_context, ManagerDb db)
    {
        ManagerContext = manager_context;
        Db = db;

        //NewGeneralNavMenus();
    }

    public async Task<string> OnSwitchService(string service_name, Task<AuthenticationState> authentication_state)
    {
        if (CurrentServiceName == service_name) return CurrentServiceName;
        CurrentServiceName = service_name;

        NavMenuItems.Clear();

        var state = await authentication_state;
        var user_name = state.User.Identity?.Name;
        var user = await Db.GetUserByUserName(user_name, false);

        string default_url = string.Empty;

        if (service_name == "__general")
        {
            NewGeneralNavMenus(user);

            return default_url;
        }

        var plugin_info = ManagerContext.GetPluginInfo(service_name);
        if (plugin_info == null)
        {
            NewGeneralNavMenus(user);

            return default_url;
        }

        foreach (var i in plugin_info.NavMenuItems)
        {
            if (string.IsNullOrEmpty(default_url))
            {
                default_url = i.Url;
            }

            RoleType min_role = (RoleType)Enum.Parse(typeof(RoleType), i.MinRole);

            if (user.Role >= min_role)
            {
                MenuItem item = new()
                {
                    Text = i.Text,
                    Url = i.Url,
                    Icon = i.Icon,
                };
                NavMenuItems.Add(item);

                //if (i.Children.Count > 0)
                //{
                //    foreach (var j in i.Children)
                //    {
                //        RoleType min_role2 = (RoleType)Enum.Parse(typeof(RoleType), j.MinRole);
                //        if (user.Role >= min_role2)
                //        {
                //            MenuItem item2 = new()
                //            {
                //                Text = j.Text,
                //                Url = j.Url,
                //                Icon = j.Icon,
                //            };
                //            item.Items.Append(item2);
                //        }
                //    }
                //}
            }
        }

        return default_url;
    }

    void NewGeneralNavMenus(User current_user)
    {
        NavMenuItems.Clear();

        {
            MenuItem item = new()
            {
                Text = "首页",
                Url = "/",
                Icon = "oi oi-home",
            };
            NavMenuItems.Add(item);
        }

        if (current_user.Role >= RoleType.Agent)
        {
            MenuItem item = new()
            {
                Text = "账号列表",
                Url = "/userlist",
                Icon = "oi oi-home",
            };
            if (current_user.Role >= RoleType.Platform)
            {
                NavMenuItems.Add(item);
            }
            else if (current_user.Role == RoleType.Agent)
            {
                if (current_user.AgentParents == null || current_user.AgentParents.Count() < 2)
                {
                    NavMenuItems.Add(item);
                }
            }
        }

        if (current_user.Role >= RoleType.Agent)
        {
            MenuItem item = new()
            {
                Text = "日志管理",
                Url = "/logmgr",
                Icon = "oi oi-home",
            };
            if (current_user.Role >= RoleType.Platform)
            {
                NavMenuItems.Add(item);
            }
            else if (current_user.Role == RoleType.Agent)
            {
                if (current_user.AgentParents == null || current_user.AgentParents.Count() < 2)
                {
                    NavMenuItems.Add(item);
                }
            }
        }

        //{
        //    MenuItem item = new()
        //    {
        //        Text = "集群状态",
        //        Url = "/clusterstatus",
        //        Icon = "oi oi-home",
        //    };
        //    NavMenuItems.Add(item);
        //}

        //{
        //    MenuItem item = new()
        //    {
        //        Text = "用户管理",
        //        Url = "/usermgr",
        //        Icon = "oi oi-home",
        //    };
        //    NavMenuItems.Add(item);
        //}
    }
}