
namespace RRHHApp.Models.Usuarios
{
    public class UsuarioContrasena : MainModel
    {
        public int UsuarioId { get; set; } = 0;
        public string Contraseña { get; set; } = string.Empty;

        public virtual Usuario _Usuario { get; set; }
    }
}