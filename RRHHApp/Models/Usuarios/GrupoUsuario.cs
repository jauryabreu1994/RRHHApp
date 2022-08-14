using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RRHHApp.Models.Usuarios
{
    public class GrupoUsuario : MainModel
    {
        [MaxLength(100)]
        public string Descripcion { get; set; } = string.Empty;
        public virtual ICollection<GrupoPermiso> __GrupoPermisos { get; set; } = null;

        public virtual ICollection<Usuario> __Usuarios { get; set; } = null;
    }
}