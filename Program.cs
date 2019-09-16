﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BlockRPGBackend
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            /*
            using (var context = new MyDbContext())
            {
                var order = new Modules.Order { Code = "http://sample.com" };
                context.Order.Add(order);
                context.SaveChanges();
            }
            // */
            CreateWebHostBuilder(args).UseKestrel().Build().Run();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var netenviroment=Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"appsettings.{netenviroment}.json", optional: true)
            .Build();
            return WebHost.CreateDefaultBuilder(args)
                    .UseConfiguration(config)
                    .UseStartup<Startup>();
        }
    }
}
