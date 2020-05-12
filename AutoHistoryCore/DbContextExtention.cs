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
