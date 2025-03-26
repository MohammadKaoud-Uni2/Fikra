using Fikra.Helper;
using Fikra.Service.Implementation;
using Fikra.Service.Interface;
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
            services.AddScoped<IPdfService,PdfService>();
            services.AddScoped<IRSAService, RSAService>();
            services.AddSingleton<RSAKeyGenerator>();
            services.AddScoped<ISignatureRepo, SignatureRepo>();
            services.AddScoped<IContractRepo, contractRepo>();
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
