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
        public DbSet<Jugador> Jugador { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<ApplicationDbContext>(null);
            base.OnModelCreating(modelBuilder);
        }
    }
}