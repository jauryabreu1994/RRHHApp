using RRHHApp.Models.Licencias;
using RRHHApp.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using RRHHApp.Models.Enums;

namespace RRHHApp.Models.Solicitudes
{
    public class UsuarioSolicitudes : MainModel
    {
        public int UsuarioId { get; set; }
        public int LicenciaId { get; set; }

        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }

        public string Comentario { get; set; }
        [NotMapped]
        public SolicitudEstadoEnum SolicitudEstado { get; set; }
        public virtual Usuario _Usuario { get; set; }
        public virtual Licencia _Licencia { get; set; }
        public virtual ICollection<SolicitudEstado> __SolicitudesEstado { get; set; } = null;
    }
}