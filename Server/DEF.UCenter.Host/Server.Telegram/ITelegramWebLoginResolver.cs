using FluentResults;

namespace Hibit.IdServer.Telegrams;

public interface ITelegramWebLoginResolver
{
    string GetDataHash(TelegramValidData data);

    bool ValidateWebLogin(TelegramValidData data);

    Result<ValidTelegramUser> ResolveWebLogin(TelegramValidData data);
}

public readonly record struct TelegramValidData(IReadOnlyDictionary<string, string> Data)
{
    public string Hash => Data["hash"];
    public DateTimeOffset AuthDate => DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(Data["auth_date"]));

    public string CheckString => string.Join('\n', Data.Where(x => x.Key != "hash")
        .Select(x => $"{x.Key}={x.Value}")
        .Order());
}