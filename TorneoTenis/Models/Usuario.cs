using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TorneoTenis.Models
{
    public class Usuario
    {

        public int Id { get; set; }
        public String nombre { get; set; }
        public String apellido { get; set; }
        public String email { get; set; }
        public String pass { get; set; }


    }
}