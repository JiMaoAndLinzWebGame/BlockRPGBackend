using BlockRPGBackend.APIModules;

namespace BlockRPGBackend.Request.Users
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginRequest : UsersBaseRequest
    {
        /// <summary>
        /// 用户基本登录信息
        /// </summary>
        /// <returns></returns>
        public UserInfo UserInfo = new UserInfo();
    }
}