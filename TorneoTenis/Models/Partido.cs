using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TorneoTenis.Models
{
    public class Partido
    {
        public int Id { get; set; }
        public int IdTorneo { get; set; }
        public String ptganador { get; set; }
        public String ptperdedor { get; set; }
        public String ganador { get; set; }
        public String perdedor { get; set; }
    }
}