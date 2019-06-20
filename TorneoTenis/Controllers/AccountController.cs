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
    public class AccountController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        
        [HttpPost]
        public ActionResult Login(String email, String pass)
        {
            TempData["msje"] = insertarLogin(email,pass);
            if (getSessionId()!=-1)
            {
                return RedirectToAction("Torneos","Torneos");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpPost]
        public ActionResult Register(String nombre, String apellido, String email, String pass1, String pass2)
        {
            TempData["msje"] = insertarUsuario(nombre, apellido, email, pass1, pass2);
            return RedirectToAction("Index", "Home");
        }
        public ActionResult UserData()
        {
            if (getSessionId()!=-1)
            {
                return View(getUsuario(getSessionId()));
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        public ActionResult salirSession()
        {
            Session.Remove("idusuario");
            return RedirectToAction("Index", "Home");
        }

        public String insertarUsuario(String nombre, String apellido, String email, String pass1, String pass2)
        {
            String msje;
            Usuario usuario = db.Usuario.SqlQuery("SELECT * FROM dbo.Usuarios WHERE email=@email", new SqlParameter("@email", email)).FirstOrDefault();
            if (usuario == null&& pass1==pass2)
            {
                db.Usuario.Add(new Usuario { nombre = nombre, apellido = apellido, email = email, pass = pass1 });
                db.SaveChanges();
                msje = "Usuario registrado con exito";
            }
            else
            {
                if (usuario!=null)
                {
                    msje = "El usuario ya se encuentra registrado";
                }
                else
                {
                    msje = "Revisar clave";
                }
            }
            return msje;
        }
        public String insertarLogin(String email, String pass)
        {
            String msje;
            Usuario usuario = db.Usuario.SqlQuery("SELECT * FROM dbo.Usuarios WHERE email=@email", new SqlParameter("@email", email)).FirstOrDefault();
            if (usuario != null && usuario.pass.Equals(pass))
            {
                Session["idusuario"] = usuario.Id;
                msje = "Login exitoso";
            }
            else
            {
                msje = "Email o clave incorrectos";
            }
            return msje;
        }
        public Usuario getUsuario(int id)
        {
            Usuario usuario = db.Usuario.SqlQuery("SELECT * FROM dbo.Usuarios WHERE Id=@id", new SqlParameter("@id", id)).FirstOrDefault();
            return usuario;
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