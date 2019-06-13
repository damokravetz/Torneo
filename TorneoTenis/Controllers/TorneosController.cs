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
        private AccountController ac = new AccountController();
        
        public ActionResult Torneos()
        {
            if (getSessionId()!=-1)
            {
                List<Torneo> torneos = getTorneos(getSessionId());
                return View(torneos);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult TorneoAdd()
        {
            if (getSessionId()!=-1)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult AddTorneo(String nombre, int cantjgdrs)
        {
            if (getSessionId()!=-1)
            {
                insertarTorneo(nombre, cantjgdrs, getSessionId());
                return RedirectToAction("Torneos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult Torneo(int Id)
        {
            if (getSessionId()!=-1)
            {
                if (TempData["msje1"]!=null|| TempData["msje2"] != null)
                {
                    if (TempData["msje1"] != null)
                    {
                        ViewBag.msje1 = TempData["msje1"];
                    }
                    else
                    {
                        ViewBag.msje2 = TempData["msje2"];
                    }
                }
                TorneoDatos td = getTorneoDatos(Id, getSessionId());
                return View(td);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult AddJugador(String nombre, int Id)
        {
            if (getSessionId()!=-1)
            {
                TempData["msje1"] = insertarJugador(nombre,Id);
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult AddPartido(String jgdr1, String jgdr2, String ptje1, String ptje2, int Id)
        {
            if (getSessionId()!=-1)
            {
                TempData["msje2"] = insertarPartido(jgdr1,jgdr2,ptje1,ptje2,Id);
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult DelTorneo(int Id)
        {
            if (getSessionId() != -1)
            {
                eliminarTorneo(Id);
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet]
        public ActionResult DelPartido(int Id)
        {
            if (getSessionId() != -1)
            {
                TempData["msje2"] = eliminarPartido(Id);
                return Redirect(Request.UrlReferrer.ToString());
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }



        public void insertarTorneo(String nombre, int cantjgdrs, int Idusuario)
        {
            Torneo t = new Torneo { IdUsuario = Idusuario, nombre = nombre, cantjdrs = cantjgdrs };
            db.Torneo.Add(t);
            db.SaveChanges();
        }
        public List<Torneo> getTorneos(int id)
        {
            List<Torneo> torneos = db.Torneo.SqlQuery("SELECT * FROM dbo.Torneos WHERE IdUsuario=@idusuario", new SqlParameter("@idusuario", id)).ToList();
            return torneos;
        }
        public Torneo getTorneo(int Idtorneo, int Idusuario)
        {
            SqlParameter idtor = new SqlParameter("@idtorneo", Idtorneo);
            SqlParameter idusu = new SqlParameter("@idusuario", Idusuario);
            Torneo t = db.Torneo.SqlQuery("SELECT * FROM dbo.Torneos WHERE Id=@idtorneo AND IdUsuario=@idusuario", idtor, idusu).FirstOrDefault();
            return t;
        }
        public List<Jugador> getJugadores(int Idtorneo)
        {
            SqlParameter idtorneo = new SqlParameter("@idtorneo", Idtorneo);
            List<Jugador> jugadores = db.Jugador.SqlQuery("SELECT * FROM dbo.Jugadors WHERE IdTorneo=@idtorneo", idtorneo).ToList();
            return jugadores;
        }
        public List<Partido> getPartidos(int Idtorneo)
        {
            SqlParameter idtorneo = new SqlParameter("@idtorneo", Idtorneo);
            List<Partido> partidos = db.Partido.SqlQuery("SELECT * FROM dbo.Partidoes WHERE IdTorneo=@idtorneo", idtorneo).ToList();
            return partidos;
        }
        public Partido getPartido(int Id)
        {
            SqlParameter idpartido = new SqlParameter("@idpartido", Id);
            Partido p = db.Partido.SqlQuery("SELECT * FROM dbo.Partidoes WHERE Id=@idpartido", idpartido).FirstOrDefault();
            return p;
        }
        public Jugador getJugador(String nombre, int Idtorneo)
        {
            SqlParameter nom = new SqlParameter("@nombre", nombre);
            SqlParameter idtorneo = new SqlParameter("@idtorneo", Idtorneo);
            Jugador jugador = db.Jugador.SqlQuery("SELECT * FROM dbo.Jugadors WHERE IdTorneo=@idtorneo AND nombre=@nombre", idtorneo, nom).FirstOrDefault();
            return jugador;
        }
        public TorneoDatos getTorneoDatos(int Idtorneo, int Idusuario)
        {
            Torneo torneo = getTorneo(Idtorneo, Idusuario);
            List<Jugador> jugadores = getJugadores(Idtorneo);
            List<Partido> partidos = getPartidos(Idtorneo);
            TorneoDatos td = new TorneoDatos { torneo = torneo, jugadores = jugadores, partidos = partidos };
            return td;
        }
        public String insertarJugador(String nombre, int Idtorneo)
        {
            String msje;
            Jugador j = getJugador(nombre, Idtorneo);
            if (j==null)
            {
                db.Jugador.Add(new Jugador {IdTorneo=Idtorneo, nombre=nombre });
                db.SaveChanges();
                msje = "Jugador agregado correctamente";
            }
            else
            {
                msje = "Jugador ya existente";
            }
            return msje;
            
        }
        public String insertarPartido(String jgdr1, String jgdr2, String ptje1, String ptje2, int Idtorneo)
        {
            String msje;
            Jugador j1 = getJugador(jgdr1, Idtorneo);
            Jugador j2 = getJugador(jgdr2, Idtorneo);
            Partido p=null;
            if (j1!=null&&j2!=null&&jgdr1.Equals(jgdr2)==false )
            {
                p = new Partido{ IdTorneo = Idtorneo, ptganador=ptje1, ptperdedor=ptje2, ganador=jgdr1, perdedor=jgdr2 };
                db.Partido.Add(p);
                db.SaveChanges();
                msje = "Partido añadido correctamente";
            }
            else
            {
                msje = "Partido no añadido, los jugadores no se encuentran en el torneo";
            }
            return msje;
        }
        public String eliminarTorneo(int Id)
        {
            Torneo t = new Torneo() { Id = Id };
            db.Torneo.Attach(t);
            db.Torneo.Remove(t);
            db.SaveChanges();
            return "Torneo eliminado";
        }
        public String eliminarPartido(int Id)
        {
            Partido p = new Partido() { Id = Id };
            db.Partido.Attach(p);
            db.Partido.Remove(p);
            db.SaveChanges();
            return "Partido eliminado";
        }
        public int getSessionId()
        {
            int id;
            if (Session["idusuario"] != null)
            {
                id = Int32.Parse(Session["idusuario"].ToString());
            }
            else
            {
                id = -1;
            }
            return id;
        }

    }
}
