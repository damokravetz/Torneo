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
        private AccountController ac = new AccountController();

        public ActionResult Index()
        {
            if (getSessionId()!=-1)
            {
                return RedirectToAction("Torneos","Torneos");
            }
            else
            {
                return View();
            }
            
        }

        public int getSessionId()
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