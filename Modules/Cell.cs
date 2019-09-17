using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BlockRPGBackend.Modules
{
    /// <summary>
    /// 格子
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// 这个格子在block里的坐标_X
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// 这个格子在block里的坐标_Y
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// 这个格子的资源ID
        /// </summary>
        public int ResID { get; set; }
        /// <summary>
        /// 格子数据
        /// </summary>
        /// <value></value>
        public string Data { get; set; }
    }
}