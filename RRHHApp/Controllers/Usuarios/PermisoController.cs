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
    public class PermisoController : Controller
    {
        private MainDbContext db = new MainDbContext();

        // GET: Permisoes
        public async Task<ActionResult> Index()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para el Listado de Permisos.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }
            return View(await db.Permiso.ToListAsync());
        }

        // GET: Permisoes/Create
        public ActionResult Create()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Crear Permisos.", NotificationType.WARNING);
                return RedirectToAction("Index", "Permiso");
            }
            return View();
        }

        // POST: Permisoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Descripcion,FechaCreacion,FechaModificacion,Estado,Eliminado")] Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                if (db.Permiso.Where(a => a.Descripcion == permiso.Descripcion).Count() == 0)
                {
                    db.Permiso.Add(permiso);
                    await db.SaveChangesAsync();
                    this.AddNotification("Permiso registrado exitosamente.", NotificationType.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                    this.AddNotification("Este Permiso existe. Escribir otra descripcion", NotificationType.ERROR);
            }

            return View(permiso);
        }

        // GET: Permisoes/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Editar Permisos.", NotificationType.WARNING);
                return RedirectToAction("Index", "Permiso");
            }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");;
            }
            Permiso permiso = await db.Permiso.FindAsync(id);
            if (permiso == null)
            {
                return HttpNotFound();
            }
            return View(permiso);
        }

        // POST: Permisoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Descripcion,FechaCreacion,FechaModificacion,Estado,Eliminado")] Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                if (db.Permiso.Where(a => a.Descripcion == permiso.Descripcion).Count() == 0)
                {
                    db.Entry(permiso).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    this.AddNotification("Permiso modificado exitosamente.", NotificationType.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                    this.AddNotification("Este Permiso existe. Escribir otra descripcion", NotificationType.ERROR);
            }
            return View(permiso);
        }

        // GET: Permisoes/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Eliminar Permisos.", NotificationType.WARNING);
                return RedirectToAction("Index", "Permiso");
            }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");;
            }
            Permiso permiso = await db.Permiso.FindAsync(id);
            if (permiso == null)
            {
                return HttpNotFound();
            }
            return View(permiso);
        }

        // POST: Permisoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Permiso permiso = await db.Permiso.FindAsync(id);
            permiso.FechaModificacion = DateTime.Now;
            permiso.Eliminado = true;
            permiso.Estado = Models.Enums.EstadoEnum.Inactivo;
            db.Entry(permiso).State = EntityState.Modified;
            await db.SaveChangesAsync();
            this.AddNotification("Permiso eliminado exitosamente", NotificationType.SUCCESS);
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
