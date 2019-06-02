using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TorneoTenis.Controllers
{
    public class HomeController : Controller
    {
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
    }
}