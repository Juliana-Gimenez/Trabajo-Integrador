using System;
using System.Collections.Generic;
using Trabajo_Integrador.Datos;
using Trabajo_Integrador.Servicios;
using Trabajo_Integrador.Modelos;

namespace Trabajo_Integrador.Metodos
{
    public static class Menu
    {
        public static void MostrarMenuInicial()
        {
            Console.WriteLine("\n=== MENÚ INICIAL ===");
            Console.WriteLine("1. Crear una cuenta");
            Console.WriteLine("2. Iniciar sesión");
            Console.WriteLine("0. Salir");
        }

        public static void RegistrarUsuario(Autenticacion autenticacion)
        {
            Console.WriteLine("\n=== CREAR UNA CUENTA ===");

            string nombre = EntradaConsola.LeerTextoObligatorio("Nombre: ");
            string apellido = EntradaConsola.LeerTextoObligatorio("Apellido: ");
            string nombreUsuario = EntradaConsola.LeerTextoObligatorio("Nombre de usuario: ");
            string email = EntradaConsola.LeerTextoObligatorio("Correo electrónico: ");

            Console.Write("Contraseña: ");
            string contrasenia = Console.ReadLine() ?? "";

            string resultado = autenticacion.Registrar(
                nombre, apellido, nombreUsuario, email, contrasenia);

            Console.WriteLine($"\n{resultado}");
        }

        public static Usuario? SolicitarInicioSesion(Autenticacion autenticacion)
        {
            Console.WriteLine("\n=== INICIAR SESIÓN ===");

            string nombreUsuario = EntradaConsola.LeerTextoObligatorio("Nombre de usuario: ");

            Console.Write("Contraseña: ");
            string contrasenia = Console.ReadLine() ?? "";

            Usuario? usuario = autenticacion.IniciarSesion(nombreUsuario, contrasenia);

            if (usuario == null)
                Console.WriteLine("\nUsuario o contraseña incorrectos.");

            return usuario;
        }

        public static void EjecutarMenuUsuario(
            Usuario usuario,
            Autorizacion autorizacion,
            PeliculaDAO peliculaDAO)
        {
            bool sesionActiva = true;

            while (sesionActiva)
            {
                MostrarMenu(usuario, autorizacion);
                int opcion = EntradaConsola.LeerOpcion();

                switch (opcion)
                {
                    case 0:
                        sesionActiva = false;
                        Console.WriteLine("\nSesión cerrada.");
                        break;

                    case 1:
                        GestionPeliculas.MostrarCatalogo(peliculaDAO);
                        break;

                    case 2:
                        Console.WriteLine($"\nUsuario: {usuario.NombreUsuario}");
                        Console.WriteLine($"Rol: {usuario.Rol}");
                        break;

                    case 3:
                        if (!autorizacion.EsAdministrador(usuario))
                        {
                            MostrarAccesoDenegado();
                            break;
                        }
                        GestionPeliculas.AgregarPelicula(peliculaDAO);
                        break;

                    case 4:
                        if (!autorizacion.EsAdministrador(usuario))
                        {
                            MostrarAccesoDenegado();
                            break;
                        }
                        GestionPeliculas.ModificarPelicula(peliculaDAO);
                        break;

                    case 5:
                        if (!autorizacion.EsAdministrador(usuario))
                        {
                            MostrarAccesoDenegado();
                            break;
                        }
                        GestionPeliculas.EliminarPelicula(peliculaDAO);
                        break;

                    default:
                        Console.WriteLine("\nLa opción ingresada no existe.");
                        break;
                }
            }
        }

        private static void MostrarMenu(Usuario usuario, Autorizacion autorizacion)
        {
            Console.WriteLine("\n=== MENÚ MASITATV ===");
            Console.WriteLine("1. Ver catálogo");
            Console.WriteLine("2. Ver mi cuenta");

            if (autorizacion.EsAdministrador(usuario))
            {
                Console.WriteLine("3. Agregar película");
                Console.WriteLine("4. Modificar película");
                Console.WriteLine("5. Eliminar película");
            }

            Console.WriteLine("0. Cerrar sesión");
        }

        private static void MostrarAccesoDenegado()
        {
            Console.WriteLine("\nNo tenés permisos de administrador.");
        }
    }

    public static class EntradaConsola
    {
        public static int LeerOpcion()
        {
            while (true)
            {
                Console.Write("\nElegí una opción: ");
                if (int.TryParse(Console.ReadLine(), out int opcion))
                    return opcion;

                Console.WriteLine("Entrada inválida. Ingresá un número.");
            }
        }

        public static string LeerTextoObligatorio(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);
                string texto = (Console.ReadLine() ?? "").Trim();

                if (!string.IsNullOrWhiteSpace(texto))
                    return texto;

                Console.WriteLine("Este campo no puede estar vacío.");
            }
        }

        public static int LeerAnioEstreno()
        {
            while (true)
            {
                Console.Write("Año de estreno: ");

                if (int.TryParse(Console.ReadLine(), out int anio) &&
                    anio >= 1880 && anio <= DateTime.Now.Year)
                    return anio;

                Console.WriteLine($"Ingresá un año entre 1880 y {DateTime.Now.Year}.");
            }
        } 

        public static int LeerDuracion()
        {
            while (true)
            {
                Console.Write("Duración en minutos: ");

                if (int.TryParse(Console.ReadLine(), out int duracion) && duracion > 0)
                    return duracion;

                Console.WriteLine("La duración debe ser un número entero mayor que cero.");
            }
        }
    }




}
