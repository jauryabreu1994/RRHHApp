using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RRHHApp.Context;
using RRHHApp.Models.Solicitudes;
using RRHHApp.Extensions;
using RRHHApp.Models.Usuarios;

namespace RRHHApp.Controllers.Solicitudes
{
    public class SolicitudEstadoController : Controller
    {
        private MainDbContext db = new MainDbContext();
        private GenericController generic = new GenericController();
        // GET: SolicitudEstado
        public async Task<ActionResult> Index()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.SolicitudEstado, Session))
            {
                this.AddNotification("No posees permisos para modificar cambios de estado.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }

            var solicitudEstado = db.SolicitudEstado.Where(a=>!a.Eliminado).Include(s => s._Supervisor).Include(s=>s._Gerente).Include(s=>s._RRHH).Include(s => s._UsuarioSolicitudes);

            foreach (var solicitud in solicitudEstado)
            {
                if (solicitud.RRHHId == 0)
                    solicitud._RRHH = new Usuario();

                if (solicitud.SupervisorId == 0)
                    solicitud._Supervisor = new Usuario();

                if (solicitud.GerenteId == 0)
                    solicitud._Gerente = new Usuario();
            }

            return View(await solicitudEstado.ToListAsync());
        }

        public async Task<ActionResult> Home()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.SolicitudEstado, Session))
            {
                this.AddNotification("No posees permisos para modificar el estado.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }

            int id = Convert.ToInt32(new Encriptar_DesEncriptar().DesEncriptar(Session["UserID"].ToString()));
            Usuario usuarioInfo = await db.Usuario.FindAsync(id);

            if (!usuarioInfo._Posicion.EsLider) 
            {
                this.AddNotification("No posees permisos para acceder a este panel", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }

            var solicitudEstado = db.SolicitudEstado.Where(a => (a.SupervisorId == id || a.GerenteId == id || a.RRHHId == id) &&
                                                                !a.Eliminado).Include(s => s._Supervisor).Include(s => s._Gerente)
                                                                .Include(s => s._RRHH).Include(s => s._UsuarioSolicitudes).ToList();

            List<InformacionSolicitud> listado = new List<InformacionSolicitud>();
            foreach (var solicitud in solicitudEstado)
            {

                var usuario = await db.Usuario.FindAsync(solicitud._UsuarioSolicitudes.UsuarioId);

                InformacionSolicitud info = new InformacionSolicitud()
                {
                    Id = solicitud.Id,
                    UsuarioId = solicitud._UsuarioSolicitudes._Usuario.Id,
                    UsuarioNombre = string.Format("{0} {1}", usuario.Nombre, usuario.Apellido),
                    Licencia = solicitud._UsuarioSolicitudes._Licencia.Descripcion,
                    Comentario = solicitud._UsuarioSolicitudes.Comentario,
                    FechaDesde = solicitud._UsuarioSolicitudes.FechaDesde,
                    FechaHasta = solicitud._UsuarioSolicitudes.FechaHasta,
                    FechaCreacion = solicitud.FechaCreacion
                };
                info.CantidadDias = new GenericController().BusinessDaysUntil(solicitud._UsuarioSolicitudes.FechaDesde, solicitud._UsuarioSolicitudes.FechaHasta);

                if (solicitud.SupervisorId == id){
                    info.FirmaId = solicitud.SupervisorId;
                    info.FirmaNombre = string.Format("{0} {1}", solicitud._Supervisor.Nombre, solicitud._Supervisor.Apellido);
                    info.Estado = solicitud.SupervisorEstado;
                }
                else if (solicitud.GerenteId == id){
                    info.FirmaId = solicitud.GerenteId;
                    info.FirmaNombre = string.Format("{0} {1}", solicitud._Gerente.Nombre, solicitud._Gerente.Apellido);
                    info.Estado = solicitud.GerenteEstado;
                }
                else{
                    info.FirmaId = solicitud.RRHHId;
                    info.FirmaNombre = string.Format("{0} {1}", solicitud._RRHH.Nombre, solicitud._RRHH.Apellido);
                    info.Estado = solicitud.RRHHEstado;
                }

                if (solicitud.SupervisorEstado == Models.Enums.SolicitudEstadoEnum.Rechazado ||
                   solicitud.GerenteEstado == Models.Enums.SolicitudEstadoEnum.Rechazado ||
                   solicitud.RRHHEstado == Models.Enums.SolicitudEstadoEnum.Rechazado)
                    info.Estado = Models.Enums.SolicitudEstadoEnum.Rechazado;

                listado.Add(info);
            }

            return View(listado.ToList());
        }

        public async Task<ActionResult> Authorize(int id) 
        {
            Authorize_Reject_Request(id);
            return RedirectToAction("Home");
        }

        public async Task<ActionResult> Reject(int id)
        {
            Authorize_Reject_Request(id, false);
            return RedirectToAction("Home");
        }

        public async void Authorize_Reject_Request(int id, bool Authorize = true) {


            int userId = Convert.ToInt32(new Encriptar_DesEncriptar().DesEncriptar(Session["UserID"].ToString()));
            var solicitud = db.SolicitudEstado.FindAsync(id).Result;

            var usuario = db.Usuario.FindAsync(solicitud._UsuarioSolicitudes.UsuarioId).Result;
            if (Authorize) {
                var DateAvailable = generic.getDateAvailable(userId, solicitud._UsuarioSolicitudes.LicenciaId,
                                                                   solicitud._UsuarioSolicitudes.FechaDesde, 
                                                                   solicitud._UsuarioSolicitudes.FechaHasta, 
                                                                   usuario.FechaIngreso).Result;
                if (DateAvailable.Item1 < 0)
                {
                    this.AddNotification("Esta solicitud no puede ser Autorizada. No posee sufucientes dias para esta licencia.", NotificationType.SUCCESS);
                }
            }


            if (solicitud.SupervisorId == userId)
                solicitud.SupervisorEstado = (Authorize) ? Models.Enums.SolicitudEstadoEnum.Aprobado : Models.Enums.SolicitudEstadoEnum.Rechazado;

             if (solicitud.GerenteId == userId)
                solicitud.GerenteEstado = (Authorize) ? Models.Enums.SolicitudEstadoEnum.Aprobado : Models.Enums.SolicitudEstadoEnum.Rechazado;

            if (solicitud.RRHHId == userId)
                solicitud.RRHHEstado = (Authorize) ? Models.Enums.SolicitudEstadoEnum.Aprobado : Models.Enums.SolicitudEstadoEnum.Rechazado;

            solicitud.FechaModificacion = DateTime.Now;
            db.Entry(solicitud).State = EntityState.Modified;
            db.SaveChanges();

            this.AddNotification((Authorize) ? "Solicitud Aprobada" : "Solicitud Rechazada", NotificationType.SUCCESS);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
