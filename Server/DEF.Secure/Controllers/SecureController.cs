using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace DEF;

public class AsciiComparer : System.Collections.Generic.IComparer<string>
{
    public int Compare(string a, string b)
    {
        if (a == b)
            return 0;
        else if (string.IsNullOrEmpty(a))
            return -1;
        else if (string.IsNullOrEmpty(b))
            return 1;
        if (a.Length <= b.Length)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] < b[i])
                    return -1;
                else if (a[i] > b[i])
                    return 1;
                else
                    continue;
            }
            return a.Length == b.Length ? 0 : -1;
        }
        else
        {
            for (int i = 0; i < b.Length; i++)
            {
                if (a[i] < b[i])
                    return -1;
                else if (a[i] > b[i])
                    return 1;
                else
                    continue;
            }
            return 1;
        }
    }
}

[ApiController]
public class SecureController : ControllerBase
{
    ILogger<SecureController> Logger { get; set; }

    public SecureController(ILogger<SecureController> logger)
    {
        Logger = logger;
    }

    // 客户端请求认证，认证成功，发放Token。客户端必须在IP白名单中。限流
    [HttpGet]
    [Route("api/1/auth")]
    public IEnumerable<ClientAuthRequest> Auth()
    {
        string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        return Enumerable.Range(1, 5).Select(index => new ClientAuthRequest
        {
            Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    // 客户端请求拉取启动数据。必须提供认证成功后的Token
    [HttpGet]
    [Route("api/1/startup")]
    public Task Startup()
    {
        Test();

        return Task.CompletedTask;
    }

    string SecretKey { get; set; } = "your_secret_key_here";

    // 确保密钥安全，不要将其硬编码在客户端代码中
    [HttpGet]
    [Route("api/1/getdata")]
    public IActionResult GetData([FromQuery] Dictionary<string, string> query_params, [FromQuery] string signature)
    {
        if (string.IsNullOrEmpty(signature))
        {
            return BadRequest("Signature is missing.");
        }

        var sorted_params = query_params.OrderBy(kvp => kvp.Key).Select(kvp => $"{kvp.Key}={kvp.Value}").ToList();
        var query_string = string.Join("&", sorted_params);
        var to_sign = query_string; // 假设时间戳也是查询参数之一，否则你可能需要将其包含在toSign中  

        var key = Encoding.UTF8.GetBytes(SecretKey);
        using (var hmac = new HMACSHA256(key))
        {
            var computed_hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(to_sign));
            var hash_string = BitConverter.ToString(computed_hash).Replace("-", "").ToLowerInvariant();

            if (hash_string != signature)
            {
                return Unauthorized("Signature validation failed.");
            }
        }

        // 如果签名验证通过，则处理请求  
        return Ok("Data retrieved successfully.");
    }

    string Test()
    {
        int _timestamp = 12345678;

        var map_param = new SortedDictionary<string, string>(new AsciiComparer())
        {
            { "z", "ZZZ" },
            { "a", "AAA" },
            { "Z", "zzz" },
            { "A", "aaa" },
            { "2", "贰" },
            { "1", "壹" },
            { "_appid", "club" },
            { "_timestamp", _timestamp.ToString() }
        };

        string sign = GetSign(map_param);

        string url_param = string.Join("&", map_param.Select(i => i.Key + "=" + i.Value));

        string url = "http://api.demo.com/dog/add?" + url_param + "&sign=" + sign;

        return url;
    }

    string GetSign(SortedDictionary<string, string> map_param, string app_key = "test")
    {
        map_param.Remove("sign");

        StringBuilder sb = new(app_key);

        foreach (var p in map_param)
        {
            sb.Append(p.Key).Append(p.Value);
        }

        sb.Append(app_key);

        return GetMD5(sb.ToString());
    }

    string GetMD5(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        var sb = new StringBuilder(32);
        var md5 = MD5.Create();
        var output = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
        for (int i = 0; i < output.Length; i++)
        {
            sb.Append(output[i].ToString("X").PadLeft(2, '0'));
        }

        return sb.ToString();
    }
}