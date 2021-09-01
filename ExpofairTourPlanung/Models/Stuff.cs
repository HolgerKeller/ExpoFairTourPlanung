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
        [Required(ErrorMessage = "Bitte geben Sie den Mitarbeiternamen ein"), MaxLength(100)]
        public string EmployeeName1 { get; set; }
        [StringLength(100)]
        public string EmployeeName2 { get; set; }
        [StringLength(100)]
        public string EmployeeType { get; set; }
        [Required(ErrorMessage = "Bitte geben Sie eine Mitarbeiternummer ein"), MaxLength(100)]
        public string EmployeeNr { get; set; }
        [StringLength(500)]
        public string Comments { get; set; }
        public bool? IsActiv { get; set; }
        [StringLength(20)]
        public string Status { get; set; }
        [StringLength(300)]
        public string Employer { get; set; }
        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; }
    }
}
