using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ExpofairTourPlanung.Models
{
    [Table("Del4Job", Schema = "expofair")]
    public partial class Del4Job
    {
        //ToDo: Längen Prüfen
        [Key]
        public int IdDelJob { get; set; }
        public int IdTourJob { get; set; }
        public int IdJob { get; set; }
        [Column("In_Out")]
        [StringLength(20)]
        public string InOut { get; set; }
        public string Number { get; set; }
        public string Caption { get; set; }
        public string Comment { get; set; }
        [Column(TypeName = "date")]
        public DateTime JobDate { get; set; }
        public DateTime? DeliveryTime { get; set; }
        public string AddressTXT { get; set; }
        public string Time { get; set; }
        public string ReadyTime { get; set; }
        public string Status { get; set; }
        public string StatusDel { get; set; }
        public string Customer { get; set; }
        public string CustomerEmail { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string HeadName { get; set; }
        public string HeadEmail { get; set; }
        public string Contact { get; set; }
        public string ContactPhone { get; set; }
        public string CustomerSignature { get; set; }
        public string HeadSignature { get; set; }
        public string PackMaterial { get; set; }
        public string TransportMaterial { get; set; }
        public DateTime? LastUpdate { get; set; }
    }
}
