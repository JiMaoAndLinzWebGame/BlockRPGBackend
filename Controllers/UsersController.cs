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
        private readonly static SHA256 SHA256hash = SHA256.Create();

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
            var bytes = SHA256hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return string.Join("", from b in bytes select b.ToString("x2"));
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

            var salt = Configuration.GetValue<string>("salt");
            user = new Modules.Users();
            user.UserName = request.UserInfo.UserName;
            user.Email = request.UserInfo.Email;
            user.Password = string.IsNullOrWhiteSpace(salt) ? request.UserInfo.Password : ComputeSha256Hash(salt + request.UserInfo.Password);
            user.RegisterIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            user.RegisterTime = DateTime.Now;
            user.LastLoginTime = DateTime.Now;
            user.status = 0;
            user.Amount = 0;
            DbContext.Users.Add(user);
            DbContext.SaveChanges();
            #region 填充返回值
            return new RegisterResponse()
            {
                Code = 0,
                Msg = "注册成功!",
            };
            #endregion
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public LoginResponse Login(LoginRequest request)
        {
            Modules.Users user = null;
            if (request.UserInfo.UserID != null) DbContext.Users.FirstOrDefault(x => x.Id == request.UserInfo.UserID);
            if (!string.IsNullOrWhiteSpace(request.UserInfo.UserName) && user != null) DbContext.Users.FirstOrDefault(x => x.UserName == request.UserInfo.UserName);

            if (user == null) return new LoginResponse() { Code = -1, Msg = "用户不存在" };
            var salt = Configuration.GetValue<string>("salt");
            var password = string.IsNullOrWhiteSpace(salt) ? request.UserInfo.Password : ComputeSha256Hash(salt + request.UserInfo.Password);
            if (user.Password != password) return new LoginResponse() { Code = -1, Msg = "密码不正确" };
            user.Token = ComputeSha256Hash(salt + DateTime.Now.ToString("yyyyMMddHHmmss"));
            DbContext.SaveChanges();
            #region 填充返回值
            return new LoginResponse()
            {
                Code = 0,
                Msg = "成功!",
                UserInfo = new UserInfo()
                {
                    UserID = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Amount = user.Amount,
                    Token = user.Token,
                }
            };
            #endregion
        }
    }//End Class
}