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
        public Torneo getTorneo(int idtorneo, Usuario usuario)
        {
            SqlParameter idtor = new SqlParameter("@idtorneo", idtorneo);
            SqlParameter idusu = new SqlParameter("@idusuario", usuario.Id);
            Torneo t = db.Torneo.SqlQuery("SELECT * FROM dbo.Torneos WHERE Id=@idtorneo AND IdUsuario=@idusuario", idtor, idusu).FirstOrDefault();
            return t;
        }
        public Torneo getTorneo(String nombre, Usuario usuario)
        {
            SqlParameter nom = new SqlParameter("@nombre", nombre);
            SqlParameter idusu = new SqlParameter("@idusuario", usuario.Id);
            Torneo t = db.Torneo.SqlQuery("SELECT * FROM dbo.Torneos WHERE nombre=@nombre AND IdUsuario=@idusuario", nom, idusu).FirstOrDefault();
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

        public TorneoDatos getTorneoDatos(int idtorneo, int id)
        {
            Usuario usuario = getUsuario(id);
            Torneo torneo = getTorneo(idtorneo, usuario);
            List<Jugador> jugadores = getJugadores(torneo);
            List<Partido> partidos = getPartidos(torneo);
            TorneoDatos td = new TorneoDatos { torneo = torneo, jugadores = jugadores, partidos = partidos };
            return td;
        }
        public TorneoDatos getTorneoDatos(String nombre, int id)
        {
            Usuario usuario = getUsuario(id);
            Torneo torneo = getTorneo(nombre, usuario);
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
        public ActionResult insertarJugador(String nombretorneo, String nombre, String grupo, int id)
        {
            TorneoDatos td = getTorneoDatos(nombretorneo, id);
            Jugador j = new Jugador { IdTorneo = td.torneo.Id, nombre=nombre, grupo= grupo[0] };
            if(validarJugador(td.torneo, j, td.jugadores))
            {
                agregarJugador(j);
                td.jugadores.Add(j);
            }
            ViewBag.Seccion = "Jugadores";
            return View("TorneoMultiple", td);
        }
        public ActionResult insertarPartido(String jgdr1, String jgdr2, String ptje1, String ptje2, String ganador , String nombretorneo, int id)
        {
            TorneoDatos td = getTorneoDatos(nombretorneo, id);
            Jugador j1 = buscarJugador(jgdr1,td.jugadores);
            Jugador j2 = buscarJugador(jgdr2, td.jugadores);
            Jugador jganador = buscarJugador(ganador, td.jugadores);
            Partido p=null;
            if (j1!=null&&j2!=null )
            {
                p = new Partido{ IdJgdr1 = j1.Id, IdJgdr2 = j2.Id, IdTorneo = td.torneo.Id, pt1=ptje1, pt2=ptje2, fase=estimarFase(td.torneo,td.partidos), IdGanador=jganador.Id };
            }
            if(validarPartido(td.torneo, p, td.partidos))
            {
                agregarPartido(p);
                td.partidos.Add(p);
            }
            ViewBag.Seccion = "Partidos";
            return View("TorneoMultiple", td);
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
        public int estimarFase(Torneo t, List<Partido>ps)
        {
            int fase=-1;
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
            return fase;
        }
        public int faseMenor(int fase)
        {
            int fase1=-1;
            switch (fase)
            {
                case 1:
                    fase1 = 2;
                    break;
                case 2:
                    fase1 = 4;
                    break;
                case 4:
                    fase1 = 8;
                    break;
            }
            return fase1;
        }
        public bool sonGanadores(List<Partido>ps, Partido p, int fase)
        {
            int fase1 = faseMenor(fase);
            int i = 0;
            Partido p1 = null;
            Partido p2 = null;
            bool res = false;
            while (i<ps.Count&&p1==null&&p2==null)
            {
                if ((ps[i].IdJgdr1==p.IdJgdr1|| ps[i].IdJgdr2 == p.IdJgdr1) && ps[i].fase==fase1 && ps[i].IdGanador==p.IdJgdr1)
                {
                    p1 = ps[i];
                }
                if ((ps[i].IdJgdr1 == p.IdJgdr2 || ps[i].IdJgdr2 == p.IdJgdr2) && ps[i].fase == fase1 && ps[i].IdGanador == p.IdJgdr2)
                {
                    p2 = ps[i];
                }
                i++;
            }
            if (p1==null&&p2==null)
            {
                res = true;
            }
            else
            {
                if (p1!=null&&p2!=null)
                {
                    res = true;
                }
            }
            return res;
        }
        public bool validarPartido(Torneo t, Partido p, List<Partido> ps)
        {
            bool res = false;
            bool ganadores=false;//tiene que ser true
            bool haypartido=true;//tiene que ser falso
            bool max=true;//tiene que ser falso
            int maxpartidos = (t.cantjdrs / 2) + (t.cantjdrs / 4) + (t.cantjdrs / 8) + (t.cantjdrs / 16);
            if (maxpartidos <= ps.Count)
            {
                max = false;
            }
            else
            {   
                int fase = estimarFase(t, ps);
                for (int i=0; i<ps.Count;i++)
                {
                    if ((ps[i].IdJgdr1==p.IdJgdr1&&ps[i].fase==fase)|| (ps[i].IdJgdr2 == p.IdJgdr2 && ps[i].fase == fase)|| (ps[i].IdJgdr1 == p.IdJgdr2 && ps[i].fase == fase)|| (ps[i].IdJgdr2 == p.IdJgdr1 && ps[i].fase == fase))
                    {
                        haypartido = true;
                    }
                    else
                    {
                        haypartido=false;
                    }
                }
                ganadores = sonGanadores(ps, p, fase);
            }
            if (haypartido==false&&max==false&&ganadores==true)
            {
                res = true;
            }
            return res; 
        }
        public bool validarJugador(Torneo t, Jugador j, List<Jugador> js)
        {
            bool grupovalido = false;
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
                    if (j.grupo.Equals("a") || j.grupo.Equals("b") || j.grupo.Equals("c") || j.grupo.Equals("d"))
                    {
                        grupovalido = true;
                    }
                    break;
                case 16:
                    if (j.grupo.Equals("a") || j.grupo.Equals("b") || j.grupo.Equals("c") || j.grupo.Equals("d") || j.grupo.Equals("e") || j.grupo.Equals("f") || j.grupo.Equals("g") || j.grupo.Equals("h"))
                    {
                        grupovalido = true;
                    }
                    break;
            }
            bool res;
            bool nombreigual = false;
            int jsgrupo = 0;
            int i = 0;
            if (js.Count >= t.cantjdrs)
            {
                res = false;
            }
            else
            {
                while (i < js.Count && jsgrupo < 2 && nombreigual == false)
                {
                    if (js[i].nombre.Equals(j.nombre))
                    {
                        nombreigual = true;
                    }
                    if (js[i].grupo.Equals(j.grupo))
                    {
                        jsgrupo++;
                    }
                    i++;
                }
                if (nombreigual == false && jsgrupo < 2 && grupovalido == true)
                {
                    res = true;
                }
                else
                {
                    res = false;
                }
            }
            return res;
        }

    }
}
