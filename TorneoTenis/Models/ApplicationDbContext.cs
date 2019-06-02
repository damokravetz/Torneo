using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace TorneoTenis.Models
{
    public class ApplicationDbContext: DbContext
    {
        public DbSet<Usuario> Usuario{ get; set;}
        public DbSet<Torneo> Torneo { get; set; }
    }
}