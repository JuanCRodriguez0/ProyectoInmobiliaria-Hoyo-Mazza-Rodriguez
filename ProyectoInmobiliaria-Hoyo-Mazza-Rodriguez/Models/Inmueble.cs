using System.ComponentModel.DataAnnotations;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class Inmueble
    {
        [Key]
        public int IdInmueble { get; set; }

        [Required]
        public int IdPropietario { get; set; }

        [Required]
        public int IdTipoInmueble { get; set; }

        [Required]
        public string Direccion { get; set; } = "";

        [Required]
        [Range(1, 100)]
        public int Cupo { get; set; }

        [Required]
        public int Ambientes { get; set; }

        [Required]
        public decimal Superficie { get; set; }

        [Required]
        public decimal PrecioPorDia { get; set; }

        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }

        public bool Disponible { get; set; } = true;
        public bool Estado { get; set; } = true;

        public string? Portada { get; set; }

        // Solo para mostrar en las vistas (vienen del JOIN)
        public string? NombrePropietario { get; set; }
        public string? DescripcionTipo { get; set; }
    }
}