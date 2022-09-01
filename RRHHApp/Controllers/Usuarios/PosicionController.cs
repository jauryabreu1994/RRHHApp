using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using RRHHApp.Context;
using RRHHApp.Models.Usuarios;
using RRHHApp.Extensions;
using System.Linq;

namespace RRHHApp.Controllers.Usuarios
{
    public class PosicionController : Controller
    {
        private MainDbContext db = new MainDbContext();

        // GET: Posicion
        public async Task<ActionResult> Index()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para el Listado de Posicion.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }
            var posicion = db.Posicion.Include(a => a._Departamento);
            return View(await posicion.ToListAsync());
        }

        // GET: Posicion/Create
        public ActionResult Create()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Crear Posicion.", NotificationType.WARNING);
                return RedirectToAction("Index", "Posicion");
            }

            ViewBag.DepartamentoId = new SelectList(db.Departamento.Where(a => !a.Eliminado).ToList(), "Id", "Descripcion");
            return View();
        }

        // POST: Posicion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,DepartamentoId,Descripcion,EsLider,FechaCreacion,FechaModificacion,Estado,Eliminado")] Posicion posicion)
        {
            if (ModelState.IsValid)
            {
                db.Posicion.Add(posicion);
                await db.SaveChangesAsync();
                this.AddNotification("Posicion registrada exitosamente.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.DepartamentoId = new SelectList(db.Departamento.Where(a => !a.Eliminado).ToList(), "Id", "Descripcion", posicion.DepartamentoId);
            this.AddNotification("Favor completar todos los campos", NotificationType.ERROR);
            return View(posicion);
        }

        // GET: Posicion/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Editar Posicion.", NotificationType.WARNING);
                return RedirectToAction("Index", "Posicion");
            }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");;
            }
            Posicion posicion = await db.Posicion.FindAsync(id);
            if (posicion == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartamentoId = new SelectList(db.Departamento.Where(a => !a.Eliminado).ToList(), "Id", "Descripcion", posicion.DepartamentoId);
            return View(posicion);
        }

        // POST: Posicion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,DepartamentoId,Descripcion,EsLider,FechaCreacion,FechaModificacion,Estado,Eliminado")] Posicion posicion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(posicion).State = EntityState.Modified;
                await db.SaveChangesAsync();
                this.AddNotification("Posicion modificada exitosamente.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.DepartamentoId = new SelectList(db.Departamento.Where(a => !a.Eliminado).ToList(), "Id", "Descripcion", posicion.DepartamentoId);
            this.AddNotification("Favor completar todos los campos", NotificationType.ERROR);
            return View(posicion);
        }

        // GET: Posicion/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Eliminar Posicion.", NotificationType.WARNING);
                return RedirectToAction("Index", "Posicion");
            }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");;
            }
            Posicion posicion = await db.Posicion.FindAsync(id);
            ViewBag.DepartamentoId = new SelectList(db.Departamento.Where(a => !a.Eliminado).ToList(), "Id", "Descripcion", posicion.DepartamentoId);
            if (posicion == null)
            {
                return HttpNotFound();
            }
            return View(posicion);
        }

        // POST: Posicion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Posicion posicion = await db.Posicion.FindAsync(id);
            db.Posicion.Remove(posicion);
            await db.SaveChangesAsync();
            this.AddNotification("Posicion eliminada exitosamente.", NotificationType.SUCCESS);
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
