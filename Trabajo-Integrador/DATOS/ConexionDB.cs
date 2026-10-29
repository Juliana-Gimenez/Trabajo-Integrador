using System;
using Microsoft.Data.Sqlite;

namespace Trabajo_Integrador.Datos
{
    public class ConexionDB
    {
        private readonly string cadenaConexion = "Data Source = MasitaTV.db"; //Indica dónde se encuentra la base de datos. MasitaTV.db será el archivo de SQLite

        public SqliteConnection ObtenerConexion() //crea y devuelve una conexion para que otras clases puedan usarla.
        {
            return new SqliteConnection(cadenaConexion);
        }

        public void InicializarBaseDeDatos() //Abre la conexión y crea una tabla "Usuarios" si todavía no existe.
        {
            using (SqliteConnection conexion = ObtenerConexion())
            {
                conexion.Open();

                string consultaUsuarios = @"
                    CREATE TABLE IF NOT EXISTS Usuarios
                    ( 
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Nombre TEXT NOT NULL,
                        Apellido TEXT NOT NULL,
                        NombreUsuario TEXT NOT NULL UNIQUE,
                        Email TEXT NOT NULL UNIQUE,
                        ContraseniaHash TEXT NOT NULL,
                        ContraseniaSalt TEXT NOT NULL,
                        Rol TEXT NOT NULL DEFAULT 'Usuario',
                        FechaRegistro TEXT NOT NULL,
                        SuscripcionActiva INTEGER NOT NULL DEFAULT 0
                        )";

                using (SqliteCommand comando = new SqliteCommand(consultaUsuarios, conexion))
                {
                    comando.ExecuteNonQuery(); //Ejecuta instrucciones que modifican los datos, como el INSERT.
                }

                //CREATE TABLE IF NOT EXISTS evita intentar crear la tabla nuevamente si ya existe
                //UNIQUE impide que dos usuarios tengan el mismo nombre de usuario o correo electrónico
                //ROL permite distinguir entre admin y usuario común
                //SUSCRIPCION ACTIVA permite representar si la suscripcion está activa (1) o no (0)

                string consultaPeliculas = @"
                    CREATE TABLE IF NOT EXISTS Peliculas
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Titulo TEXT NOT NULL,
                        Genero TEXT NOT NULL,
                        Anio INTEGER NOT NULL,
                        Duracion INTEGER NOT NULL,
                        Sinopsis TEXT NOT NULL
                    );";

                //INTEGER PRIMARY KEY AUTOINCREMENT: SQLite genera un identificador nuevo para cada película.

                using (SqliteCommand comando = new SqliteCommand(
                    consultaPeliculas, conexion))
                {
                    comando.ExecuteNonQuery();
                }

            }
            
                
            
        }
    }
}
