using RRHHApp.Models.Usuarios;
using System;

namespace RRHHApp.Models.Tardanza
{
    public class UsuarioTardanza : MainModel
    {
        public int UsuarioId { get; set; }
        public int MotivoTardanzaId { get; set; }

        public DateTime Tiempo { get; set; }
        public DateTime FechaTardanza { get; set; }
        public string Comentario { get; set; }

        public virtual MotivoTardanza _MotivoTardanza { get; set; }
        public virtual Usuario _Usuario { get; set; }
    }
}