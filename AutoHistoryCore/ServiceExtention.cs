using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AutoHistory
{
    public static class ServiceExtention
    {
       
        public static IServiceCollection AddAutoHisorty(this IServiceCollection services)
        {

            IHttpContextAccessor accessor;
            accessor= services.BuildServiceProvider().GetService<IHttpContextAccessor>();
            if (accessor == null)
            {
               var ser= services.AddHttpContextAccessor();
                accessor = ser.BuildServiceProvider().GetService<IHttpContextAccessor>();

            }
            DbContextExtention.SetHttpContextAccsor(accessor);
            return services;

        }

       
    }
}
