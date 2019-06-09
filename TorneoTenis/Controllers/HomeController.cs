﻿using System;
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
                return View("Torneos", getTorneos(getIdSession()));
            }
            
        }

        public ActionResult TorneoMultiple()
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
        /*public ActionResult Torneo()
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
        public ActionResult Partidos()
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
        public ActionResult Jugadores()
        {
            if (getIdSession() == -1)
            {
                return View("Index");
            }
            else
            {
                return View();
            }
        }*/
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
        public int ManejarCookie()
        {
            HttpCookie miCookie1 = HttpContext.Request.Cookies["Userid"];
            int id = Int32.Parse(miCookie1.Value);
            return id;
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
        public bool validarPartido(Torneo t, Partido p, List<Partido>ps)
        {
            bool res;
            int maxpartidos = (t.cantjdrs / 2) + (t.cantjdrs/4)+(t.cantjdrs/8)+(t.cantjdrs/16);
            if (maxpartidos>=ps.Count)
            {
                res = false;
            }
            else
            {
                int fase;
                switch (t.cantjdrs)
                {
                    case 2:
                        fase = 1;
                        break;
                    case 4:
                        if (ps.Count < 2)
                        {
                            fase = 2;
                        }
                        else
                        {
                            fase = 1;
                        }
                        break;
                    case 8:
                        if (ps.Count < 4)
                        {
                            fase = 4;
                        }
                        else
                        {
                            if (ps.Count < 6)
                            {
                                fase = 2;
                            }
                            else
                            {
                                fase = 1;
                            }
                        }
                        break;
                    case 16:
                        if (ps.Count < 8)
                        {
                            fase = 8;
                        }
                        else
                        {
                            if (ps.Count < 12)
                            {
                                fase = 4;
                            }
                            else
                            {
                                if (ps.Count < 14)
                                {
                                    fase = 2;
                                }
                                else
                                {
                                    fase = 1;
                                }
                            }
                        }
                        break;
                }
                res= true;
            }
            return res;
        }
        public bool validarJugador(Torneo t, Jugador j)
        {
            bool grupovalido=false;
            switch (t.cantjdrs)
            {
                case 2:
                    if (j.grupo.Equals("a"))
                    {
                        grupovalido = true;
                    }
                    break;
                case 4:
                    if (j.grupo.Equals("a") || j.grupo.Equals("b"))
                    {
                        grupovalido = true;
                    }
                    break;
                case 8:
                    if (j.grupo.Equals("a") || j.grupo.Equals("b")|| j.grupo.Equals("c") || j.grupo.Equals("d"))
                    {
                        grupovalido = true;
                    }
                    break;
                case 16:
                    if (j.grupo.Equals("a") || j.grupo.Equals("b") || j.grupo.Equals("c") || j.grupo.Equals("d")|| j.grupo.Equals("e") || j.grupo.Equals("f") || j.grupo.Equals("g") || j.grupo.Equals("h"))
                    {
                        grupovalido = true;
                    }
                    break;
            }
            bool res;
            bool nombreigual = false;
            int jsgrupo = 0;
            int i = 0;
            List<Jugador> jugadores = getJugadores(t);
            if (jugadores.Count>=t.cantjdrs)
            {
                res= false;
            }
            else
            {
                while (i < jugadores.Count && jsgrupo < 2 && nombreigual == false)
                {
                    if (jugadores[i].nombre.Equals(j.nombre))
                    {
                        nombreigual = true;
                    }
                    if (jugadores[i].grupo.Equals(j.grupo))
                    {
                        jsgrupo++;
                    }
                }
                if (nombreigual == false && jsgrupo < 2 && grupovalido==true)
                {
                    res= true;
                }
                else{
                    res= false;
                }
            }
            return res;
        }

    }
}