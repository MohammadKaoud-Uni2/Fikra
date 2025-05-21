using Fikra.Helper;
using Fikra.Hubs;
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
            services.AddScoped<IStripeService, StripeService>();
            services.AddScoped<IStripeAccountsRepo,StripeAccountRepo>();
            services.AddScoped<IStripeCustomer,StripeCustomerRepo>();
            services.AddScoped<ITransictionRepo,TransactionRepo>();
            services.AddScoped<IIdeaService, IdeaService>();
            services.AddScoped<IMessageRepo, MessageRepo>();
            services.AddScoped<IRequestRepo,RequestRepo>();
            services.AddScoped<IIdeaRating,IdeaRatingService>();
            services.AddScoped<ICVService, CVService>();
            services.AddScoped<IJoinRequestService, JoinRequestService>();
            services.AddSingleton<PresenceTracker>();
            services.AddScoped<ContractHub>();
            services.AddScoped<IPresistanceGroupService,PersistentGroupService>();
            services.AddScoped<IGroupMessageService, GroupMessageService>();
            services.AddScoped<IPenaltyPointService, PenaltyPointService>();
            services.AddScoped<IComplaintService, ComplaintService>();

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
