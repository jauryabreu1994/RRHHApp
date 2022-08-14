using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RRHHApp.Models.Usuarios
{
    public class Departamento : MainModel
    {
        [MaxLength(100)]
        public string Descripcion { get; set; } = string.Empty;

        public virtual ICollection<Posicion> __Posiciones { get; set; } = null;
    }
}