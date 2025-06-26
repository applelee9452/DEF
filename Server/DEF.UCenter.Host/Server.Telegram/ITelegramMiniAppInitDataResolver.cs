using FluentResults;
using System.Text.Json.Serialization;

namespace Hibit.IdServer.Telegrams;

public interface ITelegramMiniAppInitDataResolver
{
    string GetDataHash(TelegramValidData data);

    bool ValidateMiniInitData(TelegramValidData initData);

    Result<ValidTelegramUser> ResolveMiniAppInitData(TelegramValidData data);
}

public record ValidTelegramUser(long UserId, IReadOnlyDictionary<string, string> Source);

public record TelegramUserInitData
{
    /// <summary>
    /// The date the initialization data was created. Is a number representing a Unix timestamp.
    /// </summary>
    public DateTimeOffset AuthDate { get; set; }

    /// <summary>
    /// Optional. The number of seconds after which a message can be sent via the method answerWebAppQuery.
    /// </summary>
    public int CanSendAfter { get; set; }

    /// <summary>
    /// Optional. An object containing information about the chat with the bot in which the Mini Apps was launched. It is returned only for Mini Apps opened through the attachments menu.
    /// </summary>
    public TelegramChat Chat { get; set; }

    /// <summary>
    /// Optional. The type of chat from which the Mini Apps was opened. Values:
    /// </summary>
    public TelegramUserInitDataChatType? ChatType { get; set; }

    /// <summary>
    /// Optional. A global identifier indicating the chat from which the Mini Apps was opened. Returned only for applications opened by direct link.
    /// </summary>
    public string ChatInstance { get; set; }

    /// <summary>
    /// Initialization data signature.
    /// </summary>
    public string Hash { get; set; }

    /// <summary>
    /// Optional. The unique session ID of the Mini App. Used in the process of sending a message via the method answerWebAppQuery.
    /// </summary>
    public string QueryId { get; set; }

    /// <summary>
    /// Optional. An object containing data about the chat partner of the current user in the chat where the bot was launched via the attachment menu. Returned only for private chats and only for Mini Apps launched via the attachment menu.
    /// </summary>
    public TelegramUser Receiver { get; set; }

    /// <summary>
    /// Optional. The value of the startattach or startapp query parameter specified in the link. It is returned only for Mini Apps opened through the attachment menu.
    /// </summary>
    public string StartParam { get; set; }

    /// <summary>
    /// Optional. An object containing information about the current user.
    /// </summary>
    public TelegramUser User { get; set; }
}

public enum TelegramUserInitDataChatType
{
    Sender,
    Private,
    Group,
    Supergroup,
    Channel
}

public record TelegramChat
{
    /// <summary>
    /// Unique chat ID.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    ///
    /// </summary>
    public TelegramChatType Type { get; set; }

    /// <summary>
    /// Chat title
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Optional. Chat photo link. The photo can have .jpeg and .svg formats. It is returned only for Mini Apps opened through the attachments menu.
    /// </summary>
    public string PhotoUrl { get; set; }

    /// <summary>
    /// Optional. Chat user login.
    /// </summary>
    public string Username { get; set; }
}

public enum TelegramChatType
{
    Group,
    Supergroup,
    Channel
}

public record TelegramUser
{
    /// <summary>
    /// Optional. True, if this user added the bot to the attachment menu.
    /// </summary>
    [JsonPropertyName("added_to_attachment_menu")]
    public bool AddedToAttachmentMenu { get; set; }

    /// <summary>
    /// Optional. True, if this user allowed the bot to message them.
    /// </summary>
    [JsonPropertyName("allows_write_to_pm")]
    public bool AllowsWriteToPm { get; set; }

    /// <summary>
    /// Optional. Has the user purchased Telegram Premium.
    /// </summary>
    [JsonPropertyName("is_premium")]
    public bool IsPremium { get; set; }

    /// <summary>
    /// Bot or user name.
    /// </summary>
    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Bot or user ID.
    /// </summary>
    [JsonPropertyName("id")]
    public long Id { get; set; }

    /// <summary>
    /// Optional. Is the user a bot.
    /// </summary>
    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }

    /// <summary>
    /// Optional. User's last name.
    /// </summary>
    [JsonPropertyName("last_name")]
    public string LastName { get; set; }

    /// <summary>
    /// Optional. IETF user's language.
    /// </summary>
    [JsonPropertyName("language_code")]
    public string LanguageCode { get; set; }

    /// <summary>
    /// Optional. Link to the user's or bot's photo. Photos can have formats .jpeg and .svg. It is returned only for Mini Apps opened through the attachment menu.
    /// </summary>
    [JsonPropertyName("photo_url")]
    public string PhotoUrl { get; set; }

    /// <summary>
    /// Optional. Login of the bot or user.
    /// </summary>
    [JsonPropertyName("user_name")]
    public string Username { get; set; }
}