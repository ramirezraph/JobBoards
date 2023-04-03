using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace JobBoards.Data.Authentication;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var issuer = configuration.GetValue<string>("JwtSettings:Issuer");
        var audience = configuration.GetValue<string>("JwtSettings:Audience");
        var secret = configuration.GetValue<string>("JwtSettings:Secret");
        var expiryMinutes = configuration.GetValue<int>("JwtSettings:ExpiryMinutes");

        if (issuer is null || audience is null || secret is null)
        {
            throw new ArgumentNullException("JWT Settings is not configured.");
        }

        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret))
            };
        });

        return services;
    }
}