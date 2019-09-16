using BlockRPGBackend.APIModules;

namespace BlockRPGBackend.Request.Users
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class RegisterRequest : UsersBaseRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        
        public UserInfo UserInfo = new UserInfo();
    }
}