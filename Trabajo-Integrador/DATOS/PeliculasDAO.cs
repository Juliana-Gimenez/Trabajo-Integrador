using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Trabajo_Integrador.Modelos;

namespace Trabajo_Integrador.Datos
{
    public class PeliculaDAO
    {
        private readonly ConexionDB _conexionDB;

        public PeliculaDAO()
        {
            _conexionDB = new ConexionDB();
        }

        public void AgregarPelicula(Pelicula pelicula)
        {
            using (SqliteConnection conexion = _conexionDB.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                    INSERT INTO Peliculas
                    (Titulo, Genero, Anio, Duracion, Sinopsis)
                    VALUES
                    (@titulo, @genero, @anio, @duracion, @sinopsis);";

                using (SqliteCommand comando =new SqliteCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue(
                        "@titulo", pelicula.Titulo);

                    comando.Parameters.AddWithValue(
                        "@genero", pelicula.Genero);

                    comando.Parameters.AddWithValue(
                        "@anio", pelicula.Anio);

                    comando.Parameters.AddWithValue(
                        "@duracion", pelicula.Duracion);

                    comando.Parameters.AddWithValue(
                        "@sinopsis", pelicula.Sinopsis);

                    comando.ExecuteNonQuery();
                }
            }
        }

        public List<Pelicula> ObtenerTodas()
        {
            List<Pelicula> peliculas = new List<Pelicula>();

            using (SqliteConnection conexion = _conexionDB.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                    SELECT Id, Titulo, Genero, Anio,
                           Duracion, Sinopsis
                    FROM Peliculas
                    ORDER BY Titulo;";

                using (SqliteCommand comando = new SqliteCommand(consulta, conexion))
                    
                using (SqliteDataReader lector = comando.ExecuteReader())
                    
                {
                    while (lector.Read())
                    {
                        Pelicula pelicula = new Pelicula
                        {
                            Id = lector.GetInt32(0),
                            Titulo = lector.GetString(1),
                            Genero = lector.GetString(2),
                            Anio = lector.GetInt32(3),
                            Duracion = lector.GetInt32(4),
                            Sinopsis = lector.GetString(5)
                        };

                        peliculas.Add(pelicula);
                    }
                }
            }

            return peliculas;
        }


        public bool ModificarPelicula(Pelicula pelicula)
        {
            using (SqliteConnection conexion = _conexionDB.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                    UPDATE Peliculas
                    SET Titulo = @titulo,
                        Genero = @genero,
                        Anio = @anio,
                        Duracion = @duracion,
                        Sinopsis = @sinopsis
                    WHERE Id = @id;";

                using (SqliteCommand comando = new SqliteCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@titulo", pelicula.Titulo);
                    comando.Parameters.AddWithValue("@genero", pelicula.Genero);
                    comando.Parameters.AddWithValue("@anio", pelicula.Anio);
                    comando.Parameters.AddWithValue("@duracion", pelicula.Duracion);
                    comando.Parameters.AddWithValue("@sinopsis", pelicula.Sinopsis);
                    comando.Parameters.AddWithValue("@id", pelicula.Id);

                    int filasModificadas = comando.ExecuteNonQuery();

                    return filasModificadas > 0;
                }
            }
        }

        public bool EliminarPelicula(int id)
        {
            using (SqliteConnection conexion = _conexionDB.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                    DELETE FROM Peliculas
                    WHERE Id = @id;
                ";
                
                using(SqliteCommand comando = new SqliteCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    int filasEliminadas = comando.ExecuteNonQuery();
                    return filasEliminadas > 0;
                }
            }
        }

        public bool ExistePelicula(string titulo, int anio, int idExcluir = 0)
        {
            using (SqliteConnection conexion = _conexionDB.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                        SELECT COUNT(*)
                        FROM Peliculas
                        WHERE Titulo = @titulo
                        AND Anio = @anio
                        AND Id != @idExcluir;";

                //COUNT(*) cuenta cuantos registros cumplen la condición.
                //WHERE filtra las peliculas por titulo.
                //AND exije que también coincida el año.

                using (SqliteCommand comando = new SqliteCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@titulo", titulo);
                    comando.Parameters.AddWithValue("@anio", anio);
                    comando.Parameters.AddWithValue("@idExcluir", idExcluir);

                    long cantidad = (long)comando.ExecuteScalar();
                    //ExecuteScalar() devuelve el resultado único de la consulta: en este caso, la cantidad encontrada.

                    return cantidad > 0; //Devuelve true si ya existe una pelicula con esos datos.
                }
            }
        }



    }
}

