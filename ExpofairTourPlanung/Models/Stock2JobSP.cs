using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ExpofairTourPlanung.Models
{
    [Keyless]
    public partial class Stock2jobSP
    {
        public int? StockType { get; set; }
        public int? Count { get; set; }
        public string Caption { get; set; }
        public string CustomNumber { get; set; }
        public decimal? Weight { get; set; }
        public decimal? SumWeight { get; set; }
    }
}
