using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using PruebaPayment.Configuration;
using System.Text;

namespace PruebaPayment.Startup.Configure;

internal static class JwtStartup
{
    internal static void AddJwt(WebApplicationBuilder builder)
    {
        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.Section));

        var jwtSettings = builder.Configuration
            .GetSection(JwtSettings.Section)
            .Get<JwtSettings>() ?? throw new Exception("Could not load jwt config");

        builder.Services.AddAuthorization();
        //builder.Services.AddAuthorizationBuilder()
        //    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        //        .RequireAuthenticatedUser()
        //        .Build());

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ClockSkew = TimeSpan.Zero
                };
            });
    }
}