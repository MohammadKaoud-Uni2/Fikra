using Microsoft.AspNetCore.Identity;

using SparkLink.Helper;
using SparkLink.Service.Implementation;
using SparkLink.Service.Interface;

namespace SparkLink.Service
{
    public static  class RepoServices
    {
        public static IServiceCollection AddRepoService(this IServiceCollection services)
        {

            services.AddScoped<IIdentityServices, IdentityServices>();
            services.AddScoped<ITokenRepo, TokenRepo>();
            services.AddScoped(typeof(IGenericRepo<>), (typeof(GenericRepo<>)));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();


            return services;    
        }
        public static IServiceCollection RegisterEmail(this IServiceCollection services,IConfiguration configuration)
        {
            var EmailSettings=new EmailSettings();
            configuration.GetSection("EmailSettings").Bind(EmailSettings);
            services.AddSingleton(EmailSettings);
            return services;
        }
    }
}
