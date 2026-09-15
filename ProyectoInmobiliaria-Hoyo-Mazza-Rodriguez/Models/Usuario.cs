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

        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        public string Apellido { get; set; } = "";

        [Required]
        public string Rol { get; set; } = "Empleado";

        public string? Avatar { get; set; }

        public bool Estado { get; set; } = true;
    }
}