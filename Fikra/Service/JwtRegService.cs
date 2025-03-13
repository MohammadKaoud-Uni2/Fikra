using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SparkLink.Helper;
using System.Text;

namespace SparkLink.Service
{
    public static  class JwtRegService
    {
        public static IServiceCollection JwtRegistering(this IServiceCollection services, IConfiguration configuration)
        {
            JwtSettings jwtSettings = new JwtSettings();
            configuration.GetSection("JWT").Bind(jwtSettings);
            jwtSettings.ValidateIssuerSigninngkey = true;
            services.AddSingleton(jwtSettings);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = jwtSettings.ValidateIssuer,
                    ValidateAudience = jwtSettings.ValidateAudience,
                    ValidateLifetime = jwtSettings.ValidateLifeTime,
                    ValidateIssuerSigningKey = jwtSettings.ValidateIssuerSigninngkey,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };


            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SwaggerDoc",
                    Version = "v1"
                });

                options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    Description = "Input your JWT token in this format: Bearer {your token here} to access this API"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = JwtBearerDefaults.AuthenticationScheme
                            },
                            Scheme = "Bearer",
                            Name = "JWT",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
            });
            //services.AddAuthorization(x =>
            //{
            //    x.AddPolicy("CreateStudent", p =>
            //    {
            //        p.RequireClaim("Create Student", true.ToString());
            //    });

            //});

            return services;
        }
    }
}
