// BeeCloud->Pay

using System.Runtime.Serialization;

namespace DEF.UCenter;

public class Extra
{
}

public class Refunds
{
    public string @object { get; set; }

    public string url { get; set; }

    public bool has_more { get; set; }

    public List<object> data { get; set; }
}

public class Metadata
{
}

public class Credential
{
}

[DataContract]
public class DataObject
{
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "object")]
    public string ObjectName { get; set; }

    [DataMember(Name = "created")]
    public int Created { get; set; }

    [DataMember(Name = "livemode")]
    public bool LiveMode { get; set; }

    [DataMember(Name = "paid")]
    public bool Paid { get; set; }

    [DataMember(Name = "refunded")]
    public bool Refunded { get; set; }

    [DataMember(Name = "app")]
    public string App { get; set; }

    [DataMember(Name = "channel")]
    public string Channel { get; set; }

    [DataMember(Name = "order_no")]
    public string OrderNo { get; set; }

    [DataMember(Name = "client_ip")]
    public string ClientIp { get; set; }

    [DataMember(Name = "amount")]
    public int Amount { get; set; }

    [DataMember(Name = "amount_settle")]
    public int AmountSettle { get; set; }

    [DataMember(Name = "currency")]
    public string Currency { get; set; }

    [DataMember(Name = "subject")]
    public string Subject { get; set; }

    [DataMember(Name = "body")]
    public string Body { get; set; }

    [DataMember(Name = "extra")]
    public Extra Extra { get; set; }

    [DataMember(Name = "time_paid")]
    public int TimePaid { get; set; }

    [DataMember(Name = "time_expire")]
    public int TimeExpire { get; set; }

    [DataMember(Name = "time_settle")]
    public object TimeSettle { get; set; }

    [DataMember(Name = "transaction_no")]
    public string TransactionNo { get; set; }

    [DataMember(Name = "refunds")]
    public Refunds Refunds { get; set; }

    [DataMember(Name = "amount_refunded")]
    public int AmountRefunded { get; set; }

    [DataMember(Name = "failure_code")]
    public string FailureCode { get; set; }

    [DataMember(Name = "failure_msg")]
    public string FailureMessage { get; set; }

    [DataMember(Name = "metadata")]
    public Metadata metadata { get; set; }

    [DataMember(Name = "credential")]
    public Credential Credential { get; set; }

    [DataMember(Name = "description")]
    public string Description { get; set; }
}

[DataContract]
public class Data
{
    [DataMember(Name = "object")]
    public DataObject DataObject { get; set; }
}

[DataContract]
public class CallbackInfo
{
    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "created")]
    public int Created { get; set; }

    [DataMember(Name = "livemode")]
    public bool LiveMode { get; set; }

    [DataMember(Name = "type")]
    public string EventType { get; set; }

    [DataMember(Name = "data")]
    public Data Data { get; set; }

    [DataMember(Name = "object")]
    public string ObjectName { get; set; }

    [DataMember(Name = "pending_webhooks")]
    public int PendingWebhooks { get; set; }

    [DataMember(Name = "request")]
    public string Request { get; set; }
}