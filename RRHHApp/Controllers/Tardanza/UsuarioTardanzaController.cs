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
using RRHHApp.Models.Tardanza;
using RRHHApp.Controllers;
using RRHHApp.Extensions;
using RRHHApp.Models.Usuarios;

namespace MedicalApp.Controllers.Tardanza
{
    public class UsuarioTardanzaController : Controller
    {
        private MainDbContext db = new MainDbContext();
        private Encriptar_DesEncriptar encriptar_DesEncriptar = new Encriptar_DesEncriptar();

        // GET: UsuarioTardanza
        public async Task<ActionResult> Home(int id)
        {
            if (!new GenericController().hasAccess(RRHHApp.Models.Enums.PermisoEnum.CrearTardanza, Session))
            {
                this.AddNotification("No posees permisos para listar las tardanzas de usuario.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }
            if (id <= 0)
            {
                this.AddNotification("No selecciono un usuario para realizar este proceso.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }

            var usuarioTardanza = db.UsuarioTardanza.Where(a=>!a.Eliminado && a.UsuarioId == id).Include(u => u._MotivoTardanza).Include(u => u._Usuario);
            ViewBag.Id = id;
            return View(await usuarioTardanza.ToListAsync());
        }
        public ActionResult Index()
        {
            if (!new GenericController().hasAccess(RRHHApp.Models.Enums.PermisoEnum.CrearTardanza, Session))
            {
                this.AddNotification("No posees permisos para listar las tardanzas de usuario.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }
            int id = Convert.ToInt32(encriptar_DesEncriptar.DesEncriptar(Session["UserID"].ToString()));
            var usuarios = db.UsuarioAsociado.Where(a => !a.Eliminado && a.LiderId == id).ToList();

            var usuarioTardanza = db.UsuarioTardanza.Where(a => !a.Eliminado).Include(u => u._MotivoTardanza).Include(u => u._Usuario).ToList();
            usuarioTardanza = usuarioTardanza.Where(a => usuarios.Any(b => b.UsuarioId == a.UsuarioId)).ToList();
            return View(usuarioTardanza);
        }

        // GET: UsuarioTardanza/Create
        public async Task<ActionResult> Create(int usuarioId = 1)
        {
            if (!new GenericController().hasAccess(RRHHApp.Models.Enums.PermisoEnum.CrearTardanza, Session))
            {
                this.AddNotification("No posees permisos para crear tardanza a usuario.", NotificationType.WARNING);
                return RedirectToAction("Index");
            }
            int id = Convert.ToInt32(encriptar_DesEncriptar.DesEncriptar(Session["UserID"].ToString()));
            Usuario usuario = db.Usuario.FindAsync(id).Result;
            List<Usuario> usuariosList = new List<Usuario>();
            if (usuario._Posicion.EsLider)
            {
                var usuarios = db.UsuarioAsociado.Where(a => a.LiderId == usuario.Id).ToListAsync().Result;
                if (usuarios.Count() > 0)
                {
                    foreach (var item in usuarios)
                    {
                        item._Usuario.Nombre = string.Format("{0} {1}", item._Usuario.Nombre, item._Usuario.Apellido);
                        usuariosList.Add(item._Usuario);
                    }
                }
                else 
                {
                    this.AddNotification("No posees usuarios para supervisar", NotificationType.WARNING);
                    return RedirectToAction("Index");
                }
            }
            else if (usuario._GrupoUsuario.Id == 3)
            {
                var usuarios = db.UsuarioAsociado.Where(a => a.LiderId == usuario.Id);
                if (usuarios.Count() > 0)
                {
                    foreach (var item in usuarios)
                    {
                        var result = db.Usuario.Find(item.UsuarioId);
                        result.Nombre = string.Format("{0} {1}", result.Nombre, result.Apellido);
                        usuariosList.Add(result);
                    }
                }
                else
                {
                    this.AddNotification("No posees usuarios para supervisar", NotificationType.WARNING);
                    return RedirectToAction("Index");
                }
            }
            else {
                this.AddNotification("No posees permisos para crear tardanza a usuario.", NotificationType.WARNING);
                return RedirectToAction("Index");
            }

            ViewBag.UsuarioId = new SelectList(usuariosList, "Id", "Nombre", usuarioId);
            ViewBag.MotivoTardanzaId = new SelectList(db.MotivoTardanza, "Id", "Descripcion");

            ViewBag.MaxDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            return View();
        }

        // POST: UsuarioTardanza/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,UsuarioId,MotivoTardanzaId,FechaTardanza,Comentario,FechaCreacion,FechaModificacion,Estado,Eliminado")] UsuarioTardanza usuarioTardanza, int Horas, int Minutos)
        {
            if (ModelState.IsValid)
            {

                usuarioTardanza.Tiempo = usuarioTardanza.FechaTardanza.Date.AddHours(Horas).AddMinutes(Minutos);
                usuarioTardanza.Comentario = !string.IsNullOrEmpty(usuarioTardanza.Comentario) ? usuarioTardanza.Comentario : string.Empty;
                usuarioTardanza.FechaCreacion = DateTime.Now;
                usuarioTardanza.FechaModificacion = DateTime.Now;
                db.UsuarioTardanza.Add(usuarioTardanza);
                await db.SaveChangesAsync();
                this.AddNotification("Tardanza de Usuario registrada exitosamente.", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.MotivoTardanzaId = new SelectList(db.MotivoTardanza, "Id", "Descripcion", usuarioTardanza.MotivoTardanzaId);
            ViewBag.UsuarioId = new SelectList(db.Usuario, "Id", "UsuarioId", usuarioTardanza.UsuarioId);
            return View(usuarioTardanza);
        }

        // GET: UsuarioTardanza/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            var usuarioTardanza = await db.UsuarioTardanza.FindAsync(id);
            usuarioTardanza.FechaModificacion = DateTime.Now;
            usuarioTardanza.Eliminado = true;
            usuarioTardanza.Estado = RRHHApp.Models.Enums.EstadoEnum.Inactivo;
            db.Entry(usuarioTardanza).State = EntityState.Modified;
            await db.SaveChangesAsync();
            this.AddNotification("Tardanza de Usuario eliminada exitosamente.", NotificationType.SUCCESS);
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
