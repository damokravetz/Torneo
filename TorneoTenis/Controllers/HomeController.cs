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
            return View();
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
        [HttpPost]
        public ActionResult Torneos(String email, String pass)
        {
            if (email.Equals("kravetzdamian@gmail.com")&&pass.Equals("1234"))
            {
                ViewBag.Message = "Exito";
            }
            else
            {
                ViewBag.Message = "Fracaso";
            }
            return View();
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
        [HttpPost]
        public ActionResult AgregarUsuario(String nombre, String apellido, String email, String pass)
        {
            Usuario usuario = db.Usuario.SqlQuery("SELECT * FROM dbo.Usuarios WHERE email=@email", new SqlParameter("@email", email)).FirstOrDefault();
            if (usuario == null)
            {
                db.Usuario.Add(new Usuario { nombre = nombre, apellido = apellido, email = email, pass = pass });
                db.SaveChanges();
                ViewBag.Useradd = "Usuario registrado con exito";

            }
            else
            {
                ViewBag.Useradd = "El usuario ya se encuentra registrado";
            }
            return RedirectToAction("Index", "Home");
            //return Index();
        }
    }
}