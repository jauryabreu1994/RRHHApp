using RRHHApp.Context;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using RRHHApp.Models.Solicitudes;
using System.Collections.Generic;
using RRHHApp.Models.Usuarios;

namespace RRHHApp.Controllers.DashBoard
{
    public class DashBoardController : Controller
    {
        private MainDbContext db = new MainDbContext();
        private Encriptar_DesEncriptar encriptar_DesEncriptar = new Encriptar_DesEncriptar();
        private GenericController generic = new GenericController();
        public ActionResult Index()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Perfil, Session))
            {
                return RedirectToAction("Index", "Login");
            }

            Models.DashBoard.DashBoard dashBoard = new Models.DashBoard.DashBoard();

            int UserID = Convert.ToInt32(encriptar_DesEncriptar.DesEncriptar(Session["UserID"].ToString()));

            
            DateTime date = DateTime.Now.AddMonths(-2);
            ViewBag.Usuarios = db.Usuario.Where(a=>a.FechaIngreso >= date && !a.Eliminado).OrderByDescending(a => a.FechaIngreso).Take(10).ToList();

            DateTime desde = DateTime.Now.Date;
            DateTime hasta = DateTime.Now.Date.AddDays(20);

            var solicitudes = db.UsuarioSolicitudes.Where(a => a.FechaDesde >= desde && a.FechaDesde <= hasta).Include(a=>a._Usuario).Include(a => a._Licencia).ToList();
            solicitudes = solicitudes.Where(a => !a._Usuario.Eliminado).ToList();

            List<UsuarioSolicitudes> listado = new List<UsuarioSolicitudes>();
            foreach (var linea in solicitudes)
            {
                var data = db.SolicitudEstado.Where(a => a.UsuarioSolicitudId == linea.Id).First();
                
                if(data.SupervisorEstado == Models.Enums.SolicitudEstadoEnum.Aprobado &&
                   data.GerenteEstado == Models.Enums.SolicitudEstadoEnum.Aprobado &&
                   data.RRHHEstado == Models.Enums.SolicitudEstadoEnum.Aprobado)
                {
                    listado.Add(linea);
                }
            }

            ViewBag.UsuarioSolicitudes = listado;

            desde = DateTime.Now.Date;
            hasta = DateTime.Now.Date.AddYears(1);

            solicitudes = db.UsuarioSolicitudes.Where(a => a.FechaDesde >= desde && a.FechaDesde <= hasta).Include(a => a._Usuario).Include(a => a._Licencia).ToList();

            listado = new List<UsuarioSolicitudes>();
            foreach (var linea in solicitudes)
            {
                var lineas = db.SolicitudEstado.Where(a => a.UsuarioSolicitudId == linea.Id && (a.SupervisorId == UserID || a.GerenteId == UserID || a.RRHHId == UserID)).ToList();

                var data = new SolicitudEstado();

                if (lineas.Count() > 0)
                    data = lineas.FirstOrDefault();
                else
                    continue;

                if ((data.SupervisorId == UserID && data.SupervisorEstado == Models.Enums.SolicitudEstadoEnum.Aprobado) ||
                    (data.GerenteId == UserID && data.GerenteEstado == Models.Enums.SolicitudEstadoEnum.Aprobado) ||
                    (data.RRHHId == UserID && data.RRHHEstado == Models.Enums.SolicitudEstadoEnum.Aprobado))
                {
                    dashBoard.Cant_Aprobadas++;
                }
                else if ((data.SupervisorId == UserID && data.SupervisorEstado == Models.Enums.SolicitudEstadoEnum.Pendiente) ||
                    (data.GerenteId == UserID && data.GerenteEstado == Models.Enums.SolicitudEstadoEnum.Pendiente) ||
                    (data.RRHHId == UserID && data.RRHHEstado == Models.Enums.SolicitudEstadoEnum.Pendiente))
                {
                    dashBoard.Cant_Pendientes++;
                }

                dashBoard.Cant_Total++;
            }


            Usuario usuario = db.Usuario.FindAsync(UserID).Result;
            var licenciaslistado = db.Licencia.Where(a => !a.Eliminado && a.Es_Principal && (a.Genero == usuario.Genero || a.Genero == Models.Enums.GeneroEnum.Otro)).ToList();
            
            foreach (var lic in licenciaslistado)
            {
                var DateAvailable = generic.getDateAvailable(usuario.Id, lic.Id, DateTime.Now.Date, DateTime.Now.Date.AddDays(-1), usuario.FechaIngreso).Result;
                lic.Dias_Disponibles = DateAvailable.Item1;
            }

            ViewBag.Licencias = licenciaslistado;

            return View(dashBoard);
        }



    }
}