using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ExpofairTourPlanung.Models
{
    [Table("Stuff", Schema = "expofair")]
    public partial class Stuff
    {
        [Key]
        public int IdStuff { get; set; }
        [StringLength(100)]
        public string EmployeeName1 { get; set; }
        [StringLength(100)]
        public string EmployeeName2 { get; set; }
        [StringLength(100)]
        public string EmployeeType { get; set; }
        [StringLength(100)]
        public string EmployeeNr { get; set; }
        [StringLength(500)]
        public string Comments { get; set; }
    }
}
