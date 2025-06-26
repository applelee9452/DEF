namespace DEF.UCenter;

public class PayEnjoyWebhookData
{
    public string appid { get; set; }// 应用id
    public string tx_id { get; set; }// 支付订单号
    public string unid { get; set; }// 用户id
    public string usermark { get; set; }// 自定义用户标识符
    public string sku { get; set; }// 付费项id
    public string pay_order_time { get; set; }// 支付时间(1970-01-01至今)
    public string user_country_code { get; set; }// 用户地区二字码
    public string pay_type { get; set; }// 支付类型
    public string order_id { get; set; }// 支付渠道订单号
    public string market { get; set; }// 应用市场id
    public string price { get; set; }// 价格
    public string currency { get; set; }// 货币单位
    public string payload { get; set; }// 自定义参数(可传入订单号)
    public string purchase_type { get; set; }// Test
}