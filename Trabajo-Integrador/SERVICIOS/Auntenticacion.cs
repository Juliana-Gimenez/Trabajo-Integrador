using System;
using System.Security.Cryptography;
using Trabajo_Integrador.Datos;
using Trabajo_Integrador.Modelos;

namespace Trabajo_Integrador.Servicios
{
    public class Autenticacion
    {
        private readonly UsuarioDAO _usuarioDAO;

        public Autenticacion(UsuarioDAO _usuarioDAO)
        {
            this._usuarioDAO = _usuarioDAO; //Recibe un objeto UsuarioDAO y lo guarda para poder utilizarlo
        }

        public string Registrar(
            string nombre,
            string apellido,
            string nombreUsuario,
            string email,
            string contraseña)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(apellido) ||
                string.IsNullOrWhiteSpace(nombreUsuario) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrEmpty(contraseña))
            {
                return "Todos los campos son obligatorios.";
            }

            if (_usuarioDAO.ExisteUsuario(nombreUsuario))
            {
                return "Ese nombre de usuario ya está registrado.";
            }

            if (_usuarioDAO.ExisteEmail(email))
            {
                return "Ese correo electrónico ya está registrado.";
            }

            byte[] salt = RandomNumberGenerator.GetBytes(16); 
            //RandomNumberGenerator genera 16 bytes aleatorios medinate un generador criptográficamente seguro.
            //byte[]: arreglo cuyos valores van desde 0 a 255 (por el tipo de dato que es byte).
            // salt es una variable que guarda una colección de bytes.

            string saltTexto = Convert.ToBase64String(salt);
            //Convert.ToBase64String convierte los bytes almacenados en salt en cadenas de texto.

            string hash = CrearHash(contraseña, salt);

            Usuario usuario = new Usuario
            {
                Nombre = nombre.Trim(),
                Apellido = apellido.Trim(),
                NombreUsuario = nombreUsuario.Trim(),
                Email = email.Trim(),
                ContraseniaHash = hash,
                ContraseniaSalt = saltTexto,

                //El rol se asigna desde el texto
                Rol = "Usuario",

                FechaRegistro = DateTime.Now,
                SuscripcionActiva = false
            };

            _usuarioDAO.RegistrarUsuario(usuario);

            return "¡Registro exitoso! Ya podés iniciar sesión en Masita TV.";
        }

        public Usuario? IniciarSesion(
            string nombreUsuario,
            string contraseña)
        {
            Usuario? usuario = _usuarioDAO.BuscarPorNombreUsuario(nombreUsuario);  //Buscamos si el usuario existe

            if (usuario == null)//Si no existe un usuario, no hay contraseñas que validar.
            {
                return null;
            }

            byte[] salt = Convert.FromBase64String(usuario.ContraseniaSalt); //Se recupera el salt del usuario, que vuelve a convertirse en int para realizar el cálculo.

            string hashIngresado = CrearHash(contraseña, salt); //Se toma la contraseña ingresada y se calcula un nuevo hash usando el salt guardado de ese usuario

            byte[] hashOriginal = Convert.FromBase64String(usuario.ContraseniaHash);

            byte[] hashCalculado = Convert.FromBase64String(hashIngresado);

            bool contraseñaCorrecta = CryptographicOperations.FixedTimeEquals(hashOriginal, hashCalculado); //FixedTimeEquals es de la biblioteca criptográfica de Microsoft, y determina la igualdadde dos secuencias de bytes en un período de tiempo que depende de la longitud de las secuencias, pero no de sus valores.

            if (!contraseñaCorrecta)
            {
                return null; //Porque las credenciales no son válidas, devuelve null
            }

            return usuario; //Devuelve el objeto usuario si las credenciales pertenecientes a este son válidas.
        }

        private string CrearHash(string contraseña, byte[] salt) //contraseña es la variable que contiene a la contraseña que queremos analizar. "salt" tiene los bytes aleatorios asociados a esa contraseña.
        {
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2( //PBKDF2 es una función de derivación de clases a partir de contraseñas. Combina la contraseña con el salt y realiza muchas iteraciones de cálculo. Muchas iteraciones para que cada intento de comprobar una contraseñatenga un costo computacional mayor. Así se encarecen los ataques que prueban muchas contraseñas.
                contraseña,
                salt,
                600000, //Cantidad de iteraciones de PBKDF2 configuradas
                HashAlgorithmName.SHA256, //Función hash utilizada internamente por PBKDF2
                32); //Cantidad de bytes del resultado derivado

            return Convert.ToBase64String(hash); //Devuelve el código en tipo string para poder almacenarlo en la base de datos.
        }
    }
}

