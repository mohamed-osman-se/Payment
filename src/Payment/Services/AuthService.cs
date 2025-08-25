using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Payment.Models;

namespace Payment.Services
{
    public class AuthService
    {
        public AuthService() { }
        public static async Task Auth(HttpContext httpContext, User user)
        {

            string role = user.Email == "admin@test.com" ? "Admin" : "User";

            var claims = new List<Claim>
        {
            new Claim("name", user.Name!),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role,role),
            new Claim("UserId", user.Id.ToString()),
        };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }
    }

}
