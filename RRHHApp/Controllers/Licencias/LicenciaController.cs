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
    public class LicenciaController : Controller
    {
        private MainDbContext db = new MainDbContext();

        // GET: Licencia
        public async Task<ActionResult> Index()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.ConfigurarLicencia, Session))
            {
                this.AddNotification("No posees permisos para el Listado de Licencias.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }

            return View(await db.Licencia.Where(a=>!a.Eliminado).ToListAsync());
        }


        // GET: Licencia/Create
        public ActionResult Create()
        {
            ViewBag.Icono = string.Empty;
            return View();
        }

        // POST: Licencia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Descripcion,FechaCreacion,Genero,FechaModificacion,Icono,Es_Principal,Estado,Eliminado")] Licencia licencia)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(licencia.Descripcion))
            {
                db.Licencia.Add(licencia);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");

            }
            else
                this.AddNotification("Descripción no puede estar vacio.", NotificationType.ERROR);
            ViewBag.Icono = licencia.Icono;
            ViewBag.Genero = Convert.ToInt32(licencia.Genero).ToString();

            return View(licencia);
        }

        // GET: Licencia/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Licencia licencia = await db.Licencia.FindAsync(id);
            if (licencia == null)
            {
                return HttpNotFound();
            }

            ViewBag.Icono = licencia.Icono;
            ViewBag.Genero = Convert.ToInt32(licencia.Genero).ToString();
            return View(licencia);
        }

        // POST: Licencia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Descripcion,Es_Principal,Icono,Genero,FechaCreacion,FechaModificacion,Estado,Eliminado")] Licencia licencia)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(licencia.Descripcion))
            {
                db.Entry(licencia).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
                this.AddNotification("Descripción no puede estar vacio.", NotificationType.ERROR);

            ViewBag.Icono = licencia.Icono;
            ViewBag.Genero = Convert.ToInt32(licencia.Genero).ToString();
            return View(licencia);
        }

        // GET: Licencia/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Licencia licencia = await db.Licencia.FindAsync(id);
            if (licencia == null)
            {
                return HttpNotFound();
            }
            return View(licencia);
        }

        // POST: Licencia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {

            if (db.LicenciaConfiguracion.Where(a => a.LicenciaId == id && !a.Eliminado).Count() > 0)
            {
                this.AddNotification("Esta licencia posee configuraciones activas, primero es necesario eliminar las configuraciones de esta licencia", NotificationType.WARNING);
                return RedirectToAction("Index");
            }
            Licencia licencia = await db.Licencia.FindAsync(id);
            licencia.FechaModificacion = DateTime.Now;
            licencia.Eliminado = true;
            licencia.Estado = Models.Enums.EstadoEnum.Inactivo;
            db.Entry(licencia).State = EntityState.Modified;
            db.SaveChanges();
            this.AddNotification("Licencia eliminada exitosamente", NotificationType.SUCCESS);
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
