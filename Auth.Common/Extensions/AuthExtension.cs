using Auth.Common.Configurations;
using Auth.Common.Context;
using Auth.Common.Models;
using Auth.Common.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Common.Extensions
{
    public static class AuthExtension
    {
        public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthConfiguration>(configuration.GetSection(nameof(AuthConfiguration)));
            services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("AuthApplicationConnection")));
            services
                .AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddDefaultTokenProviders();
            var authConf = new AuthConfiguration();
            configuration.GetSection(nameof(AuthConfiguration)).Bind(authConf);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = authConf.Issuer,
                    ValidateAudience = true,
                    ValidAudience = authConf.Audience,
                    ValidateLifetime = true,

                    IssuerSigningKey = authConf.GetSymmetricSecurityKey(),
                };
            });

            services.AddTransient<ITokenGetterService, JwtTokenGetterService>();

            return services;
        }
    }
}
