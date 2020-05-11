using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using UAParser;


namespace AutoHistory
{
    public static class DbContextExtention
    {

        private static IHttpContextAccessor _httpContextAccessor;

        static string ip = "Unknown";
        static string os = "Unknown";
        static string Browser = "Unknown";
        static string Device = "Unknown";

        public static void SetHttpContextAccsor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// UndeltedRecord
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="set"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> Undelited<TEntity>(this DbSet<TEntity> set)
       where TEntity : HistoryBaseModel
        {
            var data = set.AsNoTracking().Where(e => !e.IsDeleted);
            return data.AsQueryable();
        }



        /// <summary>
        /// Save Change with SoftDelete Pattern(Logical Delete)
        /// Save Agent info -- OS,Broswer and IpAddress
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        public static int SaveChangesWithHistory(this DbContext db)
        {

            var entries = db.ChangeTracker.Entries().ToArray();
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

            return db.SaveChanges();
        }


        private static void Filldata()
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
