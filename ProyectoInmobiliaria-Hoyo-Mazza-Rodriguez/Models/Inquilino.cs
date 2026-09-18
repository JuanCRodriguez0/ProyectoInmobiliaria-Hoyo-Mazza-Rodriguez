using System.ComponentModel.DataAnnotations;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class Inquilino
    {
        [Key]
        public int IdInquilino { get; set; }

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

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^[0-9+\-\s()]{6,25}$", ErrorMessage = "Ingrese un teléfono válido")]
        public string Telefono { get; set; } = "";

        [Required]
        [EmailAddress(ErrorMessage = "Ingrese un email válido")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Indique al menos un garante")]
        [StringLength(500, MinimumLength = 3)]
        public string Garantes { get; set; } = "";

        [Required(ErrorMessage = "Indique el sueldo")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El sueldo debe ser mayor a cero")]
        public decimal Sueldo { get; set; }
    }
}