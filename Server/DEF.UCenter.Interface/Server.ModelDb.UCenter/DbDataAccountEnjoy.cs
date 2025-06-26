using System;

namespace DEF.UCenter;

public class DataAccountEnjoy : DataBase
{
    public string AccountId { get; set; }
    public string AppId { get; set; }
    public string Market { get; set; }// 市场标识
    public string AppId2 { get; set; }// 应用Id
    public string Unid { get; set; }// 用户唯一Id
    public string LoginType { get; set; }// 登陆类型
    public string LoginId { get; set; }// 第三方登陆Id
    public string NickName { get; set; }// 用户昵称
    public string Email { get; set; }// 用户邮箱
    public string CountryCode { get; set; }// 用户地区（国家码）
    public DateTime CreateTime { get; set; }// 用户创建时间
    public bool Paid { get; set; }// 是否是付费用户
    public string DeviceId { get; set; }// 设备Id
}