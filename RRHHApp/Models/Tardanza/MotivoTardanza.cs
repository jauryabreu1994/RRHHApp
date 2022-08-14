using System.Collections.Generic;

namespace RRHHApp.Models.Tardanza
{
    public class MotivoTardanza : MainModel
    {
        public string Descripcion { get; set; } = string.Empty;
        public virtual ICollection<UsuarioTardanza> __UsuarioTardanza { get; set; } = null;
    }
}