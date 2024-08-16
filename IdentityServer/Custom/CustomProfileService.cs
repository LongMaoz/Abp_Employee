using IdentityModel;
using Domain.Entity;
using Domain.IRepository;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using IdentityServer4.Models;
using IdentityServer4.Services;

namespace IdentityServer.Custom
{
    public class CustomProfileService(IEmployeeRepository employeeRepository) : IProfileService
    {
        
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            //var user = await _userManager.GetUserAsync(context.Subject);
            var value = context.Subject.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject)?.Value;
            if (value != null)
            {
                var id = int.Parse(value);
                var employee = await employeeRepository.GetAsync(id);
                var claims = GetClaims(employee);

                // 添加自定义声明
                //claims.Add(new System.Security.Claims.Claim(JwtClaimTypes.Name, user.UserName));
                context.IssuedClaims.AddRange(claims);
            }
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
            return Task.CompletedTask;
        }


        public IList<Claim> GetClaims(Employee employee) 
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(JwtClaimTypes.Name, employee.Name.ToString()));
            return claims;
        }
    }
}
