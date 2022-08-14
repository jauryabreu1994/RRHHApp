using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Mvc;
using RRHHApp.Context;
using RRHHApp.Extensions;
using System.Linq;
using System;

namespace RRHHApp.Controllers.Usuarios
{
    public class LoginController : Controller
    {
        private MainDbContext db = new MainDbContext();
        private Encriptar_DesEncriptar encriptar_DesEncriptar = new Encriptar_DesEncriptar();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(string usuario, string contrasena)
        {
            try
            {
                if (!string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(contrasena))
                {
                    var _usuario = await db.Usuario.FirstAsync(a => (a.UsuarioId == usuario || a.Correo == usuario) && a.Estado == Models.Enums.EstadoEnum.Activo);

                    if (_usuario != null)
                    {
                        var _contrasena = await db.UsuarioContrasena.FirstAsync(a => a.UsuarioId == _usuario.Id && a.Estado == Models.Enums.EstadoEnum.Activo);
                        if (encriptar_DesEncriptar.DesEncriptar(_contrasena.Contraseña) == contrasena)
                        {
                            
                            
                            Session.Add("UserID", encriptar_DesEncriptar.Encriptar(_usuario.Id.ToString()));
                            Session["UserName"] = (!string.IsNullOrEmpty(_usuario.Apellido)) ? string.Format("{0} {1}", _usuario.Nombre.Split(' ')[0], _usuario.Apellido.Split(' ')[0]) : _usuario.Nombre;
                            Session["GrupoUsuarioId"] = encriptar_DesEncriptar.Encriptar(_usuario.GrupoUsuarioId.ToString());
                            Session["GrupoUsuario"] = db.GrupoUsuario.Find(_usuario.GrupoUsuarioId).Descripcion;
                            Session["EsLider"] = _usuario._Posicion.EsLider.ToString();
                            int[] permisos = new int[db.Permiso.Count()];
                            int i = 0;
                            foreach (var p in _usuario._GrupoUsuario.__GrupoPermisos)
                            {
                                permisos[i] = p.PermisoId;
                                i++;
                            }
                            Session["PermisosId"] = permisos;

                            return UserDashBoard();
                        }
                        else
                            ViewBag.GeneralError = "Contraseña incorrecta";

                    }
                    else
                        ViewBag.GeneralError = "Usuario o Correo incorrecto";
                }
                else
                    this.AddNotification("Usuario y/o Contraseña incorrecto", NotificationType.ERROR);
            }
            catch(Exception ex)
            {
                ViewBag.GeneralError = (ex.Message == "Sequence contains no elements") ? "Usuario o Correo incorrecto" : "Usuario y/o Contraseña incorrecto";
            }

            ViewBag.usuario = usuario;
            ViewBag.contrasena = contrasena;
            return View();
        }

        public ActionResult UserDashBoard()
        {
            if (Session["UserID"] != null)
            {
                return RedirectToAction("Index", "DashBoard");
            }
            else
            {
                ViewBag.GeneralError = "Usuario y/o Contraseña incorrecto";
                return View();
            }
        }

        public ActionResult Salir() 
        {
            Session.Abandon();
            Session.Clear();
            Response.Cookies.Clear();

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
