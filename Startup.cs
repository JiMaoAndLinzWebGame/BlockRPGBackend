using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Formatting.Compact;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace BlockRPGBackend
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// 配置
        /// </summary>
        /// <value></value>
        public static IConfiguration Configuration { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IConfiguration>(Configuration);
            //这里就是填写数据库的连接字符串
            services.AddDbContext<MyDbContext>(options => options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));
            #region 配置Swagger
            var basepath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录（绝对，不受工作目录影响，建议采用此方法获取路径）
            var xmlpath = Path.Combine(basepath, Assembly.GetEntryAssembly().GetName().Name + ".xml");
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                /*
                // Enable Swagger examples
                c.OperationFilter<ExamplesOperationFilter>();

                // Enable swagger descriptions
                c.OperationFilter<DescriptionOperationFilter>();

                // Enable swagger response headers
                c.OperationFilter<AddResponseHeadersFilter>();

                // Add (Auth) to action summary
                c.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
                // */
                c.IncludeXmlComments(xmlpath);
            });
            #endregion
            services.AddSignalR();
            services.AddMvc();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(
                c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                });//配置Swagger
            #endregion 

            #region 构建数据库上下文实例
            var builder = new DbContextOptionsBuilder<MyDbContext>();
            builder.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            var dbcontext = new MyDbContext(builder.Options);
            #endregion

            #region WebSocket

            var _Sessions = new List<WSAPI.Session>();

            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(300),
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path != "/ws")
                {
                    await next();
                    return;
                }
                if (!context.WebSockets.IsWebSocketRequest)
                {
                    context.Response.StatusCode = 400;
                    return;
                }
                var ws = await context.WebSockets.AcceptWebSocketAsync();
                await WSAPI.Server.addSession(context.Connection.RemoteIpAddress.ToString(), ws, dbcontext);
            });
            #endregion

            //app.UseStaticFiles();

            loggerFactory.AddFile("Logs/{Date}.log");

            app.UseMvc(routes => routes.MapRoute(name: "Default", template: "{controller}/{action}", defaults: new { controller = "Home", action = "Index" }));//配置默认路由

            app.Run(async (context) => await context.Response.WriteAsync("Hello World!"));

            Console.WriteLine("程序执行!");
        }
    }//End Class
}
