using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Web;

namespace DEF.Manager;

public class LoginModel
{
    [DataType(DataType.Text)]
    [Display(Name = "账号")]
    [Required(ErrorMessage = "{0}不能为空")]
    public string UserName { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "密码")]
    [Required(ErrorMessage = "{0}不能为空")]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}

public class LoginResult
{
    public int Code { get; set; }
}

[AllowAnonymous]
public class AccountController : Controller
{
    ManagerDb ManagerDb { get; set; }

    public AccountController(ManagerDb manager_db)
    {
        ManagerDb = manager_db;
    }

    [HttpGet]
    public IActionResult Login(string returnUrl)
    {
        return LocalRedirect($"/Login?ReturnUrl={HttpUtility.UrlEncode(returnUrl)}");
    }

    [HttpPost]
    public async Task<LoginResult> Login([FromBody] LoginModel model)
    {
        var ret = new LoginResult() { Code = 1001 };

        var user_name = model.UserName;
        var password = model.Password;
        if (string.IsNullOrEmpty(user_name) || string.IsNullOrEmpty(password))
        {
            return ret;
        }

        var auth = await ManagerDb.Authenticate(user_name, password);
        if (auth)
        {
            var persistent = model.RememberMe;

            await SignInAsync(user_name, "/", persistent);

            ret.Code = 0;
        }

        return ret;
    }

    async Task SignInAsync(string user_name, string return_url, bool persistent, string authentication_scheme = CookieAuthenticationDefaults.AuthenticationScheme)
    {
        var identity = new ClaimsIdentity(authentication_scheme);
        identity.AddClaim(new Claim(ClaimTypes.Name, user_name));

        var properties = new AuthenticationProperties();
        if (persistent)
        {
            properties.IsPersistent = true;
            properties.ExpiresUtc = DateTimeOffset.Now.AddDays(3);
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();

        return LocalRedirect("/");
    }
}