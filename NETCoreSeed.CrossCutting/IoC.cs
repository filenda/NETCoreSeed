namespace NETCoreSeed.CrossCutting
{
    public class IoC
    {
        public void ApplyServices(ref IServiceCollection services)
        {
            //ASP.NET HttpContext dependency
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //Application
            services.AddScoped<AnyGymDataContext, AnyGymDataContext>();
            services.AddTransient<IUow, Uow>();
            services.AddTransient<SmtpOptions, SmtpOptions>();

            //Repositories
            services.AddTransient<IActivityRepository, ActivityRepository>();

        }
    }
}