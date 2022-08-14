using RRHHApp.Models.Enums;
using RRHHApp.Models.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RRHHApp.Models.Solicitudes
{
    public class SolicitudEstado : MainModel
    {
        public int UsuarioSolicitudId { get; set; }
        public int SupervisorId { get; set; }
        public SolicitudEstadoEnum SupervisorEstado { get; set; }
        public int GerenteId { get; set; }
        public SolicitudEstadoEnum GerenteEstado { get; set; }
        public int RRHHId { get; set; }
        public SolicitudEstadoEnum RRHHEstado { get; set; }

        public virtual Usuario _Supervisor { get; set; }
        public virtual Usuario _Gerente { get; set; }
        public virtual Usuario _RRHH { get; set; }
        public virtual UsuarioSolicitudes _UsuarioSolicitudes { get; set; }

    }
}