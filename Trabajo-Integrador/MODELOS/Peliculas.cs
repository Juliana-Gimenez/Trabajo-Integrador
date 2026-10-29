using System;

namespace Trabajo_Integrador.Modelos
{
    public class Pelicula
    {
        public int Id { get; set; }

        public string Titulo { get; set; } = "";

        public string Genero { get; set; } = "";

        public int Anio { get; set; }

        public int Duracion { get; set; }

        public string Sinopsis { get; set; } = "";
    }
}

