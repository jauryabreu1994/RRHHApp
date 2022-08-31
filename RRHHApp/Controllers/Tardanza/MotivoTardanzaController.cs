using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using RRHHApp.Context;
using RRHHApp.Models.Tardanza;
using RRHHApp.Extensions;
using RRHHApp.Controllers;

namespace MedicalApp.Controllers.Tardanza
{
    public class MotivoTardanzaController : Controller
    {
        private MainDbContext db = new MainDbContext();

        // GET: MotivoTardanza
        public async Task<ActionResult> Index()
        {
            if (!new GenericController().hasAccess(RRHHApp.Models.Enums.PermisoEnum.Tardanza, Session))
            {
                this.AddNotification("No posees permisos para listar las Tardanzas.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }
            return View(await db.MotivoTardanza.ToListAsync());
        }


        // GET: MotivoTardanza/Create
        public ActionResult Create()
        {
            if (!new GenericController().hasAccess(RRHHApp.Models.Enums.PermisoEnum.Tardanza, Session))
            {
                this.AddNotification("No posees permisos para crear Tardanza.", NotificationType.WARNING);
                return RedirectToAction("Index");
            }
            return View();
        }

        // POST: MotivoTardanza/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Descripcion,FechaCreacion,FechaModificacion,Estado,Eliminado")] MotivoTardanza motivoTardanza)
        {
            if (ModelState.IsValid)
            {
                db.MotivoTardanza.Add(motivoTardanza);
                await db.SaveChangesAsync();
                this.AddNotification("Motivo de Tardanza registrada exitosamente.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            return View(motivoTardanza);
        }

        // GET: MotivoTardanza/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!new GenericController().hasAccess(RRHHApp.Models.Enums.PermisoEnum.Tardanza, Session))
            {
                this.AddNotification("No posees permisos para modificar Tardanza.", NotificationType.WARNING);
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MotivoTardanza motivoTardanza = await db.MotivoTardanza.FindAsync(id);
            if (motivoTardanza == null)
            {
                return HttpNotFound();
            }
            return View(motivoTardanza);
        }

        // POST: MotivoTardanza/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Descripcion,FechaCreacion,FechaModificacion,Estado,Eliminado")] MotivoTardanza motivoTardanza)
        {
            if (ModelState.IsValid)
            {
                db.Entry(motivoTardanza).State = EntityState.Modified;
                await db.SaveChangesAsync();
                this.AddNotification("Motivo de Tardanza modificada exitosamente.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            return View(motivoTardanza);
        }

        // GET: MotivoTardanza/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!new GenericController().hasAccess(RRHHApp.Models.Enums.PermisoEnum.Tardanza, Session))
            {
                this.AddNotification("No posees permisos para eliminar Tardanza.", NotificationType.WARNING);
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MotivoTardanza motivoTardanza = await db.MotivoTardanza.FindAsync(id);
            if (motivoTardanza == null)
            {
                return HttpNotFound();
            }
            return View(motivoTardanza);
        }

        // POST: MotivoTardanza/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            MotivoTardanza motivoTardanza = await db.MotivoTardanza.FindAsync(id);
            motivoTardanza.FechaModificacion = DateTime.Now;
            motivoTardanza.Eliminado = true;
            motivoTardanza.Estado = RRHHApp.Models.Enums.EstadoEnum.Inactivo;
            db.Entry(motivoTardanza).State = EntityState.Modified;
            db.SaveChanges();
            this.AddNotification("Motivo de Tardanza eliminada exitosamente", NotificationType.SUCCESS);
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
