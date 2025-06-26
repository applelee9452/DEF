namespace DEF.UCenter;

using System;

public class DataIdCard : DataBase
{
    public string CardNo { get; set; }// 身份证号码
    public string RealName { get; set; }// 真实姓名
    public string AddrCode { get; set; }// 地区编码
    public DateTime Birth { get; set; }// "1984-02-15", 出生日期
    public GenderType Sex { get; set; }// 性别，男=1
    public int Length { get; set; }// 身份证位数
    public string CheckBit { get; set; }// 身份证最后一位
    public string Addr { get; set; }// 身份证所在地
}