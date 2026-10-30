using Trabajo_Integrador.Datos;
using Trabajo_Integrador.Modelos;
using Trabajo_Integrador.Servicios;
using Trabajo_Integrador.Metodos;

namespace Trabajo_Integrador
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "MasitaTV";

            Console.WriteLine("==============================");
            Console.WriteLine("       ¡BIENVENIDO A          ");
            Console.WriteLine("          MASITA TV!          ");
            Console.WriteLine("==============================");

            try
            {
                //Inicializamos la base de datos.
                ConexionDB conexionDB = new ConexionDB();
                conexionDB.InicializarBaseDeDatos();

                //Creamos los objetos que necesitamos.
                UsuarioDAO usuarioDAO = new UsuarioDAO();
                PeliculaDAO peliculaDAO = new PeliculaDAO();
                Autenticacion autenticacion = new Autenticacion(usuarioDAO);
                Autorizacion autorizacion = new Autorizacion();

                bool aplicacionActiva = true;

                while (aplicacionActiva)
                {
                    Menu.MostrarMenuInicial();

                    int opcion = EntradaConsola.LeerOpcion();

                    switch (opcion)
                    {
                        case 1:
                            Menu.RegistrarUsuario(autenticacion);
                            break;

                        case 2:
                            Usuario? usuario = Menu.SolicitarInicioSesion(autenticacion); 

                            if (usuario != null)
                            {
                                Console.WriteLine($"\n¡Bienvenido {usuario.NombreUsuario}!");

                                Menu.EjecutarMenuUsuario(usuario, autorizacion, peliculaDAO);   
                            }
                            break;

                        case 0:
                            aplicacionActiva = false;
                            Console.WriteLine("\n¡Gracias por usar MasitaTV!");
                            break;

                        default:
                            Console.WriteLine("\nLa opción ingresada no existe.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("\nOcurrió un error al iniciar MasitaTV.");
                Console.WriteLine($"Detalle: {ex.Message}");
                Console.WriteLine("\nPresioná una tecla para salir...");
                Console.ReadKey();
            }
        }

     

     

















            


        



        static void MostrarCatalogo(PeliculaDAO peliculaDAO)
        {
            Console.WriteLine("\n=== CATÁLOGO DE MASITATV ===");

            List<Pelicula> peliculas = peliculaDAO.ObtenerTodas();

            if (peliculas.Count == 0)
            {
                Console.WriteLine("Todavía no hay películas cargadas.");
                return;
            }

            foreach (Pelicula pelicula in peliculas)
            {
                Console.WriteLine("\n------------------------------");
                Console.WriteLine($"ID: {pelicula.Id}");
                Console.WriteLine($"Título: {pelicula.Titulo}");
                Console.WriteLine($"Género: {pelicula.Genero}");
                Console.WriteLine($"Año: {pelicula.Anio}");
                Console.WriteLine($"Duración: {pelicula.Duracion} minutos");
                Console.WriteLine($"Sinopsis: {pelicula.Sinopsis}");
            }

            Console.WriteLine("\n------------------------------");
        }


    }
}

