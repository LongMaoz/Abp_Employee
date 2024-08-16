using IdentityServer4.Models;
using IdentityServer4.Validation;
using Domain.Entity;
using Domain.IRepository;
using EntityFrameworkCore.Repository;
using Microsoft.AspNetCore.Identity;
using Mysqlx.Session;
using Volo.Abp.Domain.Entities;
using static IdentityModel.OidcConstants;

namespace IdentityServer.Custom
{
    public class CustomResourceOwnerPasswordValidator(
        IEmployeeRepository employeeRepository,
        ILogger<CustomResourceOwnerPasswordValidator> logger
        )
        : IResourceOwnerPasswordValidator
    {
        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var employee = await employeeRepository.GetAsync(x => x.Name == context.UserName || x.Email == context.UserName || x.PhoneNumber == context.UserName);
                if (employee.Status == Domain.Shared.Enums.EmployeeStatus.Disabled)
                {
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "抱歉，该账户已被禁用");
                    return;
                }
                var sub = employee.Id.ToString();
                logger.LogInformation("Credentials validated for username: {username}", context.UserName);
                context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password);
            }
            catch (EntityNotFoundException ex)
            {
                logger.LogInformation("error validated : {message}", ex.Message);

                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "账户名不存在，请重新输入");
            }
        }
    }
}
