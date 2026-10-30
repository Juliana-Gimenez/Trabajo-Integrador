using Trabajo_Integrador.Datos;
using Trabajo_Integrador.Modelos;

namespace Trabajo_Integrador.Metodos
{
    public static class GestionPeliculas
    {
        public static void MostrarCatalogo(PeliculaDAO peliculaDAO)
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

        public static void AgregarPelicula(PeliculaDAO peliculaDAO)
        {
            Console.WriteLine("\n=== AGREGAR PELÍCULA ===");

            string titulo = EntradaConsola.LeerTextoObligatorio("Título: ");
            string genero = EntradaConsola.LeerTextoObligatorio("Género: ");
            int anio = EntradaConsola.LeerAnioEstreno();
            int duracion = EntradaConsola.LeerDuracion();
            string sinopsis = EntradaConsola.LeerTextoObligatorio("Sinopsis: ");

            if (peliculaDAO.ExistePelicula(titulo, anio))
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

        public static void ModificarPelicula(PeliculaDAO peliculaDAO)
        {
            Console.WriteLine("\n=== MODIFICAR PELÍCULA ===");

            List<Pelicula> peliculas = peliculaDAO.ObtenerTodas();

            if (peliculas.Count == 0)
            {
                Console.WriteLine("No hay películas para modificar.");
                return;
            }

            foreach (Pelicula pelicula in peliculas)
                Console.WriteLine($"ID: {pelicula.Id} | {pelicula.Titulo} ({pelicula.Anio})");

            Console.Write("\nIngresá el ID de la película que querés modificar: ");
            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Ingresá un ID válido.");
                return;
            }

            Pelicula? seleccionada = peliculas.Find(p => p.Id == id);

            if (seleccionada == null)
            {
                Console.WriteLine("No existe una película con ese ID.");
                return;
            }

            Console.WriteLine("\nIngresá los nuevos datos.");

            string titulo = EntradaConsola.LeerTextoObligatorio("Título: ");
            string genero = EntradaConsola.LeerTextoObligatorio("Género: ");
            int anio = EntradaConsola.LeerAnioEstreno();
            int duracion = EntradaConsola.LeerDuracion();
            string sinopsis = EntradaConsola.LeerTextoObligatorio("Sinopsis: ");

            if (peliculaDAO.ExistePelicula(titulo, anio, id))
            {
                Console.WriteLine("\nYa existe otra película con ese título y año.");
                return;
            }

            Pelicula modificada = new Pelicula
            {
                Id = id,
                Titulo = titulo,
                Genero = genero,
                Anio = anio,
                Duracion = duracion,
                Sinopsis = sinopsis
            };

            if (peliculaDAO.ModificarPelicula(modificada))
                Console.WriteLine("\n¡Película modificada correctamente!");
            else
                Console.WriteLine("\nNo se pudo modificar la película.");
        }

        public static void EliminarPelicula(PeliculaDAO peliculaDAO)
        {
            Console.WriteLine("\n=== ELIMINAR PELÍCULA ===");

            List<Pelicula> peliculas = peliculaDAO.ObtenerTodas();

            if (peliculas.Count == 0)
            {
                Console.WriteLine("No hay películas para eliminar.");
                return;
            }

            foreach (Pelicula pelicula in peliculas)
                Console.WriteLine($"ID: {pelicula.Id} | {pelicula.Titulo} ({pelicula.Anio})");

            Console.Write("\nIngresá el ID de la película que querés eliminar: ");
            if (!int.TryParse(Console.ReadLine(), out int id) || id <= 0)
            {
                Console.WriteLine("Ingresá un ID válido.");
                return;
            }

            Pelicula? seleccionada = peliculas.Find(p => p.Id == id);

            if (seleccionada == null)
            {
                Console.WriteLine("No existe una película con ese ID.");
                return;
            }

            Console.WriteLine($"\nVas a eliminar: {seleccionada.Titulo} ({seleccionada.Anio})");
            Console.Write("¿Estás seguro? (S/N): ");
            string confirmacion = (Console.ReadLine() ?? "").Trim();

            if (!confirmacion.Equals("S", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\nEliminación cancelada.");
                return;
            }

            if (peliculaDAO.EliminarPelicula(id))
                Console.WriteLine("\n¡Película eliminada correctamente!");
            else
                Console.WriteLine("\nNo se pudo eliminar la película. Puede que ya no exista.");
        }
    }
}
