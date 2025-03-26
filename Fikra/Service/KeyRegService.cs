using Fikra.Helper;

namespace Fikra.Service
{
    public static  class KeyRegService
    {
        public static IServiceCollection RegKeyService(this IServiceCollection services, IConfiguration configuration)
        {
            var _rSAKeyGenerator = new RSAKeyGenerator(configuration);
            _rSAKeyGenerator.GenerateAndSaveKeys();
            return services;
        }

    }
}
