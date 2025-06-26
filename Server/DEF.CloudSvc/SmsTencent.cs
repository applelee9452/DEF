//using System;
//using System.Threading.Tasks;
//using TencentCloud.Common;
//using TencentCloud.Common.Profile;
//using TencentCloud.Sms.V20190711;
//using TencentCloud.Sms.V20190711.Models;

//namespace DEF.Cloud;

//public class SmsTencent : ISms
//{
//    SmsClient SmsClient { get; set; }

//    public SmsTencent()
//    {
//        Credential cred = new Credential
//        {
//            SecretId = "",
//            SecretKey = ""
//        };

//        ClientProfile clientProfile = new ClientProfile();
//        HttpProfile httpProfile = new HttpProfile
//        {
//            Endpoint = ("sms.tencentcloudapi.com")
//        };
//        clientProfile.HttpProfile = httpProfile;

//        SmsClient = new SmsClient(cred, "", clientProfile);
//    }


//    async Task<Result> ISms.SendVerificationCode(string native_code, string phone_num, string verification_code)
//    {
//        try
//        {
//            SendSmsRequest req = new()
//            {
//                PhoneNumberSet = new string[] { native_code + phone_num },
//                TemplateID = "",
//                SmsSdkAppid = "",
//                Sign = "",
//                TemplateParamSet = new string[] { "", "", "" },
//                ExtendCode = "0",
//                SessionContext = "",
//                SenderId = ""
//            };
//            req.TemplateParamSet[1] = verification_code;

//            SendSmsResponse resp = await SmsClient.SendSms(req);
//        }
//        catch (Exception)
//        {
//            return Result.Failed;
//        }

//        return Result.Success;
//    }
//}

//<PackageReference Include="TencentCloudSDK" Version="3.0.1166" />