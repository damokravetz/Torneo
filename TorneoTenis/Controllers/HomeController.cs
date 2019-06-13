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
                return View("Torneos", tc.getTorneos(getIdSession()));
            }
            
        }
        public ActionResult TorneoMultiple(int Id)
        {
            if (getIdSession() == -1)
            {
                return View("Index");
            }
            else
            {
                TorneoDatos td = tc.getTorneoDatos(Id, getIdSession());
                return View(td);
            }
            
        }

        [HttpPost]
        public ActionResult agregarPartido(String jgdr1, String jgdr2, String ptje1, String ptje2, int Id)
        {
            if (getIdSession() == -1)
            {
                return View("Index");
            }
            else
            {
                TorneoDatos td = tc.insertarPartido(jgdr1,jgdr2,ptje1,ptje2,Id,getIdSession());
                return RedirectToAction("TorneoMultiple", Id);
            }
            
        }
        [HttpPost]
        public ActionResult agregarJugador(String nombre, int Id)
        {
            if (getIdSession() == -1)
            {
                return View("Index");
            }
            else
            {
                TorneoDatos td = tc.insertarJugador(nombre, Id, getIdSession());
                return View("TorneoMultiple", td);
            }

        }
        [HttpGet]
        public ActionResult eliminarTorneo(int Id)
        {
            if (getIdSession() == -1)
            {
                return View("Index");
            }
            else
            {
                tc.eliminarTorneo(Id);
                return View("Torneos", tc.getTorneos(getIdSession()));
            }
        }
        [HttpGet]
        public ActionResult eliminarPartido(int Id)
        {
            if (getIdSession() == -1)
            {
                return View("Index");
            }
            else
            {
                tc.eliminarPartido(Id);
                return Redirect(Request.UrlReferrer.ToString());

            }
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
            return View(tc.getTorneos(getIdSession()));
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
                    return View("Torneos", tc.getTorneos(getIdSession()));
                }

            }
            ViewBag.Login = "Email o Contraseña incorrectos";
            return View("Index");
        }


        [HttpPost]
        public ActionResult GuardarTorneo(String nombre , int cantjgdrs)
        {
            Torneo t = new Torneo { IdUsuario = getIdSession(), nombre = nombre, cantjdrs = cantjgdrs };
            if (tc.getTorneo(t.Id, getUsuario(t.IdUsuario)) == null)
            {
                db.Torneo.Add(t);
                db.SaveChanges();
                return View("Torneos", tc.getTorneos(getIdSession()));
            }
            else
            {
                ViewBag.Guardar = "Ya hay un torneo con ese nombre";
                return View("AgregarTorneo");
            }
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