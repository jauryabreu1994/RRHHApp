using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RRHHApp.Context;
using RRHHApp.Extensions;
using RRHHApp.Models.Ubicaciones;
using RRHHApp.Models.Usuarios;

namespace RRHHApp.Controllers.Usuarios
{
    public class UsuarioController : Controller
    {
        private MainDbContext db = new MainDbContext();
        // GET: Usuario
        public ActionResult Index()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para el Listado de Usuarios.", NotificationType.WARNING);
                return RedirectToAction("Index", "DashBoard");
            }
            var usuario = db.Usuario.Include(u => u._Posicion).Include(u => u._Ciudad).Include(u => u._GrupoUsuario).Include(u => u._Pais);
            ViewBag.ListGroup = db.GrupoUsuario.ToList();
            return View(usuario.ToList());
        }

        public ActionResult Details()
        {
            var usuarios = db.Usuario.Include(u => u._Posicion).Include(u => u._Ciudad).Include(u => u._GrupoUsuario).Include(u => u._Pais).Where(a=>!a.Eliminado).ToList();
            var asociados = db.UsuarioAsociado.Include(a=>a._Lider).ToList();
            foreach (var usuario in usuarios)
            {
                usuario.Nombre = string.Format("{0} {1}", usuario.Nombre, usuario.Apellido);
                var list = asociados.Where(a => a.UsuarioId == usuario.Id);
                if (list.Count() > 0)
                {
                    var lider = list.First()._Lider;
                    usuario.Apellido = lider.Nombre;
                }
                else
                    usuario.Apellido = string.Empty;
            }
            return View(usuarios.ToList());
        }

        public ActionResult OrgChart()
        {
            var usuarios = db.Usuario.Include(u => u._Posicion).Include(u => u._Ciudad).Include(u => u._GrupoUsuario).Include(u => u._Pais).Where(a => !a.Eliminado).ToList();
            var asociados = db.UsuarioAsociado.Include(a => a._Lider).ToList();

            string array = string.Empty;
            foreach (var usuario in usuarios)
            {
                var list = asociados.Where(a => a.UsuarioId == usuario.Id);
                if (list.Count() > 0)
                {
                    var lider = list.First()._Lider;
                    array += "{" + string.Format("*key*: *{0}*, *name*:*{1} {2}*, *title*:*{3} - {4}*, *pic*:**, *parent*:*{5}*",
                    usuario.Id, usuario.Nombre, usuario.Apellido, usuario._Departamento.Descripcion, usuario._Posicion.Descripcion, lider.Id) + "},";
                }
                else
                    array += "{" + string.Format("*key*: *{0}*, *name*:*{1} {2}*, *title*:*{3} - {4}*, *pic*:**",
                    usuario.Id.ToString(), usuario.Nombre, usuario.Apellido, usuario._Departamento.Descripcion, usuario._Posicion.Descripcion) + "},";

            }

            ViewBag.Usuarios = array.Remove(array.Length - 1);;

            return View(usuarios.ToList());
        }



        public JsonResult Ciudad_Bind(int paisId)
        {

            if (string.IsNullOrEmpty(paisId.ToString()))
                return Json(null);

            List<Ciudad> ciudades = db.Ciudad.Where(c => c.PaisId == paisId).ToList();
            List<SelectListItem> listado = new List<SelectListItem>();

            foreach (var ciudad in ciudades)
                listado.Add(new SelectListItem { Text = ciudad.Descripcion, Value = ciudad.Id.ToString() });

            return Json(listado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Posicion_Bind(int DepartamentoId)
        {

            if (string.IsNullOrEmpty(DepartamentoId.ToString()))
                return Json(null);

            List<Posicion> posiciones = db.Posicion.Where(c => c.DepartamentoId == DepartamentoId).ToList();
            List<SelectListItem> listado = new List<SelectListItem>();

            foreach (var p in posiciones)
                listado.Add(new SelectListItem { Text = p.Descripcion, Value = p.Id.ToString() });

            return Json(listado, JsonRequestBehavior.AllowGet);
        }

        // GET: Usuario/Create
        public ActionResult Create()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Crear Usuarios.", NotificationType.WARNING);
                return RedirectToAction("Index", "Usuario");
            }

            ViewBag.DepartamentoId = new SelectList(db.Departamento, "Id", "Descripcion");
            ViewBag.CiudadId = new SelectList(db.Ciudad, "Id", "Descripcion");
            ViewBag.GrupoUsuarioId = new SelectList(db.GrupoUsuario, "Id", "Descripcion");
            ViewBag.PaisId = new SelectList(db.Pais, "Id", "Descripcion");
            ViewBag.MinDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            return View();
        }

        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,GrupoUsuarioId,DepartamentoId,PosicionId,UsuarioId,Nombre,Apellido,Identificacion,FechaIngreso,PaisId,CiudadId,RRHHAutorizado,Direccion,Correo,Telefono,Genero,IsCEO,FechaCreacion,FechaModificacion,Estado,Eliminado")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {                
                usuario.FechaModificacion = DateTime.Now;
                usuario.FechaCreacion = DateTime.Now;
                db.Usuario.Add(usuario);
                db.SaveChanges();

                #region Password
                UsuarioContrasena contrasena = new UsuarioContrasena()
                {
                    Id = 0,
                    UsuarioId = usuario.Id,
                    Estado = Models.Enums.EstadoEnum.Activo,
                    Eliminado = false,
                    FechaModificacion = DateTime.Now,
                    FechaCreacion = DateTime.Now,
                    Contraseña = new Encriptar_DesEncriptar().Encriptar("123456")
                };

                db.UsuarioContrasena.Add(contrasena);
                db.SaveChanges();
                #endregion


                this.AddNotification("Usuario registrado exitosamente", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }

            ViewBag.DepartamentoId = new SelectList(db.Departamento, "Id", "Descripcion");
            ViewBag.CiudadId = new SelectList(db.Ciudad, "Id", "Descripcion", usuario.CiudadId);
            ViewBag.GrupoUsuarioId = new SelectList(db.GrupoUsuario, "Id", "Descripcion", usuario.GrupoUsuarioId);
            ViewBag.PaisId = new SelectList(db.Pais, "Id", "Descripcion", usuario.PaisId);
            this.AddNotification("Favor completar todos los campos", NotificationType.ERROR);
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Editar Usuarios.", NotificationType.WARNING);
                return RedirectToAction("Index", "Usuario");
            }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");;
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartamentoId = new SelectList(db.Departamento, "Id", "Descripcion", usuario.DepartamentoId);
            ViewBag.CiudadId = new SelectList(db.Ciudad, "Id", "Descripcion", usuario.CiudadId);
            ViewBag.GrupoUsuarioId = new SelectList(db.GrupoUsuario, "Id", "Descripcion", usuario.GrupoUsuarioId);
            ViewBag.PaisId = new SelectList(db.Pais, "Id", "Descripcion", usuario.PaisId);
            ViewBag.MinDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,GrupoUsuarioId,DepartamentoId,PosicionId,UsuarioId,Nombre,Apellido,Identificacion,FechaIngreso,RRHHAutorizado,PaisId,CiudadId,Direccion,Correo,Telefono,Genero,IsCEO,FechaCreacion,FechaModificacion,Estado,Eliminado")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.FechaModificacion = DateTime.Now;
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                this.AddNotification("Usuario modificado exitosamente", NotificationType.SUCCESS);
                return RedirectToAction("Index");
            }
            ViewBag.DepartamentoId = new SelectList(db.Departamento, "Id", "Descripcion");
            ViewBag.CiudadId = new SelectList(db.Ciudad, "Id", "Descripcion", usuario.CiudadId);
            ViewBag.GrupoUsuarioId = new SelectList(db.GrupoUsuario, "Id", "Descripcion", usuario.GrupoUsuarioId);
            ViewBag.PaisId = new SelectList(db.Pais, "Id", "Descripcion", usuario.PaisId);
            this.AddNotification("Favor completar todos los campos", NotificationType.ERROR);
            return View(usuario);
        }

        public ActionResult Profile()
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Perfil, Session))
            {
                this.AddNotification("No posees permisos para ver perfil de usuario.", NotificationType.WARNING);
                return RedirectToAction("Index", "Usuario");
            }

            int id = Convert.ToInt32(new Encriptar_DesEncriptar().DesEncriptar(Session["UserID"].ToString()));
            Usuario usuario = db.Usuario.Find(id);

            if (usuario == null)
            {
                return HttpNotFound();
            }

            ViewBag.PaisId = new SelectList(db.Pais, "Id", "Descripcion", usuario.PaisId);

            return View(usuario);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(int PaisId, int CiudadId, string Direccion, string Telefono)
        {
            int id = Convert.ToInt32(new Encriptar_DesEncriptar().DesEncriptar(Session["UserID"].ToString()));
            Usuario usuario = db.Usuario.Find(id);
            try
            {

                usuario.FechaModificacion = DateTime.Now;
                usuario.PaisId = PaisId;
                usuario.CiudadId = CiudadId;
                usuario.Direccion = Direccion;
                usuario.Telefono = Telefono;

                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();
                this.AddNotification("Perfil actualizado exitosamente", NotificationType.SUCCESS);
                return RedirectToAction("Profile");

            }
            catch
            {
                ViewBag.PaisId = new SelectList(db.Pais, "Id", "Descripcion", usuario.PaisId);
                this.AddNotification("Favor completar todos los campos", NotificationType.ERROR);
                return View(usuario);
            }
        }

        // GET: Usuario/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!new GenericController().hasAccess(Models.Enums.PermisoEnum.Usuario, Session))
            {
                this.AddNotification("No posees permisos para Eliminar Usuarios.", NotificationType.WARNING);
                return RedirectToAction("Index", "Usuario");
            }
            if (id == null)
            {
                return RedirectToAction("Error", "Home");;
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Usuario usuario = db.Usuario.Find(id);
            usuario.FechaModificacion = DateTime.Now;
            usuario.Eliminado = true;
            usuario.Estado = Models.Enums.EstadoEnum.Inactivo;
            db.Entry(usuario).State = EntityState.Modified;
            db.SaveChanges();
            this.AddNotification("Usuario eliminado exitosamente", NotificationType.SUCCESS);
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
