using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TorneoTenis.Models
{
    public class Partido
    {
        public int Id { get; set; }
        public int IdJgdr1 { get; set; }
        public int IdJgdr2 { get; set; }
        public int IdTorneo { get; set; }
        public String pt1 { get; set; }
        public String pt2 { get; set; }
        public int fase { get; set; }
    }
}