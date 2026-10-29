using System;
using Trabajo_Integrador.Datos;
using Trabajo_Integrador.Modelos;
using Trabajo_Integrador.Servicios;
using System.Collections.Generic;

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
                Autenticacion autenticacion = new Autenticacion(usuarioDAO);
                Autorizacion autorizacion = new Autorizacion();
                PeliculaDAO peliculaDAO = new PeliculaDAO();

                //IMPORTANTE!!!: Las siguientes líneas de código se han ejecutado durante el armado del programa para poder establecer un usuario administrador antes de permitir el registro de nuevos usuarios. Esto con la finalidad de aumentar la seguridad del sitio. Una buena práctica sería ejecutar solo una vez este código y luego eliminarlo, para que no se vulnere la seguridad del sitio; pero como el tp es evaluado, lo dejamos para que el profe entienda como nos encargamos de resolver esta situacion usuario-administrador <3

                //Console.Write("\nIngresá tu nombre de usuario para asignar el rol de administrador: ");
                //string nombreUsuarioAdmin = Console.ReadLine() ?? "";

                //bool actualizado = usuarioDAO.CambiarRol(
                //nombreUsuarioAdmin,
                //"Administrador"
                //);

                //if (actualizado)
                //{
                //    Console.WriteLine("¡Rol de administrador asignado correctamente!");
                //}
                //else
                //{
                //    Console.WriteLine("No se encontró ese usuario.");
                //}

                // 3. Mostramos el menú inicial.
                bool aplicacionActiva = true;

                while (aplicacionActiva)
                {
                    MostrarMenuInicial();

                    int opcion = LeerOpcion();

                    switch (opcion)
                    {
                        case 1:
                            RegistrarUsuario(autenticacion);
                            break;

                        case 2:
                            Usuario? usuario = SolicitarInicioSesion(autenticacion); 

                            if (usuario != null)
                            {
                                Console.WriteLine($"\n¡Bienvenido {usuario.NombreUsuario}!");

                                EjecutarMenuUsuario(usuario, autorizacion, peliculaDAO);   
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

        // MENÚ INICIAL
        static void MostrarMenuInicial()
        {
            Console.WriteLine("\n=== MENÚ INICIAL ===");
            Console.WriteLine("1. Crear una cuenta");
            Console.WriteLine("2. Iniciar sesión");
            Console.WriteLine("0. Salir");
        }

        // REGISTRO
        static void RegistrarUsuario(
            Autenticacion autenticacion)
        {
            Console.WriteLine("\n=== CREAR UNA CUENTA ===");

            Console.Write("Nombre: ");
            string nombre = Console.ReadLine() ?? "";

            Console.Write("Apellido: ");
            string apellido = Console.ReadLine() ?? "";

            Console.Write("Nombre de usuario: ");
            string nombreUsuario = Console.ReadLine() ?? "";

            Console.Write("Correo electrónico: ");
            string email = Console.ReadLine() ?? "";

            Console.Write("Contraseña: ");
            string contraseña = Console.ReadLine() ?? "";

            string resultado = autenticacion.Registrar(
                nombre,
                apellido,
                nombreUsuario,
                email,
                contraseña
            );

            Console.WriteLine($"\n{resultado}");
        }

        // INICIO DE SESIÓN
        static Usuario? SolicitarInicioSesion(Autenticacion autenticacion)
        {
            Console.WriteLine("\n=== INICIAR SESIÓN ===");

            Console.Write("Nombre de usuario: ");
            string nombreUsuario = Console.ReadLine() ?? "";

            Console.Write("Contraseña: ");
            string contraseña = Console.ReadLine() ?? "";

            Usuario? usuario = autenticacion.IniciarSesion(nombreUsuario, contraseña);

            if (usuario == null)
            {
                Console.WriteLine( "\nUsuario o contraseña incorrectos.");   
            }

            return usuario;
        }

        // MENÚ DEL USUARIO
        static void EjecutarMenuUsuario(Usuario usuario, Autorizacion autorizacion, PeliculaDAO peliculaDAO) 
        {
            bool sesionActiva = true;

            while (sesionActiva)
            {
                MostrarMenu(usuario);

                int opcion = LeerOpcion();

                switch (opcion)
                {
                    case 0:
                        sesionActiva = false;
                        Console.WriteLine("\nSesión cerrada.");
                        break;

                    case 1:
                        MostrarCatalogo(peliculaDAO);
                        break;

                    case 2:
                        Console.WriteLine($"Usuario: {usuario.NombreUsuario}");  
                        Console.WriteLine($"Rol: {usuario.Rol}");
                        break;

                    case 3:
                        if (usuario.Rol != "Administrador")
                        {
                            Console.WriteLine("\nNo tenés permisos de administrador.");    
                            break;
                        }
                        AgregarPelicula(peliculaDAO);
                        break;

                    case 4:
                        if (usuario.Rol != "Administrador")
                        {
                            Console.WriteLine("\nNo tenés permisos de administrador.");
                            break;
                        }

                        ModificarPelicula(peliculaDAO);
                        break;

                    case 5:
                        if(usuario.Rol != "Administrador")
                        {
                            Console.WriteLine("\nNo tenés permisos de administrador.");
                            break;
                        }
                        EliminarPelicula(peliculaDAO);
                        break;

                    default:
                        Console.WriteLine("\nLa opción ingresada no existe.");
                        break;
                }
            }
        }

        // OPCIONES DISPONIBLES SEGÚN EL ROL
        static void MostrarMenu(Usuario usuario)
        {
            Console.WriteLine("\n=== MENÚ MASITATV ===");
            Console.WriteLine("1. Ver catálogo");
            Console.WriteLine("2. Ver mi cuenta");

            if (usuario.Rol == "Administrador")
            {
                Console.WriteLine("3. Agregar película");
                Console.WriteLine("4. Modificar película");
                Console.WriteLine("5. Eliminar película");
            }

            Console.WriteLine("0. Cerrar sesión");
        }

        // LECTURA Y VALIDACIÓN DE OPCIONES
        static int LeerOpcion()
        {
            while (true)
            {
                Console.Write("\nElegí una opción: ");
                string entrada = Console.ReadLine() ?? "";

                if (int.TryParse(entrada, out int opcion))
                {
                    return opcion;
                }

                Console.WriteLine("Entrada inválida. Ingresá un número.");
                    
            }
        }


        static void AgregarPelicula(PeliculaDAO peliculaDAO)
        {
            Console.WriteLine("\n=== AGREGAR PELÍCULA ===");

            string titulo;
            while (true)
            {
                Console.Write("Título: ");
                titulo = (Console.ReadLine() ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(titulo))
                {
                    break;
                }

                Console.WriteLine("El título no puede estar vacío. Intenta de nuevo.");

            }

            string genero;
            while (true)
            {
                Console.Write("Género: ");
                genero = (Console.ReadLine() ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(genero))
                {
                    break;
                }

                Console.WriteLine("El género no puede estar vacío. Intenta de nuevo.");

            }

            int anio;
            while (true)
            {
                Console.Write("Año de estreno: ");
                string entrada = Console.ReadLine() ?? "";

                if (int.TryParse(entrada, out anio) && anio >= 1880 && anio <= DateTime.Now.Year)
                {
                    break;
                }
                    Console.WriteLine($"Ingresa un número que corresponda a un año entre 1880 y {DateTime.Now.Year}");

            }

            int duracion;
            while (true)
            {
                Console.Write("Duración en minutos: ");
                string entrada = Console.ReadLine() ?? "";

                if (int.TryParse(entrada, out duracion) && duracion > 0)
                {
                    break;
                }

                Console.WriteLine("La duración debe ser un número entero mayor que cero.");
            }

            string sinopsis;
            while (true)
            {
                Console.Write("Sinopsis: ");
                sinopsis = (Console.ReadLine() ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(sinopsis))
                {
                    break;
                }

                Console.WriteLine("La sinopsis no puede estar vacío. Intenta de nuevo.");

            }

            if(peliculaDAO.ExistePelicula(titulo, anio))
            {
                Console.WriteLine("\nYa existe una película con ese título y año de estreno.");
                return;
            }

            Pelicula pelicula = new Pelicula
            {
                Titulo = titulo,
                Genero = genero,
                Anio = anio,
                Duracion = duracion,
                Sinopsis = sinopsis
            };

            peliculaDAO.AgregarPelicula(pelicula);

            Console.WriteLine("\n¡Película agregada correctamente!");
        }


        static void ModificarPelicula(PeliculaDAO peliculaDAO)
        {
            Console.WriteLine("\n=== MODIFICAR PELÍCULA ===");

            // 1. Mostramos las películas disponibles.
            List<Pelicula> peliculas = peliculaDAO.ObtenerTodas();

            if (peliculas.Count == 0)
            {
                Console.WriteLine("No hay películas para modificar.");
                return;
            }

            foreach (Pelicula pelicula in peliculas)
            {
                Console.WriteLine($"ID: {pelicula.Id} | {pelicula.Titulo} ({pelicula.Anio})");
            }

            // 2. Solicitamos el ID.
            int id;

            while (true)
            {
                Console.Write("\nIngresá el ID de la película que querés modificar: ");

                if (int.TryParse(Console.ReadLine(), out id) && id > 0)
                {
                    break;
                }

                Console.WriteLine("Ingresá un ID válido.");
            }

            // 3. Buscamos la película seleccionada.
            Pelicula? peliculaSeleccionada = peliculas.Find(peli => peli.Id == id); //Find() busca dentro de la lista la primera película que cumple una condición.
            // peli => peli.Id == id es una expresión LAMBDA que pide buscar una película cuyo ID sea igual al ID que ingresó el usuario.

            if (peliculaSeleccionada == null)
            {
                Console.WriteLine("No existe una película con ese ID.");
                return;
            }

            Console.WriteLine("\nIngresá los nuevos datos.");

            // 4. Solicitamos el nuevo título.
            string titulo;

            while (true)
            {
                Console.Write("Título: ");
                titulo = (Console.ReadLine() ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(titulo))
                    break;

                Console.WriteLine("El título no puede estar vacío.");
            }

            // 5. Solicitamos el nuevo género.
            string genero;

            while (true)
            {
                Console.Write("Género: ");
                genero = (Console.ReadLine() ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(genero))
                    break;

                Console.WriteLine("El género no puede estar vacío.");
            }

            // 6. Solicitamos el nuevo año.
            int anio;

            while (true)
            {
                Console.Write("Año de estreno: ");

                if (int.TryParse(Console.ReadLine(), out anio) && anio >= 1888 && anio <= DateTime.Now.Year)
                {
                    break;
                }

                Console.WriteLine($"Ingresá un año entre 1888 y {DateTime.Now.Year}.");
            }

            // 7. Solicitamos la nueva duración.
            int duracion;

            while (true)
            {
                Console.Write("Duración en minutos: ");

                if (int.TryParse(Console.ReadLine(), out duracion) && duracion > 0)
                {
                    break;
                }

                Console.WriteLine("La duración debe ser un número entero mayor que cero.");
            }

            // 8. Solicitamos la nueva sinopsis.
            string sinopsis;

            while (true)
            {
                Console.Write("Sinopsis: ");
                sinopsis = (Console.ReadLine() ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(sinopsis))
                    break;

                Console.WriteLine("La sinopsis no puede estar vacía.");
            }

            // 9. Comprobamos que no exista un duplicado.
            if (peliculaDAO.ExistePelicula(titulo, anio, id))
            {
                Console.WriteLine("\nYa existe otra película con ese título y año.");
                return;
            }

            // 10. Actualizamos los datos de la película.
            Pelicula peliculaModificada = new Pelicula
            {
                Id = id,
                Titulo = titulo,
                Genero = genero,
                Anio = anio,
                Duracion = duracion,
                Sinopsis = sinopsis
            };

            bool modificada = peliculaDAO.ModificarPelicula(peliculaModificada);

            if (modificada)
            {
                Console.WriteLine("\n¡Película modificada correctamente!");
            }
            else
            {
                Console.WriteLine("\nNo se pudo modificar la película.");
            }
        }


        static void EliminarPelicula(PeliculaDAO peliculaDAO)
        {
            Console.WriteLine("\n=== ELIMINAR PELÍCULA ===");

            //Obtenemos las películas registradas.
            List<Pelicula> peliculas = peliculaDAO.ObtenerTodas();

            if (peliculas.Count == 0)
            {
                Console.WriteLine("No hay películas para eliminar.");
                return;
            }

            //Mostramos el catálogo.
            foreach (Pelicula pelicula in peliculas)
            {
                Console.WriteLine($"ID: {pelicula.Id} | {pelicula.Titulo} ({pelicula.Anio})");
            }

            //Solicitamos el ID.
            int id;

            while (true)
            {
                Console.Write("\nIngresá el ID de la película que querés eliminar: ");

                if (int.TryParse(Console.ReadLine(), out id) && id > 0)
                {
                    break;
                }

                Console.WriteLine("Ingresá un ID válido.");
            }

            //Comprobamos que la película exista.
            Pelicula? peliculaSeleccionada = peliculas.Find(peli => peli.Id == id);

            if (peliculaSeleccionada == null)
            {
                Console.WriteLine("No existe una película con ese ID.");
                return;
            }

            //Pedimos confirmación antes de eliminar.
            Console.WriteLine($"\nVas a eliminar: {peliculaSeleccionada.Titulo} " + $"({peliculaSeleccionada.Anio})");

            Console.Write("¿Estás seguro? (S/N): ");
            string confirmacion = (Console.ReadLine() ?? "").Trim();

            if (!confirmacion.Equals("S", StringComparison.OrdinalIgnoreCase)) 
            //Equals compara el contenido de la variable donde se almacenó lo ingresado por el usuario con la condición dada, en este caso "S".
            //StringComparison es una enumeración de C# que permite elegir las reglas para comparar cadenas de texto.
            //OrdinalIgnoreCase indica que no se debe distinguir entre mayúsculas y minúsculas.
            {
                Console.WriteLine("\nEliminación cancelada.");
                return;
            }

            //Eliminamos el registro de SQLite.
            bool eliminada = peliculaDAO.EliminarPelicula(id);

            if (eliminada)
            {
                Console.WriteLine("\n¡Película eliminada correctamente!");
            }
            else
            {
                Console.WriteLine("\nNo se pudo eliminar la película. Puede que ya no exista.");
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

