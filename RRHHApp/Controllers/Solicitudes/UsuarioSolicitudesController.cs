using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using RRHHApp.Context;
using RRHHApp.Models.Solicitudes;
using RRHHApp.Extensions;
using RRHHApp.Models.Usuarios;
using System.Collections.Generic;

namespace RRHHApp.Controllers.Solicitudes
{
    public class UsuarioSolicitudesController : Controller
    {
        private MainDbContext db = new MainDbContext();
        private Encriptar_DesEncriptar encriptar_DesEncriptar = new Encriptar_DesEncriptar();
        private GenericController generic = new GenericController();
        // GET: UsuarioSolicitudes
        public async Task<ActionResult> Index()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.UsuarioSolicitud, Session))
            {
                this.AddNotification("No posees permisos para el listado de solicitudes.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }
            int id = Convert.ToInt32(encriptar_DesEncriptar.DesEncriptar(Session["UserID"].ToString()));

            Usuario usuario = await db.Usuario.FindAsync(id);
            ViewBag.UsuarioId = usuario.Id;
            ViewBag.UsuarioNombre = string.Format("{0} {1}", usuario.Nombre, usuario.Apellido);

            var usuarioSolicitudes = await db.UsuarioSolicitudes.Where(a=>a.UsuarioId == id && !a.Eliminado).Include(u => u._Licencia).Include(u => u._Usuario).ToListAsync();

            usuarioSolicitudes = await getSolicitudEstadoEnum(usuarioSolicitudes);

            return View(usuarioSolicitudes);
        }


        private async Task<List<UsuarioSolicitudes>> getSolicitudEstadoEnum(List<UsuarioSolicitudes> usuarioSolicitudes) {

            foreach (var item in usuarioSolicitudes)
            {
                var line = await db.SolicitudEstado.Where(a => a.UsuarioSolicitudId == item.Id).FirstAsync();

                if (line.SupervisorEstado == Models.Enums.SolicitudEstadoEnum.Rechazado ||
                    line.GerenteEstado == Models.Enums.SolicitudEstadoEnum.Rechazado ||
                    line.RRHHEstado == Models.Enums.SolicitudEstadoEnum.Rechazado)
                    item.SolicitudEstado = Models.Enums.SolicitudEstadoEnum.Rechazado;

                else if (line.SupervisorEstado == Models.Enums.SolicitudEstadoEnum.Aprobado &&
                    line.GerenteEstado == Models.Enums.SolicitudEstadoEnum.Aprobado &&
                    line.RRHHEstado == Models.Enums.SolicitudEstadoEnum.Aprobado)
                    item.SolicitudEstado = Models.Enums.SolicitudEstadoEnum.Aprobado;

                else
                    item.SolicitudEstado = Models.Enums.SolicitudEstadoEnum.Pendiente;
            }

            return usuarioSolicitudes;
        }

        // GET: UsuarioSolicitudes/Create
        public async Task<ActionResult> Create()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.UsuarioSolicitud, Session))
            {
                this.AddNotification("No posees permisos para crear una solicitud.", NotificationType.WARNING);
                return RedirectToAction("Index", "UsuarioSolicitudes");
            }

            int id = Convert.ToInt32(encriptar_DesEncriptar.DesEncriptar(Session["UserID"].ToString()));
            Usuario usuario = await db.Usuario.FindAsync(id);
            ViewBag.UsuarioId = usuario.Id;
            ViewBag.UsuarioNombre = string.Format("{0} {1}", usuario.Nombre, usuario.Apellido);
            ViewBag.MinDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            ViewBag.MaxDate = DateTime.Now.Date.AddYears(1).ToString("yyyy-MM-dd");
            ViewBag.FechaDesde = DateTime.Now.Date.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = DateTime.Now.Date.ToString("yyyy-MM-dd");

            

            var licencias = await db.Licencia.Where(a=> !a.Eliminado && (a.Genero == usuario.Genero || a.Genero == Models.Enums.GeneroEnum.Otro)).ToListAsync();
            ViewBag.LicenciaId = new SelectList(licencias.Where(a => !a.Eliminado).ToList(), "Id", "Descripcion");
            foreach (var lic in licencias)
            {
                var DateAvailable = await generic.getDateAvailable(id, lic.Id, DateTime.Now.Date, DateTime.Now.Date.AddDays(-1), usuario.FechaIngreso);
                lic.Dias_Disponibles = DateAvailable.Item1;
            }

            ViewBag.Licencias = licencias;
            return View();
        }

        // POST: UsuarioSolicitudes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UsuarioId,LicenciaId,FechaDesde,FechaHasta,Comentario,FechaCreacion,FechaModificacion,Estado,Eliminado")] UsuarioSolicitudes usuarioSolicitudes)
        {
            int id = Convert.ToInt32(encriptar_DesEncriptar.DesEncriptar(Session["UserID"].ToString()));

            if (ModelState.IsValid)
            {
                var listado = db.UsuarioSolicitudes.Where (a => ((a.FechaDesde <= usuarioSolicitudes.FechaDesde && a.FechaHasta >= usuarioSolicitudes.FechaDesde) ||
                                                                (a.FechaDesde <= usuarioSolicitudes.FechaHasta && a.FechaHasta >= usuarioSolicitudes.FechaHasta)) && 
                                                                !a.Eliminado && a.UsuarioId == id).ToList();

                listado = await getSolicitudEstadoEnum(listado);
                if (listado.Where(a=>a.SolicitudEstado != Models.Enums.SolicitudEstadoEnum.Rechazado).Count() > 0)
                {
                    this.AddNotification("El rango de fecha seleccionada no esta disponible, posee una solicitud.", NotificationType.WARNING);
                }
                else
                {

                    usuarioSolicitudes.SolicitudEstado = Models.Enums.SolicitudEstadoEnum.Pendiente;
                    usuarioSolicitudes.Comentario = string.IsNullOrEmpty(usuarioSolicitudes.Comentario) ? "" : usuarioSolicitudes.Comentario;
                    db.UsuarioSolicitudes.Add(usuarioSolicitudes);
                    var chief = generic.getChief(Session);

                    Usuario usuarioInfo = await db.Usuario.FindAsync(id);
                    var DateAvailable = await generic.getDateAvailable(id, usuarioSolicitudes.LicenciaId, usuarioSolicitudes.FechaDesde, usuarioSolicitudes.FechaHasta, usuarioInfo.FechaIngreso);

                    if (DateAvailable.Item1 <= 0)
                    {
                        this.AddNotification("No posees dias suficientes para realizar esta solicitud. Dias Restantes: " + (DateAvailable.Item2 - DateAvailable.Item3), NotificationType.WARNING);
                    }
                    else
                    {
                        #region SolicitudEstado
                        SolicitudEstado solicitud = new SolicitudEstado()
                        {
                            Id = 0,
                            Eliminado = false,
                            Estado = Models.Enums.EstadoEnum.Pendiente,
                            FechaCreacion = DateTime.Now,
                            FechaModificacion = DateTime.Now,
                            GerenteEstado = Models.Enums.SolicitudEstadoEnum.Pendiente,
                            GerenteId = chief.Item2,
                            RRHHEstado = Models.Enums.SolicitudEstadoEnum.Pendiente,
                            RRHHId = chief.Item3,
                            SupervisorEstado = Models.Enums.SolicitudEstadoEnum.Pendiente,
                            SupervisorId = chief.Item1,
                            UsuarioSolicitudId = usuarioSolicitudes.Id
                        };


                        var licencias = await db.LicenciaConfiguracion.Where(a => a.LicenciaId == usuarioSolicitudes.LicenciaId).ToListAsync();
                        var fecha = DateTime.Now - usuarioInfo.FechaIngreso;
                        var licencia = licencias.Where(a => a.UsuarioTiempo <= fecha.TotalDays).OrderBy(a => a.UsuarioTiempo).LastOrDefault();

                        if (!licencia.RequiereSupervisorAprobacion && !licencia.RequiereGerenteAprobacion && licencia.RequiereRRHHAprobacion)
                            solicitud.GerenteId = solicitud.SupervisorId = solicitud.RRHHId;

                        else if (licencia.RequiereSupervisorAprobacion && !licencia.RequiereGerenteAprobacion && licencia.RequiereRRHHAprobacion)
                            solicitud.GerenteId = solicitud.RRHHId;

                        else if (!licencia.RequiereSupervisorAprobacion && licencia.RequiereGerenteAprobacion && licencia.RequiereRRHHAprobacion)
                            solicitud.SupervisorId = solicitud.GerenteId;

                        else if (licencia.RequiereSupervisorAprobacion && !licencia.RequiereGerenteAprobacion && !licencia.RequiereRRHHAprobacion)
                            solicitud.GerenteId = solicitud.RRHHId = solicitud.SupervisorId;

                        else if (!licencia.RequiereSupervisorAprobacion && licencia.RequiereGerenteAprobacion && !licencia.RequiereRRHHAprobacion)
                            solicitud.SupervisorId = solicitud.RRHHId = solicitud.GerenteId;


                        db.SolicitudEstado.Add(solicitud);
                        await db.SaveChangesAsync();
                        #endregion

                        return RedirectToAction("Index", "UsuarioSolicitudes");
                    }
                }
            }
            
            Usuario usuario = await db.Usuario.FindAsync(id);
            ViewBag.UsuarioId = usuario.Id;
            ViewBag.UsuarioNombre = string.Format("{0} {1}", usuario.Nombre, usuario.Apellido);
            ViewBag.MinDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            ViewBag.MaxDate = DateTime.Now.Date.AddYears(1).ToString("yyyy-MM-dd");
            ViewBag.FechaDesde = usuarioSolicitudes.FechaDesde.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = usuarioSolicitudes.FechaHasta.ToString("yyyy-MM-dd");
            ViewBag.LicenciaId = new SelectList(db.Licencia.Where(a => !a.Eliminado).ToList(), "Id", "Descripcion", usuarioSolicitudes.LicenciaId);

            var licenciaslistado = await db.Licencia.Where(a => !a.Eliminado).ToListAsync();

            foreach (var lic in licenciaslistado)
            {
                var DateAvailable = await generic.getDateAvailable(id, lic.Id, DateTime.Now.Date, DateTime.Now.Date.AddDays(-1), usuario.FechaIngreso);
                lic.Dias_Disponibles = DateAvailable.Item1;
            }

            ViewBag.Licencias = licenciaslistado;

            return View(usuarioSolicitudes);
        }

        // GET: UsuarioSolicitudes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.UsuarioSolicitud, Session))
            {
                this.AddNotification("No posees permisos para eliminar esta solicitud.", NotificationType.WARNING);
                return RedirectToAction("Index", "UsuarioSolicitudes");
            }

            if (id == null)
                return RedirectToAction("Error", "Home"); ;

            UsuarioSolicitudes usuarioSolicitudes = await db.UsuarioSolicitudes.FindAsync(id);
            if (usuarioSolicitudes == null)
                return HttpNotFound();

            return View(usuarioSolicitudes);

        }

        // POST: UsuarioSolicitudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UsuarioSolicitudes usuarioSolicitudes = await db.UsuarioSolicitudes.FindAsync(id);
            usuarioSolicitudes.FechaModificacion = DateTime.Now;
            usuarioSolicitudes.Eliminado = true;
            usuarioSolicitudes.Estado = Models.Enums.EstadoEnum.Inactivo;
            db.Entry(usuarioSolicitudes).State = EntityState.Modified;
            db.SaveChanges();
            this.AddNotification("Solicitud eliminada exitosamente", NotificationType.SUCCESS);
            return RedirectToAction("Index");

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
