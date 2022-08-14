
using RRHHApp.Models.Enums;
using RRHHApp.Models.Solicitudes;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace RRHHApp.Models.Licencias
{
    public class Licencia : MainModel
    {
        public string Descripcion { get; set; } = string.Empty;
        public string Icono { get; set; }
        public bool Es_Principal { get; set; }
        public GeneroEnum Genero { get; set; }
        [NotMapped]
        public int Dias_Disponibles { get; set; }
        public virtual ICollection<LicenciaConfiguracion> __LicenciaConfiguraciones { get; set; } = null;
        public virtual ICollection<UsuarioSolicitudes> __UsuarioSolicitudes { get; set; } = null;
    }
}