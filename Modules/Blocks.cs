using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BlockRPGBackend.Modules
{
    /// <summary>
    /// Blocks
    /// </summary>
    [Table("blocks")]
    public class Blocks
    {
        /// <summary>
        /// ID
        /// </summary>
        /// <value></value>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key, Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 地图ID
        /// </summary>
        /// <value></value>
        [Required, Column("mapid")]
        public int MapId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        [StringLength(maximumLength: 409600)]
        [Required, Column("cells")]
        private string _Cells { get; set; }

        /// <summary>
        /// 区块里的格子数据_Cells 22*14 个 排列方式:[x][y]
        /// </summary>
        /// <value></value>
        [NotMapped]
        public Cell[][] Cells
        {
            get
            {
                if (_Cells == null) return null;
                if (_Cells.Length <= 0) return null;
                return JsonConvert.DeserializeObject<Cell[][]>(_Cells);
            }
            set
            {
                _Cells = JsonConvert.SerializeObject(value);
            }
        }
        // */

        /// <summary>
        /// 坐标_X
        /// </summary>
        /// <value></value>
        [Required, Key, Column("x", Order = 0)]
        public int X { get; set; }

        /// <summary>
        /// 坐标_Y
        /// </summary>
        /// <value></value>
        [Required, Key, Column("y", Order = 1)]
        public int Y { get; set; }
    }//End Class
}