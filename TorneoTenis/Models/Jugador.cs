using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TorneoTenis.Models
{
    public class Jugador
    {
        public int Id { get; set; }
        public int IdTorneo { get; set; }
        public String nombre { get; set; }
        public char grupo { get; set; }
    }
}