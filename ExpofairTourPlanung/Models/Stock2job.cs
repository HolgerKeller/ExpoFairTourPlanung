using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ExpofairTourPlanung.Models
{
    [Table("stock2job", Schema = "expofair")]
    public partial class Stock2job
    {
        [Key]
        public int IdStock { get; set; }
        public int? IdTourJob { get; set; }
        public int? IdStockType { get; set; }
        public int? Factor { get; set; }
        [StringLength(200)]
        public string Caption { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? Weight { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? Volume { get; set; }
        [StringLength(10)]
        public string Status { get; set; }
    }
}
