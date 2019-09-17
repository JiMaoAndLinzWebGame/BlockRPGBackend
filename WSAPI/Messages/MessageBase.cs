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
}