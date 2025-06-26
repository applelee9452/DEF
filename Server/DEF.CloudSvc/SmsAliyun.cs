using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using System;
using System.Threading.Tasks;

namespace DEF.Cloud;

public class SmsAliyun : ISms
{
    DefaultAcsClient Client { get; set; }

    public SmsAliyun()
    {
        IClientProfile profile = DefaultProfile.GetProfile("cn-hongkong", "t4Moay9a5jb0Yi3l", "yDxhZrCLJaJljE3YjYqyQYXRdWzSW1");
        Client = new DefaultAcsClient(profile);
    }

    Task<Result> ISms.SendVerificationCode(string native_code, string phone_num, string verification_code)
    {
        CommonRequest request = new()
        {
            Method = MethodType.POST,
            Domain = "dysmsapi.aliyuncs.com",
            Version = "2017-05-25",
            Action = "SendSms"
        };
        // request.Protocol = ProtocolType.HTTP;

        request.AddQueryParameters("PhoneNumbers", native_code + phone_num);
        request.AddQueryParameters("SignName", "矮人王家里有矿");
        //request.AddQueryParameters("TemplateCode", "SMS_185560835");
        //request.AddQueryParameters("TemplateParam", template_param);

        if (native_code == "86")
        {
            request.AddQueryParameters("TemplateCode", "SMS_185575849");

            string template_param = "{\"code\":\"5555\"}";
            template_param = template_param.Replace("5555", verification_code);
            request.AddQueryParameters("TemplateParam", template_param);
        }
        else
        {
            request.AddQueryParameters("TemplateCode", "SMS_185560835");

            string template_param = "{\"name\":\"player\", \"code\":\"5555\"}";
            template_param = template_param.Replace("5555", verification_code);
            request.AddQueryParameters("TemplateParam", template_param);
        }

        try
        {
            CommonResponse response = Client.GetCommonResponse(request);
            //Console.WriteLine(System.Text.Encoding.Default.GetString(response.HttpResponse.Content));
        }
        catch (ServerException)
        {
            //Console.WriteLine(e);
            return Task.FromResult(Result.Failed);
        }
        catch (ClientException)
        {
            //Console.WriteLine(e);
            return Task.FromResult(Result.Failed);
        }

        return Task.FromResult(Result.Success);
    }
}