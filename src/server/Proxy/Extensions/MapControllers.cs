using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ReverseProxy.Extensions;

public static class MapControllers
{
    public static void MapAuthenticationControllers(this IEndpointRouteBuilder app)
    {
        app.MapPost(
            "/login",
            async (UserLogin model,
                UserManager<IdentityUser> userManager,
                IConfiguration configuration) =>
            {
                var user = await userManager.FindByNameAsync(model.Username);

                if (user == null || !await userManager.CheckPasswordAsync(user, model.Password))
                {
                    return Results.Unauthorized();
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    configuration["JwtSettings:Issuer"],
                    configuration["JwtSettings:Audience"],
                    claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds);

                var tokenHandler = new JwtSecurityTokenHandler();
                string jwtToken = tokenHandler.WriteToken(token);

                return Results.Ok(new { token = jwtToken });
            });

        app.MapPost(
            "/register",
            async (UserLogin model,
                UserManager<IdentityUser> userManager,
                IConfiguration configuration) =>
            {
                var existingUser = await userManager.FindByNameAsync(model.Username);

                if (existingUser != null)
                {
                    return Results.BadRequest("Username already exists");
                }

                var user = new IdentityUser
                {
                    UserName = model.Username,
                    Email = model.Username // You can add other properties if necessary
                };

                var result = await userManager.CreateAsync(user, model.Password);

                if (!result.Succeeded)
                {
                    return Results.BadRequest(result.Errors);
                }


                return Results.Ok();
            });
    }

    public class UserLogin
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}