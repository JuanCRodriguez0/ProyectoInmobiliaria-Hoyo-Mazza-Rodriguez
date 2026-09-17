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

        
        [DataType(DataType.Date)]
        [Display(Name = "Fecha hasta original")]
        public DateTime FechaHastaOriginal { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de terminación efectiva")]
        public DateTime? FechaTerminacionEfectiva { get; set; }

        [Display(Name = "Multa")]
        public decimal? Multa { get; set; }

        [Display(Name = "Terminada anticipadamente")]
        public bool Terminada { get; set; } = false;


        public int? IdUsuarioCreador { get; set; }
        public int? IdUsuarioTerminador { get; set; }

        /// <summary>
        /// Si esta reserva nació de una renovación/extensión, referencia a la
        /// reserva original. La reserva original NUNCA se modifica: renovar
        /// siempre genera una fila nueva.
        /// </summary>
        [Display(Name = "Renovación de la reserva")]
        public int? IdReservaOrigen { get; set; }


        [NotMapped]
        [Display(Name = "Inquilino")]
        public string? NombreInquilino { get; set; }

        [NotMapped]
        [Display(Name = "Inmueble")]
        public string? DireccionInmueble { get; set; }

        [NotMapped]
        [Display(Name = "Creada por")]
        public string? NombreUsuarioCreador { get; set; }

        [NotMapped]
        [Display(Name = "Terminada por")]
        public string? NombreUsuarioTerminador { get; set; }

        [NotMapped]
        public int DiasOriginales => (FechaHastaOriginal - FechaDesde).Days;
    }
}