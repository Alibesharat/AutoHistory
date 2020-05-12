using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using UAParser;

namespace AutoHistory
{
    public class DbContextHistoryTrackable : DbContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DbContextHistoryTrackable(IHttpContextAccessor accessor)
        {
            _httpContextAccessor = accessor;
        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var EntitiyType in modelBuilder.Model.GetEntityTypes())
            {
                if (IsTrackable(EntitiyType.ClrType))
                {
                    modelBuilder.Entity(EntitiyType.Name).Property<string>("Hs_Change");
                    modelBuilder.Entity(EntitiyType.Name).Property<bool>("IsDelete");
                }
            }
            base.OnModelCreating(modelBuilder);
        }

        private bool IsTrackable(Type entitiyType)
        {
            return entitiyType.GetCustomAttributes(typeof(HistoryTrackableAttribute), true).Length > 0;


        }


        /// <summary>
        /// Save Change with SoftDelete Pattern(Logical Delete)
        /// Save Agent info -- OS,Broswer and IpAddress
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public int SaveChangesWithHistory()
        {

            var entries = ChangeTracker.Entries().ToArray();
            foreach (var entityEntry in entries)
            {
                try
                {
                    var vm = Filldata(entityEntry.State);

                    List<HistoryViewModel> data = new List<HistoryViewModel>();

                    if (IsTrackable(entityEntry.Entity.GetType()))
                    {
                        var propertyEntry = entityEntry.Property("Hs_CHange");
                        if (!string.IsNullOrWhiteSpace(propertyEntry.CurrentValue.ToString()))
                        {
                            data = JsonSerializer.Deserialize<List<HistoryViewModel>>(propertyEntry.CurrentValue.ToString());

                        }
                        data.Add(vm);
                        var JsonData = JsonSerializer.Serialize(data);
                        propertyEntry.CurrentValue = JsonData;
                        switch (entityEntry.State)
                        {

                            case EntityState.Deleted:
                                entityEntry.Property("IsDelete").CurrentValue = true;
                                entityEntry.State = EntityState.Modified;
                                break;
                            default:
                                break;
                        }
                    }



                }
                catch
                {

                    ;
                }

            }

            return base.SaveChanges();
        }



        private HistoryViewModel Filldata(EntityState state)
        {
            string Unknown = "Unknown";
            HistoryViewModel vm = new HistoryViewModel()
            {
                AgentOs = Unknown,
                AgentIp = Unknown,
                Device = Unknown,
                AgentBrowser = Unknown,
                DateTime = DateTime.Now,
                State = state.ToString()

            };
            if (_httpContextAccessor != null)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                try
                {
                    string userAgent = httpContext.Request.Headers["User-Agent"];
                    var uaParser = Parser.GetDefault();
                    ClientInfo c = uaParser.Parse(userAgent);
                    vm.Device = c.Device.ToString();
                    vm.AgentOs = c.String;
                    vm.AgentBrowser = c.String;
                    if (c.OS.ToString() != "Other")
                    {
                        vm.AgentOs = c.OS.ToString();
                    }

                    vm.AgentIp = httpContext.Connection.RemoteIpAddress.ToString();
                    if (vm.AgentIp == "::1" || vm.AgentIp == "127.0.0.1")
                    {
                        vm.AgentIp = Dns.GetHostEntry(Dns.GetHostName())
                            .AddressList
                            .FirstOrDefault(C => C.AddressFamily ==
                            System.Net.Sockets.AddressFamily.InterNetwork).ToString();
                    }
                }
                catch
                {

                    ;
                }
            }

            return vm;
        }

    }
}
