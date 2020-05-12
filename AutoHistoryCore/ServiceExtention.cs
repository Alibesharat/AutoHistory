using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AutoHistory
{
    public static class ServiceExtention
    {

        public static IServiceCollection AddAutoHisorty(this IServiceCollection services)
        {

            IHttpContextAccessor accessor;
            accessor = services.BuildServiceProvider().GetService<IHttpContextAccessor>();
            if (accessor == null)
            {
                var ser = services.AddHttpContextAccessor();
                accessor = ser.BuildServiceProvider().GetService<IHttpContextAccessor>();

            }
            DbContextExtention.SetHttpContextAccsor(accessor);
            var context = services.BuildServiceProvider().GetService<DbContext>();
            //foreach (var entityType in context.Model.GetEntityTypes())
            //{
            //    if (entityType.ClrType.GetCustomAttributes(typeof(HistoryTrackableAttribute),true).Length > 0)
            //    {
            //      context.set
            //        modelBuilder.Entity(entityType.Name).Property<DateTime>("CreatedOn");
            //        modelBuilder.Entity(entityType.Name).Property<DateTime>("LastModifiedOn");
            //    }   
            //}
            
            
            return services;

        }

    }





}
