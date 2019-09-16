using BlockRPGBackend.APIModules;
namespace BlockRPGBackend.Response.Users
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginResponse : UsersBaseResponse
    {
        /// <summary>
        /// 用户信息
        /// </summary>
        /// <returns></returns>
        public UserInfo UserInfo = new UserInfo();
    }
}