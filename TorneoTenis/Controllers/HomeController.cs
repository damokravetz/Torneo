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
        public ActionResult Index()
        {
            if (getIdSession()==-1)
            {
                return View();
            }
            else
            {
                return View("Torneos", getTorneos());
            }
            
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult AgregarTorneo()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Torneos()
        {
            if (getIdSession()==-1)
            {
                return View("Index");
            }
            return View(getTorneos());
        }
        public ActionResult Torneo()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Partidos()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult TusDatos()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Jugadores()
        {
            ViewBag.Message = "Your contact page.";

            return View();
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
            //return RedirectToAction("Index", "Home");
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
                    return View("Torneos", getTorneos());
                }
                
            }
            ViewBag.Login = "Email o Contraseña incorrectos";
            return View("Index");
        }
        [HttpPost]
        public ActionResult GuardarTorneo(String nombre , int cantjgdrs)
        {
            Torneo t = new Torneo { IdUsuario = getIdSession(), nombre = nombre, cantjdrs = cantjgdrs };
            if (getTorneo(t.nombre) == null)
            {
                db.Torneo.Add(t);
                db.SaveChanges();
                return View("Torneos", getTorneos());
            }
            else
            {
                ViewBag.Guardar = "Ya hay un torneo con ese nombre";
                return View("AgregarTorneo");
            }
        }
        public int ManejarCookie()
        {
            HttpCookie miCookie1 = HttpContext.Request.Cookies["Userid"];
            int id = Int32.Parse(miCookie1.Value);
            return id;
        }
        public List<Torneo> getTorneos()
        {
            int id =getIdSession();
            List<Torneo> torneos = db.Torneo.SqlQuery("SELECT * FROM dbo.Torneos WHERE IdUsuario=@idusuario", new SqlParameter("@idusuario", id)).ToList();
            return torneos;
        }
        public Torneo getTorneo(String nombre)
        {
            SqlParameter nom = new SqlParameter("@nombre", nombre);
            SqlParameter idusu = new SqlParameter("@idusuario", getIdSession());
            Torneo t = db.Torneo.SqlQuery("SELECT * FROM dbo.Torneos WHERE nombre=@nombre AND IdUsuario=@idusuario",nom,idusu).FirstOrDefault();
            return t;
        }
        /*public List<Jugador> getJugadores()
        {
            SqlParameter nom = new SqlParameter("@nombre", nombre);
            SqlParameter idusu = new SqlParameter("@idusuario", getIdSession());
            List<Jugador> jugadores = db.Jugador.SqlQuery("SELECT * FROM dbo.Jugadores WHERE IdTorneo=@idtorneo", new SqlParameter("@idtorneo", id)).ToList();
            return jugadores;
        }*/
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