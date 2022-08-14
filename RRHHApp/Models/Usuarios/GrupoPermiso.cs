
using System.Collections.Generic;
using System.Web.Mvc;

namespace RRHHApp.Models.Usuarios
{
    public class GrupoPermiso : MainModel
    {
        public int GrupoUsuarioId { get; set; } = 0;
        public int PermisoId { get; set; } = 0;

        public virtual GrupoUsuario _GrupoUsuario { get; set; }
        public virtual Permiso _Permiso { get; set; }
    }

    public class GrupoPermisoModel
    {
        public int? Id { get; set; } = 0;
        public string Descripcion { get; set; } = "";
        public IEnumerable<SelectListItem> grupoPermisos { get; set; }
        public IEnumerable<string> grupoPermisosSeleccionados { get; set; }
    }
}