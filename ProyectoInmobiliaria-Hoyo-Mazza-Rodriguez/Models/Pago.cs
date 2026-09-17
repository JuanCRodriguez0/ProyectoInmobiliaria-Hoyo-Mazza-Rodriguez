using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class Pago
    {
        [Key]
        public int IdPago { get; set; }

        [Required(ErrorMessage = "Debe indicar la reserva")]
        [Display(Name = "Reserva")]
        public int IdReserva { get; set; }

        [Required(ErrorMessage = "El concepto es obligatorio")]
        [StringLength(200, ErrorMessage = "El concepto no puede superar los 200 caracteres")]
        [Display(Name = "Concepto")]
        public string Concepto { get; set; } = "";

        [Required(ErrorMessage = "Debe indicar la fecha de pago")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de pago")]
        public DateTime FechaPago { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Debe indicar el importe")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El importe debe ser mayor a cero")]
        [Display(Name = "Importe")]
        public decimal Importe { get; set; }

        [Display(Name = "Anulado")]
        public bool Anulado { get; set; } = false;

        [Display(Name = "Creado por")]
        public int? IdUsuarioCreador { get; set; }

        [Display(Name = "Anulado por")]
        public int? IdUsuarioAnulador { get; set; }

        [NotMapped]
        [Display(Name = "Creado por")]
        public string? NombreCreador { get; set; }

        [NotMapped]
        [Display(Name = "Anulado por")]
        public string? NombreAnulador { get; set; }

        [NotMapped]
        [Display(Name = "Inquilino")]
        public string? NombreInquilino { get; set; }

        [NotMapped]
        [Display(Name = "Inmueble")]
        public string? DireccionInmueble { get; set; }
    }
}