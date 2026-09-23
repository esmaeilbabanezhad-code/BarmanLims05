using System.Security.Claims;
using Barman.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace BarmanLims.Login;

public static class LoginEndpointExtensions
{
    private const string PermissionClaimType = "Barman.Permission";

    public static IEndpointRouteBuilder MapLoginEndpoints(
        this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/account/login", async (
            HttpContext httpContext,
            IUserAccountService userAccountService,
            IEmployeeRoleService employeeRoleService,
            IPermissionService permissionService) =>
        {
            var form =
                await httpContext.Request.ReadFormAsync();

            var username =
                form["Username"].ToString();

            var password =
                form["Password"].ToString();

            var account =
                await userAccountService.AuthenticateAsync(
                    username,
                    password);

            if (account == null)
            {
                return Results.Redirect("/login?error=1");
            }

            var claims = new List<Claim>
            {
                new(
                    ClaimTypes.Name,
                    account.Username),

                new(
                    ClaimTypes.NameIdentifier,
                    account.EmployeeId.ToString())
            };

            // Role Claims
            var employeeRoles =
                await employeeRoleService.GetByEmployeeIdAsync(
                    account.EmployeeId);

            foreach (var employeeRole in employeeRoles)
            {
                if (employeeRole.Role == null ||
                    string.IsNullOrWhiteSpace(employeeRole.Role.Code))
                {
                    continue;
                }

                claims.Add(
                    new Claim(
                        ClaimTypes.Role,
                        employeeRole.Role.Code));
            }

            // Permission Claims
            var permissionCodes =
                await permissionService.GetPermissionCodesAsync(
                    account.EmployeeId);

            foreach (var permissionCode in permissionCodes)
            {
                if (string.IsNullOrWhiteSpace(permissionCode))
                {
                    continue;
                }

                claims.Add(
                    new Claim(
                        PermissionClaimType,
                        permissionCode));
            }

            var identity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            var principal = new ClaimsPrincipal(identity);

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return Results.Redirect("/");
        });

        endpoints.MapPost("/account/logout", async (
            HttpContext httpContext) =>
        {
            await httpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return Results.Redirect("/login");
        });

        return endpoints;
    }
}