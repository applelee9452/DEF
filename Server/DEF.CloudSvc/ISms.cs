using System.Threading.Tasks;

namespace DEF.Cloud;

public interface ISms
{
    
    public Task<Result> SendVerificationCode(string native_code, string phone_num, string verification_code);
}