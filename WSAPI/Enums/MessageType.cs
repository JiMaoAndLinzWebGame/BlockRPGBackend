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
        queryBlocks = 1
    }//End MessageType

}//End Namespace