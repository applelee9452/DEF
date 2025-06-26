namespace DEF.UCenter;

using System;

// 账号文档
public class DataAccount : DataBase
{
    public AccountType AccountType { get; set; }// 普通，游客
    public AccountStatus AccountStatus { get; set; }// 正常，禁用
    public string AccountName { get; set; }// 账号名，可以为空，为空时继续检测手机和邮箱字段
    public string PhoneCode { get; set; }// 国家码
    public string PhoneNumber { get; set; }// 手机号码
    public string Email { get; set; }// 邮箱
    public string Password { get; set; }
    public string SuperPassword { get; set; }
    public ulong AgentId { get; set; }
    public ulong[] AgentParents { get; set; }// 所有直系代理，包含上面的AgentId在第一个元素中
    public string Name { get; set; }
    public GenderType Gender { get; set; } = GenderType.Unknow;// 性别
    public string Identity { get; set; }// 身份证
    public string ProfileImage { get; set; }
    public string ProfileThumbnail { get; set; }
    public string Token { get; set; }// 每次登录成功后会改变，一次登录期间永久有效
    public DateTime LastLoginDateTime { get; set; }// 最新一次登录的时间
    public string LastLoginClientIp { get; set; }// 最新一次登录的客户端Ip
    public string LastLoginDeviceId { get; set; }// 最新一次登录的设备Id
}

public class DataAccountView
{
    public string Id { get; set; }
    public AccountType AccountType { get; set; }// 普通，游客
    public AccountStatus AccountStatus { get; set; }// 正常，禁用
    public string AccountName { get; set; }// 账号名，可以为空，为空时继续检测手机和邮箱字段
    public string PhoneCode { get; set; }// 国家码
    public string PhoneNumber { get; set; }// 手机号码
    public string Email { get; set; }// 邮箱
    public string Password { get; set; }
    public ulong AgentId { get; set; }
    public ulong[] AgentParents { get; set; }// 所有直系代理，包含上面的AgentId在第一个元素中
    public string Name { get; set; }
    public GenderType Gender { get; set; } = GenderType.Unknow;// 性别
    public string Identity { get; set; }// 身份证
    public string ProfileImage { get; set; }
    public string ProfileThumbnail { get; set; }
    public DateTime LastLoginDateTime { get; set; }// 最新一次登录的时间
    public string LastLoginClientIp { get; set; }// 最新一次登录的客户端Ip
    public string LastLoginDeviceId { get; set; }// 最新一次登录的设备Id
    public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
    public string AgentUserName { get; set; }

    public void Init(DataAccount data_account)
    {
        Id = data_account.Id;
        AccountType = data_account.AccountType;
        AccountStatus = data_account.AccountStatus;
        AccountName = data_account.AccountName;
        PhoneCode = data_account.PhoneCode;
        PhoneNumber = data_account.PhoneNumber;
        Email = data_account.Email;
        Password = data_account.Password;
        AgentId = data_account.AgentId;
        AgentParents = data_account.AgentParents;
        Name = data_account.Name;
        Gender = data_account.Gender;
        Identity = data_account.Identity;
        ProfileThumbnail = data_account.ProfileThumbnail;
        LastLoginDateTime = data_account.LastLoginDateTime;
        LastLoginClientIp = data_account.LastLoginClientIp;
        LastLoginDeviceId = data_account.LastLoginDeviceId;

        CreatedTime = data_account.CreatedTime;
        UpdatedTime = data_account.UpdatedTime;
    }
}