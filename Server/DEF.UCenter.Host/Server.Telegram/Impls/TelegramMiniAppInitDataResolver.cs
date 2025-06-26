using System.Security.Cryptography;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Options;

namespace Hibit.IdServer.Telegrams;

internal sealed partial class TelegramMiniAppInitDataResolver : ITelegramMiniAppInitDataResolver
{
    IOptions<TelegramMiniAppOptions> Options { get; set; }
    TimeProvider TimeProvider { get; set; }

    public bool ValidateMiniInitData(TelegramValidData initData)
    {
        // https://docs.telegram-mini-apps.com/platform/init-data#validating
        var firstHmacResult = ComputeHmacSha256("WebAppData"u8.ToArray(),
            Encoding.UTF8.GetBytes(Options.Value.BotToken));

        var secondHmacResult = ComputeHmacSha256(firstHmacResult, Encoding.UTF8.GetBytes(initData.CheckString));

        var secondHmacResultHex = Convert.ToHexString(secondHmacResult);
        var hashMatch = string.Compare(initData.Hash, secondHmacResultHex, StringComparison.OrdinalIgnoreCase) == 0;
        if (!hashMatch)
        {
            return false;
        }

        if (Options.Value.IsValidateAuthDate)
        {
            var authData = initData.AuthDate;
            var now = TimeProvider.GetUtcNow();
            if (authData < now.AddMinutes(-5) || authData > now.AddMinutes(1))
            {
                return false;
            }
        }

        return true;
    }

    public string GetDataHash(TelegramValidData data)
    {
        var firstHmacResult = ComputeHmacSha256("WebAppData"u8.ToArray(),
            Encoding.UTF8.GetBytes(Options.Value.BotToken));
        var secondHmacResult = ComputeHmacSha256(firstHmacResult, Encoding.UTF8.GetBytes(data.CheckString));
        return Convert.ToHexString(secondHmacResult).ToLowerInvariant();
    }

    public Result<ValidTelegramUser> ResolveMiniAppInitData(TelegramValidData data)
    {
        if (!ValidateMiniInitData(data))
        {
            return Result.Fail("Invalid telegram data");
        }

        if (data.Data.TryGetValue("user", out var userJson))
        {
            var user = Newtonsoft.Json.JsonConvert.DeserializeObject<TelegramUser>(userJson);
            return Result.Ok(new ValidTelegramUser(user.Id, data.Data));
        }

        return Result.Fail("Failed to resolve telegram user");
    }

    private static byte[] ComputeHmacSha256(byte[] key, byte[] data)
    {
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(data);
    }
}