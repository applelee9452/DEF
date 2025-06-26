using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace DEF.UCenter;

public class ContainerStatelessChannelEnjoy : ContainerStateless, IContainerStatelessChannelEnjoy
{
    DbClientMongo Db { get; set; }
    IHttpClientFactory HttpClientFactory { get; set; }

    string EnjoyServerSecret = "4ch3ngjwkxqwlex9";

    public override Task OnCreate()
    {
        Db = UCenterContext.Instance.Db;
        HttpClientFactory = UCenterContext.Instance.HttpClientFactory;

        return Task.CompletedTask;
    }

    public override Task OnDestroy()
    {
        return Task.CompletedTask;
    }

    async Task<string> IContainerStatelessChannelEnjoy.EnjoyWebhookRequest(Dictionary<string, string> map_data, string client_ip)
    {
        //enjoy_data 为加密数据, 通过服务器密钥可以进行数据解密
        //enjoy_data 加密数据为Base64编码的 DES 加密
        //DES加密模式: ECB
        // 填充方式: PKCS5Padding
        // 密码:服务器签名字符串
        //3)	数据投递方式为: HTTP POST 方式
        //4)	投递数据机制为: 循环请求 1day 直到成功
        //5)	对接服务器在收到支付数据,并且成功处理后需要返回JSON 数据给到对应接口, 成功必须返回 { "code":1000} ,code不等于1000 则enjoy 服务器将会重新投递数据直到成功
        //6)	对接服务器需要进行订单排重处理以防同一个奖励重复派发,订单id 字段为: tx_id
        //7)	enjoy_data 解密后数据为 json 格式数据字段解析如下:

        Logger.LogInformation("GrainChannelEnjoy.EnjoyWebhookRequest()");

        var enjoy_appid = map_data["appid"];
        var enjoy_data = map_data["data"];

        Logger.LogInformation("解密前数据：" + enjoy_data);

        string enjoy_data2 = Decrypt(enjoy_data, EnjoyServerSecret);

        Logger.LogInformation("解密后数据：" + enjoy_data2);

        if (string.IsNullOrEmpty(enjoy_data2))
        {
            return string.Empty;
        }

        var container = GetContainerRpc<IContainerStatelessPayService>();
        return await container.PayChargeFinishByServer(enjoy_data2, client_ip);
    }

    public static string Decrypt(string src_data, string key)
    {
        var des = DES.Create();
        des.Mode = CipherMode.ECB;

        byte[] arr = Encoding.ASCII.GetBytes(key[..8]);
        des.Key = arr;
        des.IV = arr;

        var trans = des.CreateDecryptor();

        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, trans, CryptoStreamMode.Write);

        byte[] buffer = Convert.FromBase64String(src_data);
        cs.Write(buffer, 0, buffer.Length);
        cs.FlushFinalBlock();

        return Encoding.UTF8.GetString(ms.ToArray());
    }
}