#if !DEF_CLIENT

namespace DEF.IM;

// CDKey领取记录
public class EvExchangeCDKey : EventBase
{
    public string CDKeyGuid { get; set; }
    public string RegionGuid { get; set; }
    public string PlayerGuid { get; set; }
    public string RegionName { get; set; }
    public string PlayerNickName { get; set; }
    public string CDKey { get; set; }
}

public class EvExchangeCDKeyView : EvExchangeCDKey
{
    public EvExchangeCDKeyView()
    {
    }

    public EvExchangeCDKeyView(EvExchangeCDKey data)
    {
        this._id = data._id;
        this.EventType = data.EventType;
        this.EventTm = data.EventTm;
        this.CDKeyGuid = data.CDKeyGuid;
        this.RegionGuid = data.RegionGuid;
        this.PlayerGuid = data.PlayerGuid;
        this.RegionName = data.RegionName;
        this.PlayerNickName = data.PlayerNickName;
        this.CDKey = data.CDKey;
    }
}

#endif