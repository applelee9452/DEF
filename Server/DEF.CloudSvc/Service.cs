namespace DEF.Cloud;

public enum CloudType
{
    Aliyun = 0,
    TencentCloud,
    Other = 200,
}

public static class Service
{
    public static ISms Sms { get; private set; }
    public static IStorage Storage { get; private set; }

    public static void ConfigSms(CloudType type)
    {
        if (Sms != null) return;

        switch (type)
        {
            case CloudType.Aliyun:
                Sms = new SmsAliyun();
                break;
            //case CloudType.TencentCloud:
            //    Sms = new SmsTencent();
            //    break;
            case CloudType.Other:
                Sms = new SmsTwilio();
                break;
        }
    }


    public static void ConfigStorage(CloudType type)
    {
        if (Storage != null) return;

        switch (type)
        {
            case CloudType.Aliyun:
                Storage = new StorageOss();
                break;
                //case CloudType.TencentCloud:
                //    Storage = new StorageCOS();
                //    break;
        }
    }
}