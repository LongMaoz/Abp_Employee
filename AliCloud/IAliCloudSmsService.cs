using Volo.Abp.Domain.Services;

namespace AliCloud;

public interface IAliCloudSmsService:IDomainService
{
    Task SendCodeAsync(string phone, string code);
}