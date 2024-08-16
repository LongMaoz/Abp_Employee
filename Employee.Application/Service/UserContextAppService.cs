using System.Security.Claims;
using Application.Contracts.IService;
using Microsoft.AspNetCore.Http;

namespace Application.Service;

public class UserContextAppService(IHttpContextAccessor httpContextAccessor) : IUserContextAppService
{

    public ClaimsPrincipal User => httpContextAccessor.HttpContext?.User;

    public string GetCurrentUserId()
    {
        return "";
    }

    public string GetCurrentUserName()
    {
        return User?.Identity?.Name ?? "Unknown";
    }
}