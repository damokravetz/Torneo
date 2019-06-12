using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Web.Mvc;
using System.Data.Entity;
using TorneoTenis.Models;
using System.Data.SqlClient;

namespace TorneoTenis.Controllers
{
    public class TorneosController: Controller
    {
        
        private ApplicationDbContext db = new ApplicationDbContext();
        
        [HttpPost]
        public ActionResult GuardarTorneo(String nombre, int cantjgdrs, int idusuario)
        {
            Torneo t = new Torneo { IdUsuario = idusuario, nombre = nombre, cantjdrs = cantjgdrs };
            if (getTorneo(t.Id, getUsuario(t.IdUsuario)) == null)
            {
                db.Torneo.Add(t);
                db.SaveChanges();
                return View("Torneos", getTorneos(idusuario));
            }
            else
            {
                ViewBag.Guardar = "Ya hay un torneo con ese nombre";
                return View("AgregarTorneo");
            }
        }
        public List<Torneo> getTorneos(int id)
        {
            List<Torneo> torneos = db.Torneo.SqlQuery("SELECT * FROM dbo.Torneos WHERE IdUsuario=@idusuario", new SqlParameter("@idusuario", id)).ToList();
            return torneos;
        }
        public void eliminarTorneo(int Id)
        {
            Torneo t = new Torneo() { Id = Id };
            db.Torneo.Attach(t);
            db.Torneo.Remove(t);
            //db.Torneo.SqlQuery("DELETE * FROM dbo.Torneos WHERE Id=@id", new SqlParameter("@id", Id));
            db.SaveChanges();
        }
        public void eliminarPartido(int Id)
        {
            Partido p = new Partido() { Id = Id };
            db.Partido.Attach(p);
            db.Partido.Remove(p);
            //db.Torneo.SqlQuery("DELETE * FROM dbo.Torneos WHERE Id=@id", new SqlParameter("@id", Id));
            db.SaveChanges();
        }
        public Torneo getTorneo(int idtorneo, Usuario usuario)
        {
            SqlParameter idtor = new SqlParameter("@idtorneo", idtorneo);
            SqlParameter idusu = new SqlParameter("@idusuario", usuario.Id);
            Torneo t = db.Torneo.SqlQuery("SELECT * FROM dbo.Torneos WHERE Id=@idtorneo AND IdUsuario=@idusuario", idtor, idusu).FirstOrDefault();
            return t;
        }
        public List<Jugador> getJugadores(Torneo t)
        {
            SqlParameter idtorneo = new SqlParameter("@idtorneo", t.Id);
            List<Jugador> jugadores = db.Jugador.SqlQuery("SELECT * FROM dbo.Jugadors WHERE IdTorneo=@idtorneo", idtorneo).ToList();
            return jugadores;
        }
        public List<Partido> getPartidos(Torneo t)
        {
            SqlParameter idtorneo = new SqlParameter("@idtorneo", t.Id);
            List<Partido> partidos = db.Partido.SqlQuery("SELECT * FROM dbo.Partidoes WHERE IdTorneo=@idtorneo", idtorneo).ToList();
            return partidos;
        }
        public Usuario getUsuario(int id)
        {
            Usuario usuario = db.Usuario.SqlQuery("SELECT * FROM dbo.Usuarios WHERE Id=@id", new SqlParameter("@id", id)).FirstOrDefault();
            return usuario;
        }
        public TorneoDatos getTorneoDatos(int idtorneo, int id)
        {
            Usuario usuario = getUsuario(id);
            Torneo torneo = getTorneo(idtorneo, usuario);
            List<Jugador> jugadores = getJugadores(torneo);
            List<Partido> partidos = getPartidos(torneo);
            TorneoDatos td = new TorneoDatos { torneo = torneo, jugadores = jugadores, partidos = partidos };
            return td;
        }
        public Jugador buscarJugador(String nombre, List<Jugador> js)
        {
            Jugador j = null;
            int i = 0;
            while (i<js.Count&&j==null)
            {
                if (js[i].nombre.Equals(nombre))
                {
                    j = js[i];
                }
                i++;
            }
            return j;
        }
        public TorneoDatos insertarJugador(String nombre, int Id, int idusuario)
        {
            TorneoDatos td = getTorneoDatos(Id, idusuario);
            Jugador j = new Jugador { IdTorneo = td.torneo.Id, nombre=nombre };
            if(validarJugador(td.torneo, j, td.jugadores))
            {
                agregarJugador(j);
                td.jugadores.Add(j);
            }
            return td;
        }
        public TorneoDatos insertarPartido(String jgdr1, String jgdr2, String ptje1, String ptje2, int Idtorneo, int id)
        {
            TorneoDatos td = getTorneoDatos(Idtorneo, id);
            Jugador j1 = buscarJugador(jgdr1,td.jugadores);
            Jugador j2 = buscarJugador(jgdr2, td.jugadores);
            Partido p=null;
            if (j1!=null&&j2!=null&&jgdr1.Equals(jgdr2)==false )
            {
                p = new Partido{ IdTorneo = td.torneo.Id, ptganador=ptje1, ptperdedor=ptje2, ganador=jgdr1, perdedor=jgdr2 };
                agregarPartido(p);
                td.partidos.Add(p);
            }
            return td;
        }
        public Partido getPartido(int Id)
        {
            SqlParameter idpartido = new SqlParameter("@idpartido", Id);
            Partido p = db.Partido.SqlQuery("SELECT * FROM dbo.Partidoes WHERE Id=@idpartido", idpartido).FirstOrDefault();
            return p;
        }
        public void agregarJugador(Jugador j)
        {
            db.Jugador.Add(j);
            db.SaveChanges();
        }
        public void agregarPartido(Partido p)
        {
            db.Partido.Add(p);
            db.SaveChanges();
        }
        public bool validarJugador(Torneo t, Jugador j, List<Jugador> js)
        {

            bool res=false;
            bool nombreigual = false;
            int i = 0;
            if (js.Count < t.cantjdrs)
            {
                while (i < js.Count && nombreigual == false)
                {
                    if (js[i].nombre.Equals(j.nombre))
                    {
                        nombreigual = true;
                    }
                    i++;
                }
                if (!nombreigual)
                {
                    res = true;
                }
            }
            return res;
        }

    }
}
