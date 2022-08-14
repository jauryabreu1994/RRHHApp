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
using RRHHApp.Models.Usuarios;
using RRHHApp.Extensions;

namespace RRHHApp.Controllers.Usuarios
{
    public class UsuarioAsociadoController : Controller
    {
        private MainDbContext db = new MainDbContext();
        private Encriptar_DesEncriptar encriptar_DesEncriptar = new Encriptar_DesEncriptar();

        // GET: UsuarioAsociado
        public async Task<ActionResult> Index(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.SolicitudEstado, Session))
            {
                this.AddNotification("No posees permisos para el Listado de Usuarios Asociados.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }
            if (id != null)
            {
                ViewBag.UsuarioId = id;
                var usuarioAsociado = db.UsuarioAsociado.Include(u => u._Usuario).Include(u => u._Lider).Where(a => a.LiderId == id && a.Estado == Models.Enums.EstadoEnum.Activo);
                return View(await usuarioAsociado.ToListAsync());
            }
            else
            {
                id = Convert.ToInt32(encriptar_DesEncriptar.DesEncriptar(Session["UserID"].ToString()));
                ViewBag.UsuarioId = id;
                var usuarioAsociado = db.UsuarioAsociado.Include(u => u._Usuario).Include(u => u._Lider).Where(a => a.LiderId == id && a.Estado == Models.Enums.EstadoEnum.Activo);
                return View(await usuarioAsociado.ToListAsync());

            /*this.AddNotification("No posees recursos bajo su perfil.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");*/
            }
        }

        // GET: UsuarioAsociado/Create
        public ActionResult Create(int id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.SolicitudEstado, Session))
            {
                this.AddNotification("No posees permisos para Crear Usuarios Asociados.", NotificationType.WARNING);
                return RedirectToAction("Index", "UsuarioAsociado");
            }
            #region Listado de Usuarios
            var list = getList();
            var currentUser = list.Where(a => a.Id == id).First();

            List<Usuario> usersByDepartment = new List<Usuario>();
            if(currentUser.IsCEO)
                usersByDepartment = list.Where(a => a.Id != id).ToList();
            else
                usersByDepartment = list.Where(a => a.Id != id && a.DepartamentoId == currentUser.DepartamentoId).ToList();

            var usersByDepartmentResult = usersByDepartment.Where(p => !db.UsuarioAsociado.Any(p2 => p2.UsuarioId == p.Id && !p2.Eliminado) && !p.IsCEO);

            ViewBag.UsuarioId = new SelectList(usersByDepartmentResult, "Id", "UsuarioId");
            ViewBag.LiderId = new SelectList(list.Where(a => a.Id == id).ToList(), "Id", "UsuarioId", id);
            #endregion
            ViewBag.Id = id;
            return View();
        }

        // POST: UsuarioAsociado/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,LiderId,UsuarioId,FechaCreacion,FechaModificacion,Estado,Eliminado")] UsuarioAsociado usuarioAsociado)
        {
            if (ModelState.IsValid)
            {
                db.UsuarioAsociado.Add(usuarioAsociado);
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { id = usuarioAsociado.LiderId });
            }


            #region Listado de Usuarios
            var list = getList();
            var currentUser = list.Where(a => a.Id == usuarioAsociado.LiderId).First();
            var usersByDepartment = list.Where(a => a.Id != usuarioAsociado.LiderId && a.DepartamentoId == currentUser.DepartamentoId).ToList();
            var usersByDepartmentResult = usersByDepartment.Where(p => !db.UsuarioAsociado.Any(p2 => p2.UsuarioId == p.Id));

            ViewBag.UsuarioId = new SelectList(usersByDepartmentResult, "Id", "UsuarioId");
            ViewBag.LiderId = new SelectList(list.Where(a => a.Id == usuarioAsociado.LiderId).ToList(), "Id", "UsuarioId", usuarioAsociado.LiderId);
            #endregion
            ViewBag.Id = usuarioAsociado.LiderId;
            return View(usuarioAsociado);
        }

        // GET: UsuarioAsociado/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.SolicitudEstado, Session))
            {
                this.AddNotification("No posees permisos para Editar Usuarios Asociados.", NotificationType.WARNING);
                return RedirectToAction("Index", "UsuarioAsociado");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioAsociado usuarioAsociado = await db.UsuarioAsociado.FindAsync(id);
            if (usuarioAsociado == null)
            {
                return HttpNotFound();
            }
            #region Listado de Usuarios
            var list = getList();
            var currentUser = list.Where(a => a.Id == usuarioAsociado.LiderId).First();
            var usersByDepartment = list.Where(a => a.Id != usuarioAsociado.LiderId && a.DepartamentoId == currentUser.DepartamentoId).ToList();
            var usersByDepartmentResult = usersByDepartment.Where(p => !db.UsuarioAsociado.Any(p2 => p2.UsuarioId == p.Id));

            ViewBag.UsuarioId = new SelectList(usersByDepartmentResult, "Id", "UsuarioId");
            ViewBag.LiderId = new SelectList(list.Where(a => a.Id == usuarioAsociado.LiderId).ToList(), "Id", "UsuarioId", usuarioAsociado.LiderId);
            #endregion
            ViewBag.Id = usuarioAsociado.LiderId;
            return View(usuarioAsociado);
        }

        // POST: UsuarioAsociado/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,LiderId,UsuarioId,FechaCreacion,FechaModificacion,Estado,Eliminado")] UsuarioAsociado usuarioAsociado)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuarioAsociado).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index", new { id = usuarioAsociado.LiderId });
            }
            #region Listado de Usuarios
            var list = getList();
            var currentUser = list.Where(a => a.Id == usuarioAsociado.LiderId).First();
            var usersByDepartment = list.Where(a => a.Id != usuarioAsociado.LiderId && a.DepartamentoId == currentUser.DepartamentoId).ToList();
            var usersByDepartmentResult = usersByDepartment.Where(p => !db.UsuarioAsociado.Any(p2 => p2.UsuarioId == p.Id));

            ViewBag.UsuarioId = new SelectList(usersByDepartmentResult, "Id", "UsuarioId");
            ViewBag.LiderId = new SelectList(list.Where(a => a.Id == usuarioAsociado.LiderId).ToList(), "Id", "UsuarioId", usuarioAsociado.LiderId);
            #endregion
            ViewBag.Id = usuarioAsociado.LiderId;
            this.AddNotification("Favor completar todos los campos", NotificationType.ERROR);
            return View(usuarioAsociado);
        }

        // GET: UsuarioAsociado/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.SolicitudEstado, Session))
            {
                this.AddNotification("No posees permisos para Eliminar Usuarios Asociados.", NotificationType.WARNING);
                return RedirectToAction("Index", "UsuarioAsociado");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UsuarioAsociado usuarioAsociado = await db.UsuarioAsociado.FindAsync(id);
            if (usuarioAsociado == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = usuarioAsociado.LiderId;
            return View(usuarioAsociado);
        }

        // POST: UsuarioAsociado/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            UsuarioAsociado usuarioAsociado = await db.UsuarioAsociado.FindAsync(id);
            db.UsuarioAsociado.Remove(usuarioAsociado);
            await db.SaveChangesAsync();
            this.AddNotification("Recurso desvinculado del Lider exitosamente", NotificationType.SUCCESS);
            return RedirectToAction("Index", new { id = usuarioAsociado.LiderId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private List<Usuario> getList()
        {

            List<Usuario> list = db.Usuario.ToList();

            foreach (var item in list)
                item.UsuarioId = string.Format("{0} {1}", item.Nombre, item.Apellido);

            return list;
        }
    }
}
