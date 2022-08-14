using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RRHHApp.Models.Usuarios
{
    public class Posicion : MainModel
    {
        public int DepartamentoId { get; set; } = 0;
        [MaxLength(100)]
        public string Descripcion { get; set; } = string.Empty;
        public bool EsLider { get; set; } = false;

        public Departamento _Departamento { get; set; } = null;
        public virtual ICollection<Usuario> __Usuarios { get; set; } = null;
    }
}