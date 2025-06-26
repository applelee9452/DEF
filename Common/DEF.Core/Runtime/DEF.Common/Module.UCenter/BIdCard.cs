#if !DEF_CLIENT
using Orleans;
#endif
using MemoryPack;
using ProtoBuf;

namespace DEF.UCenter
{
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class IDCradResultRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string AccountId { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Token { get; set; }
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class CheckCardAndNameRequest
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string AccountId { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string Token { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string CardNo { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public string RealName { get; set; }
        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string PhoneCode { get; set; }
        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string PhoneNumber { get; set; }
        [ProtoMember(7)]
#if !DEF_CLIENT
        [Id(6)]
#endif
        public string PhoneVerificationCode { get; set; }
    }

    // 失败返回
    //{
    //"error_code": 90099,/*状态码*/
    //"reason": "认证不通过", /*状态描述*/    
    //"ordersign":"20170721000516730299989856" /*唯一订单号*/
    //}

    // 成功返回
    //{
    //"error_code": 0,
    //"reason": "认证通过", /*验证结果*/
    //"result": {
    //    "realName": "李xxx",
    //    "cardNo": "4206061",
    //    "details": {
    //        "addrCode": "420606", /*地区编码*/
    //        "birth": "1984-02-15",  /*出生日期*/
    //        "sex": 1,  /*性别*/
    //        "length": 18,   /*身份证位数*/
    //        "checkBit": "1",   /*身份证最后一位*/
    //        "addr": "湖北省襄樊市樊城区",  /*身份证所在地*/
    //    }
    //},
    //"ordersign": "2017052722072914949571005" /*订单号*/
    //}

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class IdCardDetails
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string addrCode { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string birth { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public int sex { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public int length { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string checkBit { get; set; }

        [ProtoMember(6)]
#if !DEF_CLIENT
        [Id(5)]
#endif
        public string addr { get; set; }
    }

    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class IdCardResult
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public string realName { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public string cardNo { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public IdCardDetails details { get; set; }
    }

    //错误码    错误信息
    //0	        认证通过
    //80001	    参数不完整
    //80003	    姓名格式不正确
    //80004	    身份证号码格式不正确
    //80008	    身份证中心维护，请稍后重试
    //90033	    无此身份证号码
    //90099	    认证不通过
    [MemoryPackable]
    [ProtoContract]
#if !DEF_CLIENT
    [GenerateSerializer]
#endif
    public partial class IdCardResponse
    {
        [ProtoMember(1)]
#if !DEF_CLIENT
        [Id(0)]
#endif
        public UCenterErrorCode ErrorCode { get; set; }

        [ProtoMember(2)]
#if !DEF_CLIENT
        [Id(1)]
#endif
        public int error_code { get; set; }

        [ProtoMember(3)]
#if !DEF_CLIENT
        [Id(2)]
#endif
        public string reason { get; set; }

        [ProtoMember(4)]
#if !DEF_CLIENT
        [Id(3)]
#endif
        public IdCardResult result { get; set; }

        [ProtoMember(5)]
#if !DEF_CLIENT
        [Id(4)]
#endif
        public string ordersign { get; set; }
    }
}