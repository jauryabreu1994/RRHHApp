using RRHHApp.Models.Enums;
using System;

namespace RRHHApp.Models.Solicitudes
{
    public class InformacionSolicitud
    {
        public int Id { get; set; }
        public string Licencia { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioNombre { get; set; }

        public int FirmaId { get; set; }
        public string FirmaNombre { get; set; }

        public string Comentario { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int CantidadDias { get; set; }
        public DateTime FechaCreacion { get; set; }

        public SolicitudEstadoEnum Estado { get; set; }
    }
}