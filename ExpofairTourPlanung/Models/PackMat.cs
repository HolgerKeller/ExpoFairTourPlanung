using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace ExpofairTourPlanung.Models
{
    [Table("PackMat", Schema = "expofair")]
    public partial class PackMat
    {
        //ToDo: Längen Prüfen
        [Key]
        public int Id { get; set; }
        public int? IdTourJob { get; set; }
        public int? IdJob { get; set; }
        public string Article { get; set; }
        public string Caption { get; set; }
        public string MatGroup { get; set; }        
    }
}
