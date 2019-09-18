using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;

namespace BlockRPGBackend.APIModules
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo
    {
        /// <summary>
        /// 用户ID(登录时用户ID或用户名二选一)
        /// </summary>
        /// <value></value>
        public long? UserID { get; set; }

        /// <summary>
        /// 邮箱(注册时传)
        /// </summary>
        /// <value></value>
        public string Email { get; set; }

        /// <summary>
        /// 用户名(登录时用户ID或用户名二选一)
        /// </summary>
        /// <value></value>
        /// [SwaggerRequestExample(typeof(string),typeof(string))]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        /// <value></value>
        public string Password { get; set; }

        /// <summary>
        /// 余额(注册时免传,登录时会返回)
        /// </summary>
        /// <value></value>
        public decimal? Amount { get; set; }

        /// <summary>
        /// Token(注册时免传,登录时会返回)
        /// </summary>
        /// <value></value>

        public string Token { get; set; }
    }

}