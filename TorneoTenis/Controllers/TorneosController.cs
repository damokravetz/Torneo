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
            List<Jugador> jugadores = db.Jugador.SqlQuery("SELECT * FROM dbo.Jugadors WHERE IdTorneo=@idtorneo", idtorneo).ToList();
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
        public TorneoDatos insertarJugador(String nombre, String grupo, int Id, int idusuario)
        {
            TorneoDatos td = getTorneoDatos(Id, idusuario);
            Jugador j = new Jugador { IdTorneo = td.torneo.Id, nombre=nombre, grupo= grupo[0] };
            if(validarJugador(td.torneo, j, td.jugadores))
            {
                agregarJugador(j);
                td.jugadores.Add(j);
            }
            return td;
        }
        public TorneoDatos insertarPartido(String jgdr1, String jgdr2, String ptje1, String ptje2, String ganador , String nombretorneo, int id)
        {
            String perdedor;
            if (ganador.Equals(jgdr1))
            {
                perdedor = jgdr2;
            }
            else{
                perdedor = jgdr1;
            }
            TorneoDatos td = getTorneoDatos(nombretorneo, id);
            Jugador j1 = buscarJugador(jgdr1,td.jugadores);
            Jugador j2 = buscarJugador(jgdr2, td.jugadores);
            Jugador jganador = buscarJugador(ganador, td.jugadores);
            Partido p=null;
            if (j1!=null&&j2!=null )
            {
                p = new Partido{ IdJgdr1 = j1.Id, IdJgdr2 = j2.Id, IdTorneo = td.torneo.Id, pt1=ptje1, pt2=ptje2, fase=estimarFase(td.torneo,td.partidos), IdGanador=jganador.Id, };
            }
            if(validarPartido(td.torneo, p, td.partidos, j1,j2))
            {
                agregarPartido(p);
                td.partidos.Add(p);
            }
            return td;
        }
        public void agregarJugador(Jugador j)
        {
            //SqlParameter idtorneo = new SqlParameter("@idtorneo", j.IdTorneo);
            //SqlParameter nombre = new SqlParameter("@nombre", j.nombre);
            //SqlParameter grupo = new SqlParameter("@grupo", j.grupo);
            //db.Jugador.SqlQuery("INSERT INTO dbo.Jugadores (IdTorneo, nombre, grupo), VALUES ("+j.IdTorneo+", "+j.nombre+", "+j.grupo+");");

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
                if (ps[i].ganador.Equals(p.ganador)||ps[i].fase==fase1)
                {
                    p1 = ps[i];
                }
                if (ps[i].ganador.Equals(p.perdedor) && ps[i].fase == fase1)
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
        public bool validarPartido(Torneo t, Partido p, List<Partido> ps, Jugador j1, Jugador j2)
        {
            bool res = false;
            bool gruposvalidos = false;
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
                    if ((ps[i].ganador.Equals(p.ganador)&&ps[i].fase==fase)||(ps[i].ganador.Equals(p.perdedor)&& ps[i].fase == fase)||(ps[i].perdedor.Equals(p.perdedor)&&ps[i].fase == fase)||(ps[i].perdedor.Equals (p.ganador) && ps[i].fase == fase))
                    {
                        haypartido = true;
                    }
                    else
                    {
                        haypartido=false;
                    }
                }
                ganadores = sonGanadores(ps, p, fase);
                gruposvalidos = validarGrupo(t,j1,j2,fase);
            }
            if (haypartido==false&&max==false&&ganadores==true)
            {
                res = true;
            }
            return res; 
        }
        public bool validarGrupo(Torneo t, Jugador j1, Jugador j2, int fase)
        {
            bool res = false;
            switch (fase)
            {
                case 8:
                    if (j1.grupo==j2.grupo)
                    {
                        res = true;
                    }
                    break;
                case 4:
                    if (t.cantjdrs == 16)
                    {
                        switch (j1.grupo)
                        {
                            case 'a':
                                if (j2.grupo=='b')
                                {
                                    res = true;
                                }
                                break;
                            case 'c':
                                if (j2.grupo == 'd')
                                {
                                    res = true;
                                }
                                break;
                            case 'e':
                                if (j2.grupo == 'f')
                                {
                                    res = true;
                                }
                                break;
                            case 'g':
                                if (j2.grupo == 'h')
                                {
                                    res = true;
                                }
                                break;
                            case 'b':
                                if (j2.grupo == 'a')
                                {
                                    res = true;
                                }
                                break;
                            case 'd':
                                if (j2.grupo == 'c')
                                {
                                    res = true;
                                }
                                break;
                            case 'f':
                                if (j2.grupo == 'e')
                                {
                                    res = true;
                                }
                                break;
                            case 'h':
                                if (j2.grupo == 'g')
                                {
                                    res = true;
                                }
                                break;
                        }
                    }
                    else
                    {
                        if (j1.grupo.Equals(j2.grupo))
                        {
                            res = true;
                        }
                    }
                    break;
                case 2:
                    switch (t.cantjdrs)
                    {
                        case 4:
                            if (j1.grupo.Equals(j2.grupo))
                            {
                                res = true;
                            }
                            break;
                        case 8:
                            switch (j1.grupo)
                            {
                                case 'a':
                                    if (j2.grupo=='b')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'c':
                                    if (j2.grupo == 'd')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'b':
                                    if (j2.grupo == 'a')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'd':
                                    if (j2.grupo == 'c')
                                    {
                                        res = true;
                                    }
                                    break;
                            }
                            break;
                        case 16:
                            switch (j1.grupo)
                            {
                                case 'a':
                                    if (j2.grupo=='c'|| j2.grupo == 'd')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'b':
                                    if (j2.grupo == 'c' || j2.grupo == 'd')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'c':
                                    if (j2.grupo == 'a' || j2.grupo == 'b')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'd':
                                    if (j2.grupo == 'a' || j2.grupo == 'b')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'e':
                                    if (j2.grupo == 'g' || j2.grupo == 'h')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'f':
                                    if (j2.grupo == 'g' || j2.grupo == 'h')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'g':
                                    if (j2.grupo == 'e' || j2.grupo == 'f')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'h':
                                    if (j2.grupo == 'e' || j2.grupo == 'f')
                                    {
                                        res = true;
                                    }
                                    break;
                            }
                            break;
                    }
                    break;
                case 1:
                    switch (t.cantjdrs)
                    {
                        case 2:
                            if (j1.grupo.Equals(j2.grupo))
                            {
                                res = true;
                            }
                            break;
                        case 4:
                            if (j1.grupo=='a')
                            {
                                if (j1.grupo=='b')
                                {
                                    res = true;
                                }
                            }
                            else
                            {
                                if (j1.grupo=='b')
                                {
                                    if (j1.grupo=='a')
                                    {
                                        res = true;
                                    }
                                }
                            }
                            break;
                        case 8:
                            switch (j1.grupo)
                            {
                                case 'a':
                                    if (j2.grupo=='c'|| j2.grupo == 'd')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'b':
                                    if (j2.grupo == 'c' || j2.grupo == 'd')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'c':
                                    if (j2.grupo == 'a' || j2.grupo == 'b')
                                    {
                                        res = true;
                                    }
                                    break;
                                case 'd':
                                    if (j2.grupo == 'a' || j2.grupo == 'b')
                                    {
                                        res = true;
                                    }
                                    break;
                            }
                            break;
                        case 16:
                            res = true;
                            break;

                    }
                    break;
            }
            return res;
        }
        public bool validarJugador(Torneo t, Jugador j, List<Jugador> js)
        {
            bool grupovalido = false;
            switch (t.cantjdrs)
            {
                case 2:
                    if (j.grupo=='a')
                    {
                        grupovalido = true;
                    }
                    break;
                case 4:
                    if (j.grupo=='a' || j.grupo=='b')
                    {
                        grupovalido = true;
                    }
                    break;
                case 8:
                    if (j.grupo=='a' || j.grupo == 'b' || j.grupo == 'c' || j.grupo == 'd')
                    {
                        grupovalido = true;
                    }
                    break;
                case 16:
                    if (j.grupo == 'a' || j.grupo == 'b' || j.grupo == 'c' || j.grupo == 'd'|| j.grupo == 'e' || j.grupo == 'f' || j.grupo == 'g' || j.grupo == 'h')
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
