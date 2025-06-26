using System.ComponentModel.DataAnnotations;

namespace Hibit.IdServer.Telegrams;

public record TelegramMiniAppOptions
{
    [Required] public string BotToken { get; set; } = null!;
    public string HibitIdWebOidcTriggerUrl { get; set; } = null!;
    public bool IsValidateAuthDate { get; set; } = true;
    public string DefaultPassword { get; set; }
    public int RandomUserNameLength { get; set; } = 6;
}