namespace BlockRPGBackend.Response
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class BaseResponse
    {
        /// <summary>
        /// 返回代码
        /// </summary>
        public int Code;
        /// <summary>
        /// 消息
        /// </summary>
        public string Msg;
        
    }
}