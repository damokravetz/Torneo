﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TorneoTenis.Models
{
    public class Torneo
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public String nombre { get; set; }
        public int cantjdrs { get; set; }
    }
}