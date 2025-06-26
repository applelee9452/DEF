namespace DEF.UCenter;

public class PayChannelTraits
{
    // 同一个计费点能否同时发起多笔订单
    public static bool ChargeProductIdConcurrency(PayPlatform channel)
    {
        if (channel == PayPlatform.GooglePlay
            || channel == PayPlatform.AppStore
            || channel == PayPlatform.KilatPay)
        {
            return false;
        }

        return true;
    }
}