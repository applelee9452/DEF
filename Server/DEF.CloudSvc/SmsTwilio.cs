using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace DEF.Cloud;

public class SmsTwilio : ISms
{
    
    public SmsTwilio()
    {
        const string account_sid = "";
        const string auth_token = "";

        TwilioClient.Init(account_sid, auth_token);
    }

    
    async Task<Result> ISms.SendVerificationCode(string native_code, string phone_num, string verification_code)
    {
        var message = await MessageResource.CreateAsync(
            body: "Join Earth's mightiest heroes.",
            from: new Twilio.Types.PhoneNumber("+"),
            to: new Twilio.Types.PhoneNumber("+")
        );

        //Trace.TraceInformation(message.Sid);

        return Result.Success;
    }
}