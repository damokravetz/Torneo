using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TorneoTenis.Models
{
    public class TorneoDatos
    {
        public Torneo torneo { get; set; }
        public List<Jugador> jugadores { get; set; }
        public List<Partido> partidos { get; set; }
    }
}