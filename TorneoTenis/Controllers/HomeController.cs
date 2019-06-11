using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using TorneoTenis.Models;
using System.Data.SqlClient;
namespace TorneoTenis.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private TorneosController tc = new TorneosController();

        public ActionResult Index()
        {
            if (getIdSession()==-1)
            {
                return View();
            }
            else
            {
                return View("Torneos", getTorneos(getIdSession()));
            }
            
        }
        [HttpGet]
        public ActionResult TorneoMultiple(int Id)
        {
            TorneoDatos td=tc.getTorneoDatos(Id, getIdSession());
            int cantj=td.torneo.cantjdrs;
            return View(td);
        }

        public ActionResult AgregarTorneo()
        {
            if (getIdSession() == -1)
            {
                return View("Index");
            }
            else
            {
                return View();
            }
        }

        public ActionResult Torneos()
        {
            if (getIdSession()==-1)
            {
                return View("Index");
            }
            return View(getTorneos(getIdSession()));
        }
    
        public ActionResult TusDatos()
        {
            if (getIdSession() == -1)
            {
                return View("Index");
            }
            else
            {
                Usuario usuario = getUsuario(getIdSession());
                return View(usuario);
            }
        }
        [HttpPost]
        public ActionResult AgregarUsuario(Usuario newUser)
        {
            Usuario usuario = db.Usuario.SqlQuery("SELECT * FROM dbo.Usuarios WHERE email=@email", new SqlParameter("@email", newUser.email)).FirstOrDefault();
            if (usuario == null)
            {
                db.Usuario.Add(new Usuario { nombre = newUser.nombre, apellido = newUser.apellido, email = newUser.email, pass = newUser.pass });
                db.SaveChanges();
                ViewBag.Useradd = "Usuario registrado con exito";

            }
            else
            {
                ViewBag.Useradd = "El usuario ya se encuentra registrado";
            }
            return View("Index");
        }
        [HttpPost]
        public ActionResult Login(String email, String pass)
        {
            Usuario usuario = db.Usuario.SqlQuery("SELECT * FROM dbo.Usuarios WHERE email=@email", new SqlParameter("@email", email)).FirstOrDefault();
            if (usuario != null)
            {
                if (usuario.pass.Equals(pass))
                {
                    Session["idusuario"] = usuario.Id;
                    //HttpCookie miCookie = new HttpCookie("Userid", usuario.Id.ToString());
                    //miCookie.Expires.AddDays(1);
                    //HttpContext.Response.SetCookie(miCookie);
                    //HttpCookie miCookie1 = HttpContext.Request.Cookies["Userid"];
                    return View("Torneos", getTorneos(getIdSession()));
                }

            }
            ViewBag.Login = "Email o Contraseña incorrectos";
            return View("Index");
        }


        [HttpPost]
        public ActionResult GuardarTorneo(String nombre , int cantjgdrs)
        {
            Torneo t = new Torneo { IdUsuario = getIdSession(), nombre = nombre, cantjdrs = cantjgdrs };
            if (getTorneo(t.nombre, getUsuario(t.IdUsuario)) == null)
            {
                db.Torneo.Add(t);
                db.SaveChanges();
                return View("Torneos", getTorneos(getIdSession()));
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
        public Torneo getTorneo(String nombre, Usuario usuario)
        {
            SqlParameter nom = new SqlParameter("@nombre", nombre);
            SqlParameter idusu = new SqlParameter("@idusuario", usuario.Id);
            Torneo t = db.Torneo.SqlQuery("SELECT * FROM dbo.Torneos WHERE nombre=@nombre AND IdUsuario=@idusuario",nom,idusu).FirstOrDefault();
            return t;
        }
        public List<Jugador> getJugadores(Torneo t)
        {
            SqlParameter idtorneo = new SqlParameter("@idtorneo", t.Id);
            List<Jugador> jugadores = db.Jugador.SqlQuery("SELECT * FROM dbo.Jugadores WHERE IdTorneo=@idtorneo", idtorneo).ToList();
            return jugadores;
        }
        public List<Partido> getPartidos(Torneo t)
        {
            SqlParameter idtorneo = new SqlParameter("@idtorneo", t.Id);
            List<Partido> partidos = db.Partido.SqlQuery("SELECT * FROM dbo.Partidos WHERE IdTorneo=@idtorneo", idtorneo).ToList();
            return partidos;
        }
        public Usuario getUsuario(int id)
        {
            Usuario usuario = db.Usuario.SqlQuery("SELECT * FROM dbo.Usuarios WHERE Id=@id", new SqlParameter("@id", id)).FirstOrDefault();
            return usuario;
        }

        public TorneoDatos getTorneoDatos(String nombre)
        {
            int id = getIdSession();
            Usuario usuario = getUsuario(id);
            Torneo torneo = getTorneo(nombre, usuario);
            List<Jugador> jugadores = getJugadores(torneo);
            List<Partido> partidos = getPartidos(torneo);
            TorneoDatos td = new TorneoDatos { torneo = torneo, jugadores = jugadores, partidos = partidos };
            return td;
        }
        public int getIdSession()
        {
            int id;
            if (Session["idusuario"]!=null)
            {
                id =Int32.Parse(Session["idusuario"].ToString());
            }
            else
            {
                id = -1;
            }
            return id;
        }
        public ActionResult salirSession()
        {
            Session.Remove("idusuario");
            return View("Index");
        }
    }
}