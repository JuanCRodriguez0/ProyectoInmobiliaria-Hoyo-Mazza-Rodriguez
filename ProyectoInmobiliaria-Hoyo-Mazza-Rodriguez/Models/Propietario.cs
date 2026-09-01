using System.ComponentModel.DataAnnotations;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class Propietario
    {
        [Key]
        public int IdPropietario { get; set; }

        [Required]
        public string Dni { get; set; } = "";

        [Required]
        public string Nombre { get; set; } = "";

        [Required]
        public string Apellido { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        public string Direccion { get; set; } = "";

        [Required]
        public string Telefono { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        
    }
}