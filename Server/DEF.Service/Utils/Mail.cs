using System.Net.Mail;
using System.Threading.Tasks;

namespace DEF;

public class Mail
{
    SmtpClient SmtpClient { get; set; } = new SmtpClient();

    public Mail()
    {
        SmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;// 指定电子邮件发送方式
        SmtpClient.Host = "smtp.exmail.qq.com";// 邮件服务器
        SmtpClient.UseDefaultCredentials = true;
        SmtpClient.Credentials = new System.Net.NetworkCredential("aaa@bb.com", "123456");// 用户名、密码
    }

    public async Task Send(string subject, string body, string strto)
    {
        string strfrom = "aaa@bb.com";

        MailMessage msg = new()
        {
            From = new MailAddress(strfrom, "aaa"),
            Subject = subject,// 邮件标题
            Body = body,// 邮件内容
            BodyEncoding = System.Text.Encoding.UTF8,// 邮件内容编码
            IsBodyHtml = true,// 是否是HTML邮件
            Priority = MailPriority.High,// 邮件优先级
        };

        msg.To.Add(strto);
        //msg.CC.Add(strcc);

        try
        {
            await SmtpClient.SendMailAsync(msg);
            //Logger.LogInformation("发送成功");
        }
        catch (SmtpException)
        {
            //Logger.LogError(new EventId(0), ex, "");
        }
    }
}