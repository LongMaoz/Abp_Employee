using Newtonsoft.Json;
using Volo.Abp.Sms;

namespace AliCloud;

public class AliCloudSmsService(ISmsSender smsSender):IAliCloudSmsService
{
    public async Task SendCodeAsync(string phone, string code)
    {
        var templateParam = new { number = code };
        var smsMessage = new SmsMessage(phone, JsonConvert.SerializeObject(templateParam));
        smsMessage.Properties.Add("SignName", "**科技"); // 签名
        smsMessage.Properties.Add("TemplateCode", "**"); // 模板

        await smsSender.SendAsync(smsMessage);
    }
}