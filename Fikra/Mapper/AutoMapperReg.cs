namespace Fikra.Mapper
{
    public static class AutoMapperReg
    {
        public static IServiceCollection AutoMapReg(this IServiceCollection services)
        {
           services.AddAutoMapper(typeof(ContractProfile));
       
            return services;
        }
    }
}
