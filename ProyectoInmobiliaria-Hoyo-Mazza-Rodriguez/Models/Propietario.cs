using System.ComponentModel.DataAnnotations;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class Propietario
    {
        [Key]
        public int IdPropietario { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [RegularExpression(@"^\d{7,8}$", ErrorMessage = "El DNI debe tener 7 u 8 dígitos numéricos, sin puntos")]
        public string Dni { get; set; } = "";

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 2)]
        [RegularExpression(@"^[A-Za-zÀ-ÿ'´\s]+$", ErrorMessage = "El nombre solo puede contener letras")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, MinimumLength = 2)]
        [RegularExpression(@"^[A-Za-zÀ-ÿ'´\s]+$", ErrorMessage = "El apellido solo puede contener letras")]
        public string Apellido { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, MinimumLength = 5)]
        [RegularExpression(@"^(?=.*[A-Za-zÀ-ÿ])[A-Za-z0-9À-ÿ.,°#\-\s]+$",
            ErrorMessage = "La dirección no puede contener solo números")]
        public string Direccion { get; set; } = "";

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^[0-9+\-\s()]{6,25}$", ErrorMessage = "Ingrese un teléfono válido")]
        public string Telefono { get; set; } = "";

        [Required]
        [EmailAddress(ErrorMessage = "Ingrese un email válido")]
        public string Email { get; set; } = "";
    }
}