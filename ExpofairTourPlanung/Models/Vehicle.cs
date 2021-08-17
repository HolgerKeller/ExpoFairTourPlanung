using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ExpofairTourPlanung.Models
{
    [Table("Vehicle", Schema = "expofair")]
    public partial class Vehicle
    {
        [Key]
        public int IdVehicle { get; set; }
        [StringLength(20)]
        public string VehicleNr { get; set; }
        [StringLength(20)]
        public string VehicleType { get; set; }
        [StringLength(500)]
        public string Comment { get; set; }
        public int? MaxLoad { get; set; }
    }
}
