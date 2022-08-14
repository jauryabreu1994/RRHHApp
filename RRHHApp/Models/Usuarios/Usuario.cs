
using RRHHApp.Models.Enums;
using RRHHApp.Models.Solicitudes;
using RRHHApp.Models.Tardanza;
using RRHHApp.Models.Ubicaciones;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RRHHApp.Models.Usuarios
{
    public class Usuario : MainModel
    {
        public int GrupoUsuarioId { get; set; }
        public int DepartamentoId { get; set; }
        public int PosicionId { get; set; }
        [MaxLength(20)]
        public string UsuarioId { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;
        [MaxLength(20)]
        public string Identificacion { get; set; } = string.Empty;
        public int PaisId { get; set; } = 0;
        public int CiudadId { get; set; } = 0;
        [MaxLength(300)]
        public string Direccion { get; set; } = string.Empty;
        [MaxLength(100)]
        public string Correo { get; set; } = string.Empty;
        [MaxLength(30)]
        public string Telefono { get; set; } = string.Empty;
        public DateTime FechaIngreso { get; set; } = DateTime.Now;
        public GeneroEnum Genero { get; set; } = GeneroEnum.Masculino;
        public bool RRHHAutorizado { get; set; } = false;
        public bool IsCEO { get; set; } = false;

        public virtual GrupoUsuario _GrupoUsuario { get; set; }
        public virtual Posicion _Posicion { get; set; }
        public virtual Departamento _Departamento { get; set; }
        public virtual Pais _Pais { get; set; }
        public virtual Ciudad _Ciudad { get; set; }

        public virtual ICollection<UsuarioContrasena> __UsuarioContrasenas { get; set; } = null;
        public virtual ICollection<UsuarioAsociado> __UsuarioAsociadosLider { get; set; } = null;
        public virtual ICollection<UsuarioAsociado> __UsuarioAsociadosRecurso { get; set; } = null;
        public virtual ICollection<UsuarioSolicitudes> __UsuarioSolicitudes { get; set; } = null;


        public virtual ICollection<SolicitudEstado> __SolicitudesEstadoGerente { get; set; } = null;
        public virtual ICollection<SolicitudEstado> __SolicitudesEstadoSupervidor { get; set; } = null;
        public virtual ICollection<SolicitudEstado> __SolicitudesEstadoRRHH { get; set; } = null;
        public virtual ICollection<UsuarioTardanza> __UsuarioTardanza { get; set; } = null;
    }
}