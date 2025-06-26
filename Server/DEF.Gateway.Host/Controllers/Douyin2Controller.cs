#if !DEF_CLIENT

using System.Security.Cryptography;
using System.Text;
using DEF;
using Microsoft.AspNetCore.Mvc;
using ProtoBuf;

namespace DanMu;

[ProtoContract]
public class PayloadLike
{
    [ProtoMember(1)]
    public string MsgId { get; set; }

    [ProtoMember(2)]
    public string OpenId { get; set; }

    [ProtoMember(3)]
    public string LikeNum { get; set; }

    [ProtoMember(4)]
    public string AvatarUrl { get; set; }

    [ProtoMember(5)]
    public string NikeName { get; set; }
}

[ProtoContract]
public class PayloadGift
{
    [ProtoMember(1)]
    public string MsgId { get; set; }

    [ProtoMember(2)]
    public string OpenId { get; set; }

    [ProtoMember(3)]
    public string GiftId { get; set; }

    [ProtoMember(4)]
    public string GiftNum { get; set; }

    [ProtoMember(5)]
    public string GiftValue { get; set; }

    [ProtoMember(6)]
    public string AvatarUrl { get; set; }

    [ProtoMember(7)]
    public string NikeName { get; set; }
}

[ProtoContract]
public class PayloadComment
{
    [ProtoMember(1)]
    public string MsgId { get; set; }

    [ProtoMember(2)]
    public string OpenId { get; set; }

    [ProtoMember(3)]
    public string Content { get; set; }

    [ProtoMember(4)]
    public string AvatarUrl { get; set; }

    [ProtoMember(5)]
    public string NikeName { get; set; }
}

public class SignatureGenerator
{
    public static string Signature(Dictionary<string, string> header, string bodyStr, string secret)
    {
        var sortedKeys = header.Keys.OrderBy(k => k).ToList();

        var kvList = sortedKeys.Select(key => $"{key}={header[key]}").ToList();
        string urlParams = string.Join("&", kvList);

        string rawData = urlParams + bodyStr + secret;

        byte[] rawBytes = Encoding.UTF8.GetBytes(rawData);
        byte[] hashBytes;
        using (MD5 md5 = MD5.Create())
        {
            hashBytes = md5.ComputeHash(rawBytes);
        }

        return Convert.ToBase64String(hashBytes);
    }
}

[ApiController]
[Route("douyin")]
[Route("douyin2")]
public class Douyin2Controller : ControllerBase
{
    ILogger<Douyin2Controller> Logger { get; set; }
    IService Service { get; set; }
    
    public Douyin2Controller(ILogger<Douyin2Controller> logger)
    {
        Logger = logger;
        //Service = s;
    }

    // Webhook，点赞
    [HttpPost]
    [Route("dz")]
    public async Task<string> PostDz()
    {
        Logger.LogInformation("Webhook，点赞");

        //var options = new DouyinMicroAppClientOptions()
        //{
        //    PushToken = "dddddd",
        //    AppId = "tt1a956c28859dac3a10",
        //    AppSecret = "8224122ca83711e0ffa86e094aa5abf745ff425a",
        //};
        //var client = DouyinMicroAppClientBuilder.Create(options).Build();

        StreamReader stream = new(Request.Body);
        string body_str = await stream.ReadToEndAsync();

        Dictionary<string, string> map = new()
            {
                { "x-msg-type", HttpContext.Request.Headers["x-msg-type"] },
                { "x-nonce-str", HttpContext.Request.Headers["x-nonce-str"] },
                { "x-timestamp", HttpContext.Request.Headers["x-timestamp"] },
                { "x-roomid", HttpContext.Request.Headers["x-roomid"] },
            };

        string signature_douyin = HttpContext.Request.Headers["x-signature"];
        string signature_mine = SignatureGenerator.Signature(map, body_str, "dddddd");
        if (signature_douyin != signature_mine)
        {
            Logger.LogError("Webhook，点赞，签名不匹配！自己计算出的：{0}，抖音计算出的：{1}", signature_mine, signature_douyin);
            return string.Empty;
        }

        body_str = body_str.Replace("[", "");
        body_str = body_str.Replace("]", "");
        Dictionary<string, object> pars = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(body_str);
        PayloadLike payload_like = new PayloadLike
        {
            MsgId = pars["msg_id"].ToString(),
            OpenId = pars["sec_openid"].ToString(),
            LikeNum = pars["like_num"].ToString(),
            AvatarUrl = pars["avatar_url"].ToString(),
            NikeName = pars["nickname"].ToString(),
        };

        Logger.LogInformation("Webhook，点赞，msg_id={MsgId}, sec_openid={OpenId}, like_num={LikeNum}, avatar_url={AvatarUrl}, nickname={NikeName}",
            payload_like.MsgId, payload_like.OpenId, payload_like.LikeNum, payload_like.AvatarUrl, payload_like.NikeName);

        //var send_body_str = Service.GetContainerRpc<IContainerStatefulRoom>(map["x-roomid"]);
        //await send_body_str.DouyinDz(payload_like);

        return string.Empty;
    }

    // Webhook，礼物
    [HttpPost]
    [Route("lw")]
    public async Task<string> PostLw()
    {
        Logger.LogInformation("Webhook，礼物");

        StreamReader stream = new(Request.Body);
        string body_str = await stream.ReadToEndAsync();

        Dictionary<string, string> map = new()
            {
                { "x-msg-type", HttpContext.Request.Headers["x-msg-type"] },
                { "x-nonce-str", HttpContext.Request.Headers["x-nonce-str"] },
                { "x-timestamp", HttpContext.Request.Headers["x-timestamp"] },
                { "x-roomid", HttpContext.Request.Headers["x-roomid"] },
            };

        string signature_douyin = HttpContext.Request.Headers["x-signature"];
        string signature_mine = SignatureGenerator.Signature(map, body_str, "llllll");
        if (signature_douyin != signature_mine)
        {
            Logger.LogError("Webhook，礼物，签名不匹配！自己计算出的：{0}，抖音计算出的：{1}", signature_mine, signature_douyin);
            return string.Empty;
        }

        body_str = body_str.Replace("[", "");
        body_str = body_str.Replace("]", "");
        Dictionary<string, object> pars = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(body_str);
        PayloadGift payload_gift = new()
        {
            MsgId = pars["msg_id"].ToString(),
            OpenId = pars["sec_openid"].ToString(),
            GiftId = pars["sec_gift_id"].ToString(),
            GiftNum = pars["gift_num"].ToString(),
            GiftValue = pars["gift_value"].ToString(),
            AvatarUrl = pars["avatar_url"].ToString(),
            NikeName = pars["nickname"].ToString(),
        };

        Logger.LogInformation("Webhook，礼物，msg_id={MsgId}, sec_openid={OpenId}, GiftId={GiftId}, GiftNum={GiftNum}, GiftValue={GiftValue}, avatar_url={AvatarUrl}, nickname={NikeName}",
            payload_gift.MsgId, payload_gift.OpenId, payload_gift.GiftId, payload_gift.GiftNum, payload_gift.GiftValue, payload_gift.AvatarUrl, payload_gift.NikeName);

        //var send_body_str = Service.GetContainerRpc<IContainerStatefulRoom>(map["x-roomid"]);
        //await send_body_str.DouyinLw(payload_gift);

        return string.Empty;
    }

    // Webhook，评论
    [HttpPost]
    [Route("pl")]
    public async Task<string> PostPl()
    {
        Logger.LogInformation("Webhook，评论");

        StreamReader stream = new(Request.Body);
        string body_str = await stream.ReadToEndAsync();

        Dictionary<string, string> map = new()
            {
                { "x-msg-type", HttpContext.Request.Headers["x-msg-type"] },
                { "x-nonce-str", HttpContext.Request.Headers["x-nonce-str"] },
                { "x-timestamp", HttpContext.Request.Headers["x-timestamp"] },
                { "x-roomid", HttpContext.Request.Headers["x-roomid"] },
            };

        string signature_douyin = HttpContext.Request.Headers["x-signature"];
        string signature_mine = SignatureGenerator.Signature(map, body_str, "pppppp");
        if (signature_douyin != signature_mine)
        {
            Logger.LogError("Webhook，评论，签名不匹配！自己计算出的：{0}，抖音计算出的：{1}", signature_mine, signature_douyin);
            return string.Empty;
        }

        body_str = body_str.Replace("[", "");
        body_str = body_str.Replace("]", "");
        Dictionary<string, object> pars = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(body_str);

        PayloadComment payload_comment = new()
        {
            MsgId = pars["msg_id"].ToString(),
            OpenId = pars["sec_openid"].ToString(),
            Content = pars["content"].ToString(),
            AvatarUrl = pars["avatar_url"].ToString(),
            NikeName = pars["nickname"].ToString(),
        };

        Logger.LogInformation("Webhook，评论，msg_id={MsgId}, sec_openid={OpenId}, content={Content}, avatar_url={AvatarUrl}, nickname={NikeName}",
            payload_comment.MsgId, payload_comment.OpenId, payload_comment.Content, payload_comment.AvatarUrl, payload_comment.NikeName);

        //var send_body_str = Service.GetContainerRpc<IContainerStatefulRoom>(map["x-roomid"]);
        //await send_body_str.DouyinPl(payload_comment);

        return string.Empty;
    }
}

#endif