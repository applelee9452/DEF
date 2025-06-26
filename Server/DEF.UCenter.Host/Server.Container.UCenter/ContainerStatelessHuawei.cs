#if !DEF_CLIENT

using Microsoft.Extensions.Logging;

namespace DEF.UCenter;

public class ContainerStatelessHuawei : ContainerStateless, IContainerStatelessHuawei
{
    IHttpClientFactory HttpClientFactory { get; set; }

    public override Task OnCreate()
    {
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    async Task IContainerStatelessHuawei.QueryIpAddress(string ip)
    {
        var uri = $"http://smipcity.apistore.huaweicloud.com/v4/ip/city/query?coordsys=&ip={ip}";

        // 认证用的ak和sk硬编码到代码中或者明文存储都有很大的安全风险，建议在配置文件或者环境变量中密文存放，使用时解密，确保安全；
        // 本示例以ak和sk保存在环境变量中为例，运行本示例前请先在本地环境中设置环境变量HUAWEICLOUD_SDK_AK和HUAWEICLOUD_SDK_SK。
        APIGATEWAY_SDK.Signer signer = new()
        {
            Key = "43537541836f4f469d9b7209c11b6d11",
            Secret = "ad5ec6e84ec64fb9b3629177bb662ebb"
        };

        try
        {
            using HttpClient hc = HttpClientFactory.CreateClient();

            APIGATEWAY_SDK.HttpRequest r = new("POST", new Uri(uri));
            r.headers.Add("x-stage", "RELEASE");
            r.headers.Add("content-type", "application/json");
            using var req = signer.SignHttp(r);
            //using var req = new HttpRequestMessage(HttpMethod.Post, uri);

            using var res = await hc.SendAsync(req);

            if (res.IsSuccessStatusCode)
            {
                string response_str = await res.Content.ReadAsStringAsync();

                // {"msg":"成功","success":true,"code":200,"data":{"result":{"continent":"保留IP","owner":"","country":"","lng":"","adcode":"","city":"","timezone":"","isp":"","accuracy":"","source":"数据挖掘","asnumber":"","areacode":"B1","zipcode":"","radius":"","prov":"","lat":""},"orderNo":"979626815625128426"}}
                // {"msg":"参数ip格式不对","success":false,"code":400,"data":null}
                // {"msg":"成功","success":true,"code":200,"data":{"orderNo":"289143189416678296","result":{"continent":"亚洲","owner":"电讯盈科","country":"中国","lng":"114.184921","adcode":"810000","city":"中国香港","timezone":"UTC+8","isp":"电讯盈科","accuracy":"城市","source":"数据挖掘","asnumber":"4760","areacode":"CN","zipcode":"999077","radius":"40.0088","prov":"中国香港","lat":"22.350617"}}}

                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountWechatAutoLoginResponse>(response_str);
                if (response == null || response.errcode != 0)
                {
                }

            }
            else
            {
                Logger.LogError("ContainerStatelessHuawei.QueryIpAddress() Failed！StatusCode={0}", res.StatusCode);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "ContainerStatelessHuawei.QueryIpAddress()");
        }
    }

    // 手机三要素实名认证
    async Task IContainerStatelessHuawei.QueryMobileThree(string id_card, string name, string mobile)
    {
        var uri = $"http://mobthree.apistore.huaweicloud.com/lundear/mobThree?idcard={id_card}&mobile={mobile}&name={Uri.EscapeDataString(name)}";

        // 认证用的ak和sk硬编码到代码中或者明文存储都有很大的安全风险，建议在配置文件或者环境变量中密文存放，使用时解密，确保安全；
        // 本示例以ak和sk保存在环境变量中为例，运行本示例前请先在本地环境中设置环境变量HUAWEICLOUD_SDK_AK和HUAWEICLOUD_SDK_SK。
        APIGATEWAY_SDK.Signer signer = new()
        {
            Key = "85fcdb6191ad4cabbd8d669b7f17e997",
            Secret = "9e8a3ee8c21f4ff7a3b3e9d7c5196b22"
        };

        try
        {
            using HttpClient hc = HttpClientFactory.CreateClient();

            APIGATEWAY_SDK.HttpRequest r = new("GET", new Uri(uri));
            r.headers.Add("x-stage", "RELEASE");
            r.headers.Add("content-type", "application/json");
            using var req = signer.SignHttp(r);

            using var res = await hc.SendAsync(req);

            if (res.IsSuccessStatusCode)
            {
                string response_str = await res.Content.ReadAsStringAsync();

                // {"data":{"province":"上海","areacode":"021","postcode":"200000","city":"上海","isp":"移动"},"desc":"一致","code":0}
                // {"data":{"province":"上海","areacode":"021","postcode":"200000","city":"上海","isp":"移动"},"desc":"不一致","code":1}

                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountWechatAutoLoginResponse>(response_str);
                if (response == null || response.errcode != 0)
                {
                }

            }
            else
            {
                Logger.LogError("ContainerStatelessHuawei.QueryIpAddress() Failed！StatusCode={0}", res.StatusCode);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e, "ContainerStatelessHuawei.QueryIpAddress()");
        }
    }
}

#endif