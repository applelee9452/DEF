namespace DEF.UCenter
{
    // 帐号启用状态
    public enum AccountStatus
    {
        Active,
        Disabled
    }

    // 帐号正常类别与游客类别
    public enum AccountType
    {
        NormalAccount = 0,
        Guest,
    }

    // 登陆源
    //public enum LoginFrom
    //{
    //    Default = 0,
    //    Email = 5,
    //    Phone = 10,
    //    Orange = 20,
    //    QQ = 30,
    //    Facebook = 40
    //}

    //// 支付订单状态
    //public enum PayOrderStatus
    //{
    //    ChargeCreated = 0,
    //    PaySuccess = 50,
    //    PayFailed,
    //    PayExpired,
    //}
}