using System;

namespace DEF.SyncDb;

public class DataAccount
{
    public string Id { get; set; }
    public string AccountName { get; set; }
}

public class EvAccountLoginLogout
{
    public string Id { get; set; }
    public string AccountId { get; set; }
    public string AccountName { get; set; }
    public bool LoginOrLogout { get; set; }
    public DateTime Dt { get; set; }
}