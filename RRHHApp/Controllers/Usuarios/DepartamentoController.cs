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
    public class DepartamentoController : Controller
    {
        private MainDbContext db = new MainDbContext();

        // GET: Departamento
        public async Task<ActionResult> Index()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para el Listado de Departamento.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }
            return View(await db.Departamento.ToListAsync());
        }

        // GET: Departamento/Create
        public ActionResult Create()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Crear Departamento.", NotificationType.WARNING);
                return RedirectToAction("Index", "Departamento");
            }
            return View();
        }

        // POST: Departamento/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Descripcion,FechaCreacion,FechaModificacion,Estado,Eliminado")] Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                db.Departamento.Add(departamento);
                await db.SaveChangesAsync();
                this.AddNotification("Departamento registrada exitosamente.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            this.AddNotification("Favor completar todos los campos", NotificationType.ERROR);
            return View(departamento);
        }

        // GET: Departamento/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Editar Departamento.", NotificationType.WARNING);
                return RedirectToAction("Index", "Departamento");
            }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");;
            }
            Departamento departamento = await db.Departamento.FindAsync(id);
            if (departamento == null)
            {
                return HttpNotFound();
            }
            return View(departamento);
        }

        // POST: Departamento/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Descripcion,FechaCreacion,FechaModificacion,Estado,Eliminado")] Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                departamento.FechaModificacion = DateTime.Now;
                db.Entry(departamento).State = EntityState.Modified;
                await db.SaveChangesAsync();
                this.AddNotification("Departamento modificada exitosamente.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            this.AddNotification("Favor completar todos los campos", NotificationType.ERROR);
            return View(departamento);
        }

        // GET: Departamento/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Eliminar Departamento.", NotificationType.WARNING);
                return RedirectToAction("Index", "Departamento");
            }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");;
            }
            Departamento departamento = await db.Departamento.FindAsync(id);
            if (departamento == null)
            {
                return HttpNotFound();
            }
            return View(departamento);
        }

        // POST: Departamento/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Departamento departamento = await db.Departamento.FindAsync(id);
            departamento.FechaModificacion = DateTime.Now;
            departamento.Estado = Models.Enums.EstadoEnum.Inactivo;
            departamento.Eliminado = true;
            db.Entry(departamento).State = EntityState.Modified;

            await db.SaveChangesAsync();
            this.AddNotification("Departamento eliminada exitosamente.", NotificationType.SUCCESS);
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
