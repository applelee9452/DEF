using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace DEF.Manager;

public class ManagerAuthenticationStateProvider(ManagerDb manager_db) : AuthenticationStateProvider, IHostEnvironmentAuthenticationStateProvider
{
    Task<AuthenticationState> AuthenticationStateTask { get; set; }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var state = await AuthenticationStateTask;

        var is_auth = state.User.Identity?.IsAuthenticated ?? false;

        // 认证逻辑
        if (is_auth)
        {
            var user_name = state.User.Identity?.Name;

            if (!string.IsNullOrEmpty(user_name))
            {
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                var role = await manager_db.GetRole(user_name);
                identity.AddClaim(new Claim(ClaimTypes.Role, role.ToString()));
                state.User.AddIdentity(identity);
            }
        }
        return state;
    }

    public void SetAuthenticationState(Task<AuthenticationState> authenticationStateTask)
    {
        AuthenticationStateTask = authenticationStateTask;

        NotifyAuthenticationStateChanged(AuthenticationStateTask);
    }
}