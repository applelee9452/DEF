using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;

namespace DEF.UCenter;

public class ContainerStatelessChannelGooglePlayAccessToken : ContainerStateless, IContainerStatelessChannelGooglePlayAccessToken
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }
    string GooglePayAccessToken { get; set; }
    IOptions<UCenterOptions> UCenterOptions { get; set; }
    IDisposable TimerHandleUpdate { get; set; }

    public override Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;
        UCenterOptions = UCenterContext.Instance.UCenterOptions;

        TimerHandleUpdate = RegisterTimer((_) => TimerUpdateAsync(), null,
                TimeSpan.FromMinutes(30), TimeSpan.FromMinutes(30));

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        if (TimerHandleUpdate != null)
        {
            TimerHandleUpdate.Dispose();
            TimerHandleUpdate = null;
        }

        return Task.CompletedTask;
    }

    Task TimerUpdateAsync()
    {
        return RefreshAccessToken();
    }

    Task IContainerStatelessChannelGooglePlayAccessToken.Setup()
    {
        return RefreshAccessToken();
    }

    async Task<string> IContainerStatelessChannelGooglePlayAccessToken.GetAccessToken()
    {
        if (GooglePayAccessToken == null)
        {
            await RefreshAccessToken();
        }
        return GooglePayAccessToken;
    }

    async Task RefreshAccessToken()
    {
        // todo，待整理
        if (!UCenterOptions.Value.GooglePayTimingGetAccessToken)
        {
            return;
        }

        var request_data1 = new LitJson.JsonData
        {
            ["grant_type"] = "refresh_token",
            ["client_id"] = UCenterOptions.Value.GooglePayClientId,
            ["client_secret"] = UCenterOptions.Value.GooglePayClientSecret,
            ["refresh_token"] = UCenterOptions.Value.GooglePayRefreshToken
        };
        string request_data2 = request_data1.ToJson();

        string response_data = string.Empty;
        using (var web_client = HttpClientFactory.CreateClient())
        {
            // todo，待整理
            using var response = await web_client.PostAsync(UCenterOptions.Value.GooglePayGetAccessTokenUrl,
                new StringContent(request_data2, Encoding.UTF8, "application/json"));
            response_data = await response.Content.ReadAsStringAsync();
        }

        var responseData = LitJson.JsonMapper.ToObject(response_data);

        if (responseData.ContainsKey("access_token"))
        {
            GooglePayAccessToken = responseData["access_token"].ToString();

            Logger.LogInformation("获取google access_token 成功 GooglePayAccessToken=" + GooglePayAccessToken);
        }
        else
        {
            Logger.Log(LogLevel.Error, "获取google access_token 出错 respString=" + response_data);
        }
    }
}