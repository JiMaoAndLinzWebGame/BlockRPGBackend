using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BlockRPGBackend.Response.Users;
using BlockRPGBackend.Request.Users;
using System;
using BlockRPGBackend.APIModules;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace BlockRPGBackend.Controllers
{
    /// <summary>
    /// 给应用后端用的接口,桌面端无需调用
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext DbContext;
        private readonly IConfiguration Configuration;
        private readonly ILogger<UsersController> Log;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbcontext"></param>
        /// <param name="configuration"></param>
        /// <param name="log"></param>
        public UsersController(MyDbContext dbcontext, IConfiguration configuration, ILogger<UsersController> log)
        {
            DbContext = dbcontext;
            Configuration = configuration;
            Log = log;
        }

        private static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (var sha256hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                var bytes = sha256hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                var builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++) builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public RegisterResponse Register(RegisterRequest request)
        {
            var user = DbContext.Users.FirstOrDefault(x => x.UserName == request.UserInfo.UserName);
            if (user != null) return new RegisterResponse() { Code = -1, Msg = "用户已存在" };
            user = new Modules.Users();
            user.UserName = request.UserInfo.UserName;
            var salt = Configuration.GetValue<string>("salt");
            user.Password = string.IsNullOrWhiteSpace(salt) ? request.UserInfo.Password : ComputeSha256Hash(salt + request.UserInfo.Password);
            user.RegisterIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            user.RegisterTime = DateTime.Now;
            user.LastLoginTime = DateTime.Now;
            user.status = 0;
            user.Amount = 0;
            DbContext.Users.Add(user);
            #region 填充返回值
            return new RegisterResponse()
            {
                Code = 0,
                Msg = "注册成功!",
            };
            #endregion
        }
    }//End Class
}