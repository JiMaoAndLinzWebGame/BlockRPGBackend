
namespace BlockRPGBackend.WSAPI.Messages
{
    /// <summary>
    /// 
    /// </summary>
    public class QueryBlocks
    {
        /// <summary>
        /// 
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int W { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int H { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public int MapID { get; set; }
    }
}