using System.ComponentModel.DataAnnotations;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class Inquilinos
    {
        [Key]
        public int IdInquilino { get; set; }

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
        public string Telefono { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Garantes { get; set; } = ""; //Los vamos a escribir de la siguiente manera: "Apellido Nombre - DNI \n Apellido Nombre - DNI"

        [Required]
        public decimal Sueldo { get; set; }

        //public List<Reservas> Reservas { get; set; } = new List<Reservas>();
    }
}