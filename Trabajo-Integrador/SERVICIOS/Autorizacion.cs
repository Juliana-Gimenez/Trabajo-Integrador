using Trabajo_Integrador.Modelos;

namespace Trabajo_Integrador.Servicios
{
    public class Autorizacion
    {
        public bool EsAdministrador(Usuario? usuario)
        {
            return usuario != null && string.Equals(usuario.Rol, "Administrador", StringComparison.OrdinalIgnoreCase);
        }

        
    }
}
