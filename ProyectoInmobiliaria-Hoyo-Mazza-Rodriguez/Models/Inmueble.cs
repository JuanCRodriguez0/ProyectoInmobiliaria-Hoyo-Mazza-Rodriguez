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

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Ingrese una dirección más completa")]
        [RegularExpression(@"^(?=.*[A-Za-zÀ-ÿ])[A-Za-z0-9À-ÿ.,°#\-\s]+$",
            ErrorMessage = "La dirección no puede contener solo números o símbolos")]
        public string Direccion { get; set; } = "";

        [Required(ErrorMessage = "Indique el cupo (cantidad máxima de personas)")]
        [Range(1, 30, ErrorMessage = "El cupo debe ser un valor real, entre 1 y 30 personas")]
        [Display(Name = "Cupo (cantidad máxima de personas)")]
        public int Cupo { get; set; }

        [Required(ErrorMessage = "Indique la cantidad de ambientes")]
        [Range(1, 15, ErrorMessage = "La cantidad de ambientes debe ser un valor real, entre 1 y 15")]
        public int Ambientes { get; set; }

        [Required(ErrorMessage = "Indique la superficie")]
        [Range(1, 5000, ErrorMessage = "Ingrese una superficie válida (en m² aproximados)")]
        [Display(Name = "Superficie (m² aproximados)")]
        public decimal Superficie { get; set; }

        [Required(ErrorMessage = "Indique el precio por día")]
        [Range(1, 100000000, ErrorMessage = "Ingrese un precio por día válido")]
        [Display(Name = "Precio por día (en pesos)")]
        public decimal PrecioPorDia { get; set; }

        [Required(ErrorMessage = "Indique el porcentaje de seña")]
        [Range(0, 100, ErrorMessage = "El porcentaje de seña debe estar entre 0 y 100")]
        [Display(Name = "Porcentaje de seña al reservar (%)")]
        public decimal PorcentajeSenia { get; set; } = 30;

        [Range(-90, 90, ErrorMessage = "Latitud inválida")]
        public decimal? Latitud { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitud inválida")]
        public decimal? Longitud { get; set; }

        public bool Disponible { get; set; } = true;
        public bool Estado { get; set; } = true;

        public string? Portada { get; set; }

        // Solo para mostrar en las vistas (vienen del JOIN)
        public string? NombrePropietario { get; set; }
        public string? DescripcionTipo { get; set; }
    }
}