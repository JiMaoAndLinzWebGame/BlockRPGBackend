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
            user.Password = request.UserInfo.Password;
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