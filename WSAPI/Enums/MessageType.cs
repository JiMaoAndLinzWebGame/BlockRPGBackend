namespace BlockRPGBackend.WSAPI.Enums
{
    /// <summary>
    /// WS消息类型
    /// </summary>
    public enum MessageType : int
    {
        /// <summary>
        /// 测试网络
        /// </summary>
        None = 0,
        /// <summary>
        /// 客户端主动查询Blocks
        /// </summary>
        queryBlocks = 1,
        /// <summary>
        /// 登录请求/登录结果返回
        /// </summary>
        Login = 2,
    }//End MessageType

}//End Namespace