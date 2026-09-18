using System.ComponentModel.DataAnnotations;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; } = "";

        public string Clave { get; set; } = "";

        [DataType(DataType.Password)]
        public string? ClaveNueva { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [RegularExpression(@"^[A-Za-zÀ-ÿ'´\s]+$", ErrorMessage = "El nombre solo puede contener letras")]
        public string Nombre { get; set; } = "";

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [RegularExpression(@"^[A-Za-zÀ-ÿ'´\s]+$", ErrorMessage = "El apellido solo puede contener letras")]
        public string Apellido { get; set; } = "";

        [Required]
        public string Rol { get; set; } = "Empleado";

        public string? Avatar { get; set; }

        public bool Estado { get; set; } = true;
    }
}