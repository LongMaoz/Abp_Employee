using System.Security.Claims;

namespace Application.Contracts.IService;

public interface IUserContextAppService
{
    ClaimsPrincipal User { get; }
    string GetCurrentUserName();

    string GetCurrentUserId();

}