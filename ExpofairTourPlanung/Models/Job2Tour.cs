using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ExpofairTourPlanung.Models
{
    [Table("job2Tour", Schema = "expofair")]
    [Index(nameof(IdJob), nameof(InOut), Name = "AK_JOB_IN_OUT", IsUnique = true)]
    public partial class Job2Tour
    {
        [Key]
        public int IdTourJob { get; set; }
        public int? IdTour { get; set; }
        public int IdJob { get; set; }
        public int SplitCounter { get; set; } 
        public int IdJobState { get; set; }
        public int IdProject { get; set; }
        public int? IdAddress { get; set; }
        public int? Ranking { get; set; }
        [StringLength(30)]
        public string Number { get; set; }
        public string Caption { get; set; }
        public string Comment { get; set; }
        public string HeadLine { get; set; }
        [Column(TypeName = "date")]
        public DateTime JobDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? JobDateReturn { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? JobStartTime { get; set; }
        [Required]
        [StringLength(100)]
        public string Service { get; set; }
        [Required]
        [StringLength(100)]
        public string Status { get; set; }
        [StringLength(100)]
        public string TourName { get; set; }
        [StringLength(300)]
        public string Address { get; set; }
        public string AddressTXT { get; set; }
        public string Stock { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? Weight { get; set; }
        [Column(TypeName = "decimal(18, 0)")]
        public decimal? Volume { get; set; }
        [StringLength(200)]
        public string Time { get; set; }
        [Required]
        [Column("In_Out")]
        [StringLength(20)]
        public string InOut { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeliveryTimeStart { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DeliveryTimeEnd { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PickupTimeStart { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? PickupTimeEnd { get; set; }
        [StringLength(100)]
        public string Contact { get; set; }
        [StringLength(100)]
        public string ContactPhone { get; set; }
        [StringLength(200)]
        public string ReadyTime { get; set; }
        public string DeliveryType { get; set; }
    }
}
