using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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

        static string ip = "Unknown"; 
        static string os = "Unknown";
        static string Browser = "Unknown";
        static string Device = "Unknown";

        protected  override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var EntitiyType in modelBuilder.Model.GetEntityTypes())
            {
                if (EntitiyType.ClrType.GetCustomAttributes(typeof(HistoryTrackableAttribute), true).Length > 0)
                {
                    modelBuilder.Entity(EntitiyType.Name).Property<string>("Hs_Change");
                    modelBuilder.Entity(EntitiyType.Name).Property<Boolean>("IsDelete");
                }
            }
            base.OnModelCreating(modelBuilder);
        }


        /// <summary>
        /// Save Change with SoftDelete Pattern(Logical Delete)
        /// Save Agent info -- OS,Broswer and IpAddress
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public  int SaveChangesWithHistory()
        {
            
            var entries = ChangeTracker.Entries().ToArray();
            Filldata();
            foreach (var entity in entries)
            {
                try
                {

                    HistoryBaseModel model = (HistoryBaseModel)entity.Entity;
                    HistoryViewModel vm = new HistoryViewModel()
                    {
                        AgentIp = ip,
                        AgentOs = os,
                        Device = Device,
                        AgentBrowser = Browser,
                        DateTime = DateTime.Now,
                        State = entity.State.ToString()

                    };
                    List<HistoryViewModel> data = new List<HistoryViewModel>();
                    if (!string.IsNullOrWhiteSpace(model.Hs_Change))
                    {
                        data = JsonSerializer.Deserialize<List<HistoryViewModel>>(model.Hs_Change);

                    }
                    data.Add(vm);

                    var JSON = JsonSerializer.Serialize(data);
                    switch (entity.State)
                    {

                        case EntityState.Deleted:
                            model.IsDeleted = true;
                            entity.State = EntityState.Modified;
                            break;
                        default:
                            break;
                    }
                    model.Hs_Change = JSON;
                }
                catch
                {

                    ;
                }

            }

            return  base.SaveChanges();
        }



        private  void Filldata()
        {
            if (_httpContextAccessor != null)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                try
                {
                    string userAgent = httpContext.Request.Headers["User-Agent"];
                    var uaParser = Parser.GetDefault();
                    ClientInfo c = uaParser.Parse(userAgent);
                    Device = c.Device.ToString();
                    os = c.String;
                    Browser = c.String;
                    if (c.OS.ToString() != "Other")
                    {
                        os = c.OS.ToString();
                    }

                    ip = httpContext.Connection.RemoteIpAddress.ToString();
                    if (ip == "::1" || ip == "127.0.0.1")
                    {
                        ip = Dns.GetHostEntry(Dns.GetHostName())
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
        }

    }
}
