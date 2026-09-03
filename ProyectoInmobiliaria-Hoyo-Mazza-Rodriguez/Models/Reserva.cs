using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class Reserva
    {
        [Key]
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inquilino")]
        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un inmueble")]
        [Display(Name = "Inmueble")]
        public int IdInmueble { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ingrese un monto por día válido")]
        [Display(Name = "Monto por día")]
        public decimal MontoPorDia { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha desde")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha desde")]
        public DateTime FechaDesde { get; set; }

        [Required(ErrorMessage = "Debe indicar la fecha hasta")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha hasta")]
        public DateTime FechaHasta { get; set; }

        // ---- Campos auxiliares, no se persisten directamente ----

        [NotMapped]
        [Display(Name = "Inquilino")]
        public string? NombreInquilino { get; set; }

        [NotMapped]
        [Display(Name = "Inmueble")]
        public string? DireccionInmueble { get; set; }
    }
}