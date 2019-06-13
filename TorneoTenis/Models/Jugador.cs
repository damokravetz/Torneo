using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TorneoTenis.Models
{
    
    public class Jugador
    {
        [Key]
        public int Id { get; set; }
        public int IdTorneo { get; set; }
        public String nombre { get; set; }
        public int ganados { get; set; }
        public int jugados { get; set; }
    }
}