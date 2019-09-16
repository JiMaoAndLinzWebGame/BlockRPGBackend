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
        public long UserID { get; set; }

        /// <summary>
        /// 用户名(登录时用户ID或用户名二选一)
        /// </summary>
        /// <value></value>
        public string UserName { get; set; }

        /// <summary>
        /// 密码(注册或修改用户信息时传入)
        /// </summary>
        /// <value></value>
        public string Password { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        /// <value></value>
        public decimal Amount { get; set; }
    }

}