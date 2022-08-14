namespace RRHHApp.Models.Usuarios
{
    public class UsuarioAsociado : MainModel
    {
        public int LiderId { get; set; } = 0;
        public int UsuarioId { get; set; } = 0;

        public virtual Usuario _Lider { get; set; }
        public virtual Usuario _Usuario { get; set; }
    }
}