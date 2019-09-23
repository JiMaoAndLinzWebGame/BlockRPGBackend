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
using StackExchange.Redis.Extensions.Core.Abstractions;
using System.Threading.Tasks;

namespace BlockRPGBackend.Controllers
{
    /// <summary>
    /// 给应用后端用的接口,桌面端无需调用
    /// </summary>
    [Route("[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly MyDbContext _DbContext;
        private readonly IConfiguration _Configuration;
        private readonly ILogger<UsersController> _Log;
        private readonly IRedisDatabase _Redis;
        private readonly static SHA256 _SHA256hash = SHA256.Create();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbcontext"></param>
        /// <param name="configuration"></param>
        /// <param name="log"></param>
        /// <param name="redis"></param>
        public UsersController(MyDbContext dbcontext, IConfiguration configuration, ILogger<UsersController> log, IRedisCacheClient redis)
        {
            _DbContext = dbcontext;
            _Configuration = configuration;
            _Log = log;
            _Redis = redis.GetDbFromConfiguration();
        }

        private static string ComputeSha256Hash(string rawData)
        {
            var bytes = _SHA256hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
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
            var user = _DbContext.Users.FirstOrDefault(x => x.UserName == request.UserInfo.UserName);
            if (user != null) return new RegisterResponse() { Code = -1, Msg = "用户已存在" };

            var salt = _Configuration.GetValue<string>("salt");
            user = new Modules.Users();
            user.UserName = request.UserInfo.UserName;
            user.Email = request.UserInfo.Email;
            user.Password = string.IsNullOrWhiteSpace(salt) ? request.UserInfo.Password : ComputeSha256Hash(salt + request.UserInfo.Password);
            user.RegisterIP = Request.HttpContext.Connection.RemoteIpAddress.ToString();
            user.RegisterTime = DateTime.Now;
            user.LastLoginTime = DateTime.Now;
            user.status = 0;
            user.Amount = 0;
            _DbContext.Users.Add(user);
            _DbContext.SaveChanges();

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
        public async Task<LoginResponse> Login(LoginRequest request)
        {
            Modules.Users user = null;
            Func<Task<LoginResponse>> f = async () =>
              {
                  if (request.UserInfo.UserID != null) user = _DbContext.Users.FirstOrDefault(x => x.Id == request.UserInfo.UserID);
                  if (!string.IsNullOrWhiteSpace(request.UserInfo.UserName) && user != null) user = _DbContext.Users.FirstOrDefault(x => x.UserName == request.UserInfo.UserName);
                  if (user == null) return new LoginResponse() { Code = -1, Msg = "用户不存在" };
                  var salt = _Configuration.GetValue<string>("salt");
                  var password = string.IsNullOrWhiteSpace(salt) ? request.UserInfo.Password : ComputeSha256Hash(salt + request.UserInfo.Password);
                  if (user.Password != password) return new LoginResponse() { Code = -1, Msg = "密码不正确" };

                  //如果用户的Token不是空的,尝试从redis清理旧的
                  if (!string.IsNullOrWhiteSpace(user.Token))
                  {
                      user = await _Redis.HashGetAsync<Modules.Users>("Users", user.Token);
                      await _Redis.HashDeleteAsync("Users", user.Token);
                  }
                  user.Token = ComputeSha256Hash(salt + DateTime.Now.ToString("yyyyMMddHHmmss"));
                  user.LastLoginTime = DateTime.Now;
                  await _Redis.HashSetAsync<Modules.Users>("Users", user.Token, user);
                  await _DbContext.SaveChangesAsync();
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
              };
            #region 填充返回值
            return await f();
            #endregion
        }
    }//End Class
}