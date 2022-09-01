using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using RRHHApp.Context;
using RRHHApp.Models.Usuarios;
using RRHHApp.Extensions;
using System.Linq;

namespace RRHHApp.Controllers.Usuarios
{
    public class UsuarioContrasenaController : Controller
    {
        private MainDbContext db = new MainDbContext();

        public async Task<ActionResult> Edit()
        {
            try
            {
                int userId = Convert.ToInt32(new Encriptar_DesEncriptar().DesEncriptar(Session["UserId"].ToString()));
                UsuarioContrasena usuarioContrasena = await db.UsuarioContrasena.FirstAsync(a => a.UsuarioId == userId && a.Eliminado == false);
                if (usuarioContrasena == null)
                {
                    return HttpNotFound();
                }
                ViewBag.UsuarioId = new SelectList(db.Usuario.Where(a => !a.Eliminado).ToList(), "Id", "UsuarioId", usuarioContrasena.UsuarioId);
                return View(usuarioContrasena);
            }
            catch
            {
                return RedirectToAction("Index", "Login");
            }
        }

        public async Task<ActionResult> Update(int id)
        {
            var contrasena = db.UsuarioContrasena.Where(a => a.UsuarioId == id).FirstOrDefault();
            contrasena.Contraseña = new Encriptar_DesEncriptar().Encriptar("123456");

            db.Entry(contrasena).State = EntityState.Modified;
            await db.SaveChangesAsync();

            this.AddNotification("Contraseña reseteada", NotificationType.SUCCESS);
            return RedirectToAction("Index", "Usuario");
        }

        // POST: UsuarioContrasena/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,UsuarioId,Contraseña,FechaCreacion,FechaModificacion,Estado,Eliminado")] UsuarioContrasena usuarioContrasena,
                                             string ContrasenaActual, string RepetirContrasena)
        {
            if (ModelState.IsValid)
            {
                var contrasena = await db.UsuarioContrasena.FirstAsync(a => a.UsuarioId == usuarioContrasena.UsuarioId && a.Eliminado == false);
                string _contrasenaActual = new Encriptar_DesEncriptar().DesEncriptar(contrasena.Contraseña);
                if (ContrasenaActual != _contrasenaActual)
                    this.AddNotification("Contraseña actual NO coincide con la registrada.", NotificationType.ERROR);
                
                else if (usuarioContrasena.Contraseña != RepetirContrasena)
                    this.AddNotification("Las Contraseñas NO coinciden.", NotificationType.ERROR);
                
                else if (usuarioContrasena.Contraseña.Length < 6)
                    this.AddNotification("La longitud de la Contraseña debe ser mayor a 6 caracteres.", NotificationType.ERROR);
                
                else if (usuarioContrasena.Contraseña == ContrasenaActual)
                    this.AddNotification("Nueva Contraseña debe ser diferente a Contraseña Actual", NotificationType.ERROR);
                
                else
                {
                    contrasena.FechaModificacion = DateTime.Now;
                    contrasena.Contraseña = new Encriptar_DesEncriptar().Encriptar(usuarioContrasena.Contraseña);

                    db.Entry(contrasena).State = EntityState.Modified;
                    await db.SaveChangesAsync();

                    this.AddNotification("Contraseña actualizada", NotificationType.SUCCESS);
                    return RedirectToAction("Profile", "Usuario");
                }

                ViewBag.Contraseña = usuarioContrasena.Contraseña;
                ViewBag.RepetirContrasena = RepetirContrasena;
                ViewBag.ContrasenaActual = ContrasenaActual;
            }

            ViewBag.UsuarioId = new SelectList(db.Usuario, "Id", "UsuarioId", usuarioContrasena.UsuarioId);
            return View(usuarioContrasena);
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
