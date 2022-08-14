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
    public class GrupoUsuarioController : Controller
    {
        private MainDbContext db = new MainDbContext();

        // GET: GrupoUsuario
        public async Task<ActionResult> Index()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para el Listado de Grupos de Usuario.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }
            return View(await db.GrupoUsuario.ToListAsync());
        }

        // GET: GrupoUsuario/Create
        public ActionResult Create()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Crear Grupos de Usuario.", NotificationType.WARNING);
                return RedirectToAction("Index", "GrupoUsuario");
            }
            return View();
        }

        // POST: GrupoUsuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,EmpresaId,Descripcion,FechaCreacion,FechaModificacion,Estado,Eliminado")] GrupoUsuario grupoUsuario)
        {
            if (ModelState.IsValid)
            {
                if (db.GrupoUsuario.Where(a => a.Descripcion == grupoUsuario.Descripcion).Count() == 0)
                {
                    db.GrupoUsuario.Add(grupoUsuario);
                    await db.SaveChangesAsync();
                    this.AddNotification("Grupo de Usuario registrado exitosamente.", NotificationType.SUCCESS);
                    return RedirectToAction("Index");
                }else
                    this.AddNotification("Este Grupo de Usuario existe. Escribir otra descripcion", NotificationType.ERROR);
            }

            return View(grupoUsuario);
        }

        // GET: GrupoUsuario/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Editar Grupos de Usuario.", NotificationType.WARNING);
                return RedirectToAction("Index", "GrupoUsuario");
            }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");;
            }
            GrupoUsuario grupoUsuario = await db.GrupoUsuario.FindAsync(id);
            if (grupoUsuario == null)
            {
                return HttpNotFound();
            }
            return View(grupoUsuario);
        }

        // POST: GrupoUsuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,EmpresaId,Descripcion,FechaCreacion,FechaModificacion,Estado,Eliminado")] GrupoUsuario grupoUsuario)
        {
            if (ModelState.IsValid)
            {
                if (db.GrupoUsuario.Where(a => a.Descripcion == grupoUsuario.Descripcion).Count() == 0)
                {
                    db.Entry(grupoUsuario).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    this.AddNotification("Grupo de Usuario modificado exitosamente.", NotificationType.SUCCESS);
                    return RedirectToAction("Index");
                }
                else
                    this.AddNotification("Este Grupo de Usuario existe. Escribir otra descripcion", NotificationType.ERROR);
            }
            return View(grupoUsuario);
        }

        // GET: GrupoUsuario/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Eliminar Grupos de Usuario.", NotificationType.WARNING);
                return RedirectToAction("Index", "GrupoUsuario");
            }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");;
            }
            GrupoUsuario grupoUsuario = await db.GrupoUsuario.FindAsync(id);
            if (grupoUsuario == null)
            {
                return HttpNotFound();
            }
            return View(grupoUsuario);
        }

        // POST: GrupoUsuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            GrupoUsuario grupoUsuario = await db.GrupoUsuario.FindAsync(id);
            grupoUsuario.FechaModificacion = DateTime.Now;
            grupoUsuario.Eliminado = true;
            grupoUsuario.Estado = Models.Enums.EstadoEnum.Inactivo;
            db.Entry(grupoUsuario).State = EntityState.Modified;
            await db.SaveChangesAsync();
            this.AddNotification("Grupo de Usuario eliminado exitosamente", NotificationType.SUCCESS);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> GrupoPermiso(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Error", "Home");;
            }
            List<GrupoPermiso> grupoPermiso = await db.GrupoPermiso.Where(a => a.GrupoUsuarioId == id).ToListAsync();
            var grupo = await db.GrupoUsuario.FindAsync(id);
            var list = db.Permiso.ToList();
            List<SelectListItem> listSelectListItem = new List<SelectListItem>();

            foreach (var item in list)
            {
                SelectListItem selectList = new SelectListItem()
                {
                    Value = item.Id.ToString(),
                    Text = item.Descripcion,
                    Selected = grupoPermiso.Where(a=>a.PermisoId == item.Id).Count() > 0 ? true : false
                };
                listSelectListItem.Add(selectList);
            }

            GrupoPermisoModel grupoPermisoModel = new GrupoPermisoModel();
            grupoPermisoModel.Id = id;
            grupoPermisoModel.Descripcion = grupo.Descripcion;
            grupoPermisoModel.grupoPermisos = listSelectListItem;

            return View(grupoPermisoModel);
        }

        [HttpPost]
        public async Task<ActionResult> GrupoPermiso(int Id, IEnumerable<string> grupoPermisosSeleccionados)
        {
            try
            {
                List<GrupoPermiso> grupoPermiso = await db.GrupoPermiso.Where(a => a.GrupoUsuarioId == Id).ToListAsync();
                foreach (var item in grupoPermiso)
                {
                    db.GrupoPermiso.Remove(item);
                }
                await db.SaveChangesAsync();

                foreach (var item in grupoPermisosSeleccionados)
                {
                    GrupoPermiso grupo = new GrupoPermiso()
                    {
                        Id = 0,
                        Eliminado = false,
                        Estado = Models.Enums.EstadoEnum.Activo,
                        FechaCreacion = DateTime.Now,
                        FechaModificacion = DateTime.Now,
                        GrupoUsuarioId = Id,
                        PermisoId = Convert.ToInt32(item)
                    };

                    db.GrupoPermiso.Add(grupo);
                }
                await db.SaveChangesAsync();

                this.AddNotification("Asignacion de Permisos a grupos completado exitosamente", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                this.AddNotification(ex.Message, NotificationType.ERROR);
                return View();
            }
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
