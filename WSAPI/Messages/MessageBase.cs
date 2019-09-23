namespace BlockRPGBackend.WSAPI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageBase
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public Enums.MessageType MessageType;

        /// <summary>
        /// 参数
        /// </summary>
        public string Params = "";
    }

    /// <summary>
    /// 
    /// </summary>
    public class MessageBaseResponse
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public Enums.MessageType MessageType;

        /// <summary>
        /// 返回代码
        /// </summary>
        public int Code = 0;

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Msg = "";
    }
}