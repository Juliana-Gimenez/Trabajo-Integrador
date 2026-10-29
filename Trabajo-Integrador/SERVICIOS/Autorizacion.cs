using Trabajo_Integrador.Modelos;

namespace Trabajo_Integrador.Servicios
{
    public class Autorizacion
    {
        public bool PuedeEliminarPeliculas(Usuario usuario)
        {
            return usuario.Rol == "Administrador";
        }

        
    }
}
