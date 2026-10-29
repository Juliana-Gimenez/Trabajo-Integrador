using System;
using Microsoft.Data.Sqlite;
using Trabajo_Integrador.Modelos;

namespace Trabajo_Integrador.Datos
{
    public class UsuarioDAO
    {
        private readonly ConexionDB _conexionDB;

        public UsuarioDAO()
        {
            _conexionDB = new ConexionDB();
        }

        public bool ExisteUsuario(string nombreUsuario) //Comprueba si un nombre de usuario ya está registrado.
        {
            using (SqliteConnection conexion = _conexionDB.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                    SELECT COUNT(*)
                    FROM Usuarios
                    WHERE NombreUsuario = $nombreUsuario;";

                using (SqliteCommand comando = new SqliteCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue(
                        "$nombreUsuario", nombreUsuario);

                    long cantidad = (long)comando.ExecuteScalar();

                    return cantidad > 0;
                }
            }
        }

        public bool ExisteEmail(string email) //Comprueba si un correo ya está registrado
        {
            using (SqliteConnection conexion = _conexionDB.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                    SELECT COUNT(*)
                    FROM Usuarios
                    WHERE Email = $email;";

                using (SqliteCommand comando = new SqliteCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("$email", email);

                    long cantidad = (long)comando.ExecuteScalar();

                    return cantidad > 0;
                }
            }
        }

        public void RegistrarUsuario(Usuario usuario) //Inserta un nuevo usuario en SQLite
        {
            using (SqliteConnection conexion = _conexionDB.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                    INSERT INTO Usuarios
                    (
                        Nombre,
                        Apellido,
                        NombreUsuario,
                        Email,
                        ContraseniaHash,
                        ContraseniaSalt,
                        Rol,
                        FechaRegistro,
                        SuscripcionActiva
                    )
                    VALUES
                    (
                        $nombre,
                        $apellido,
                        $nombreUsuario,
                        $email,
                        $contraseniaHash,
                        $contraseniaSalt,
                        $rol,
                        $fechaRegistro,
                        $suscripcionActiva
                    );";

                using (SqliteCommand comando = new SqliteCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("$nombre", usuario.Nombre);

                    comando.Parameters.AddWithValue("$apellido", usuario.Apellido);

                    comando.Parameters.AddWithValue("$nombreUsuario", usuario.NombreUsuario);

                    comando.Parameters.AddWithValue("$email", usuario.Email);

                    comando.Parameters.AddWithValue("$contraseniaHash", usuario.ContraseniaHash);

                    comando.Parameters.AddWithValue("$contraseniaSalt", usuario.ContraseniaSalt);

                    comando.Parameters.AddWithValue("$rol", usuario.Rol);

                    comando.Parameters.AddWithValue("$fechaRegistro", usuario.FechaRegistro.ToString("O"));

                    comando.Parameters.AddWithValue("$suscripcionActiva", usuario.SuscripcionActiva ? 1 : 0);

                    comando.ExecuteNonQuery();
                }
            }
        }

        public Usuario? BuscarPorNombreUsuario(string nombreUsuario) //Busca un usuario y devuelve sus datos si existe
        {
            using (SqliteConnection conexion = _conexionDB.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
                    SELECT
                        Id,
                        Nombre,
                        Apellido,
                        NombreUsuario,
                        Email,
                        ContraseniaHash,
                        ContraseniaSalt,
                        Rol,
                        FechaRegistro,
                        SuscripcionActiva
                    FROM Usuarios
                    WHERE NombreUsuario = $nombreUsuario;";

                using (SqliteCommand comando = new SqliteCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("$nombreUsuario", nombreUsuario);

                    using (SqliteDataReader lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            return new Usuario
                            {
                                Id = lector.GetInt32(0),
                                Nombre = lector.GetString(1),
                                Apellido = lector.GetString(2),
                                NombreUsuario = lector.GetString(3),
                                Email = lector.GetString(4),
                                ContraseniaHash = lector.GetString(5),
                                ContraseniaSalt = lector.GetString(6),
                                Rol = lector.GetString(7),
                                FechaRegistro = DateTime.Parse(lector.GetString(8)),
                                SuscripcionActiva = lector.GetInt32(9) == 1
                            };
                        }
                    }
                }
            }

            return null;
        }

        //Este método se ha utilizado para lograr modificar el rol del administrador.
        public bool CambiarRol(string nombreUsuario, string nuevoRol)
        {
            using (SqliteConnection conexion = _conexionDB.ObtenerConexion())
            {
                conexion.Open();

                string consulta = @"
            UPDATE Usuarios
            SET Rol = $rol
            WHERE NombreUsuario = $nombreUsuario;";

                using (SqliteCommand comando = new SqliteCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("$rol", nuevoRol);
                    comando.Parameters.AddWithValue("$nombreUsuario", nombreUsuario);

                    int filasModificadas = comando.ExecuteNonQuery();

                    return filasModificadas > 0;
                }
            }
        }


    }


}

