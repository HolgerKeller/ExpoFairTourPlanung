using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ExpofairTourPlanung.Models
{
    [Keyless]
    public partial class ExpoEvent
    {
        public string In_Out { get; set; }
        public string JobType { get; set; }
        public string Address { get; set; }
        public int Count { get; set; }
    }
}
