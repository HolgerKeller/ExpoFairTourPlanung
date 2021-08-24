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
        [Required(ErrorMessage = "Bitte geben Sie das KFZ-Kennzeichen ein"), MaxLength(20)]
       public string VehicleNr { get; set; }
        [StringLength(100)]
        public string VehicleType { get; set; }
        [StringLength(500)]
        public string Comment { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        [Required(ErrorMessage = "Bitte geben Sie die Tonnage ein")]
        public decimal? NetWeight { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? NetVolume { get; set; }
        [StringLength(20)]
        public string Status { get; set; }
        [StringLength(300)]
        public string Owner { get; set; }
        public bool? IsActiv { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "smalldatetime")]
        public DateTime? EndDate { get; set; }
    }
}
