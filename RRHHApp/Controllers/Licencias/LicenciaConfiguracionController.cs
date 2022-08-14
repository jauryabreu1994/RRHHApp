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
using RRHHApp.Models.Licencias;
using RRHHApp.Extensions;

namespace RRHHApp.Controllers.Licencias
{
    public class LicenciaConfiguracionController : Controller
    {
        private MainDbContext db = new MainDbContext();

        // GET: LicenciaConfiguracion
        public async Task<ActionResult> Index()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.ConfigurarLicencia, Session))
            {
                this.AddNotification("No posees permisos para Configurar las Licencias.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }

            var licenciaConfiguracion = db.LicenciaConfiguracion.Include(l => l._Licencia);
            return View(await licenciaConfiguracion.ToListAsync());
        }


        // GET: LicenciaConfiguracion/Create
        public ActionResult Create()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.ConfigurarLicencia, Session))
            {
                this.AddNotification("No posees permisos para Crear Configuracion de las Licencias.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }

            ViewBag.LicenciaId = new SelectList(db.Licencia, "Id", "Descripcion");
            return View();
        }

        // POST: LicenciaConfiguracion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,LicenciaId,UsuarioTiempo,Cantidad,RequiereSupervisorAprobacion,RequiereGerenteAprobacion,RequiereRRHHAprobacion,FechaCreacion,FechaModificacion,Estado,Eliminado")] LicenciaConfiguracion licenciaConfiguracion)
        {
            if (ModelState.IsValid)
            {
                db.LicenciaConfiguracion.Add(licenciaConfiguracion);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.LicenciaId = new SelectList(db.Licencia, "Id", "Descripcion", licenciaConfiguracion.LicenciaId);
            return View(licenciaConfiguracion);
        }

        // GET: LicenciaConfiguracion/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LicenciaConfiguracion licenciaConfiguracion = await db.LicenciaConfiguracion.FindAsync(id);
            if (licenciaConfiguracion == null)
            {
                return HttpNotFound();
            }
            ViewBag.LicenciaId = new SelectList(db.Licencia, "Id", "Descripcion", licenciaConfiguracion.LicenciaId);
            return View(licenciaConfiguracion);
        }

        // POST: LicenciaConfiguracion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,LicenciaId,UsuarioTiempo,Cantidad,RequiereSupervisorAprobacion,RequiereGerenteAprobacion,RequiereRRHHAprobacion,FechaCreacion,FechaModificacion,Estado,Eliminado")] LicenciaConfiguracion licenciaConfiguracion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(licenciaConfiguracion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.LicenciaId = new SelectList(db.Licencia, "Id", "Descripcion", licenciaConfiguracion.LicenciaId);
            return View(licenciaConfiguracion);
        }

        // GET: LicenciaConfiguracion/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LicenciaConfiguracion licenciaConfiguracion = await db.LicenciaConfiguracion.FindAsync(id);
            if (licenciaConfiguracion == null)
            {
                return HttpNotFound();
            }
            return View(licenciaConfiguracion);
        }

        // POST: LicenciaConfiguracion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            LicenciaConfiguracion licenciaConfiguracion = await db.LicenciaConfiguracion.FindAsync(id);
            db.LicenciaConfiguracion.Remove(licenciaConfiguracion);
            await db.SaveChangesAsync();
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
