using System.Security.Cryptography;
using System.Text;
using FluentResults;
using Microsoft.Extensions.Options;

namespace Hibit.IdServer.Telegrams;

internal sealed partial class TelegramWebLoginResolver : ITelegramWebLoginResolver
{
    IOptions<TelegramMiniAppOptions> Options { get; set; }
    TimeProvider TimeProvider { get; set; }

    public string GetDataHash(TelegramValidData data)
    {
        using var sha256 = SHA256.Create();
        var key = sha256.ComputeHash(Encoding.UTF8.GetBytes(Options.Value.BotToken));
        using var hmacsha256 = new HMACSHA256(key);
        var computeHash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(data.CheckString));
        return Convert.ToHexString(computeHash).ToLowerInvariant();
    }

    public bool ValidateWebLogin(TelegramValidData data)
    {
        var dataHash = GetDataHash(data);
        if (dataHash != data.Hash)
        {
            return false;
        }

        if (Options.Value.IsValidateAuthDate)
        {
            var authData = data.AuthDate;
            var now = TimeProvider.GetUtcNow();
            if (authData < now.AddMinutes(-5) || authData > now.AddMinutes(1))
            {
                return false;
            }
        }

        return true;
    }

    public Result<ValidTelegramUser> ResolveWebLogin(TelegramValidData data)
    {
        if (!ValidateWebLogin(data))
        {
            return Result.Fail("Invalid telegram data");
        }

        if (data.Data.TryGetValue("id", out var userId))
        {
            var user = new ValidTelegramUser(Convert.ToInt64(userId), data.Data);
            return Result.Ok(user);
        }

        return Result.Fail("Failed to resolve telegram user");
    }
}