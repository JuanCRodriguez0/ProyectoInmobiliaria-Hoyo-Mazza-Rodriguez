using System.ComponentModel.DataAnnotations;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class TipoInmueble
    {
        [Key]
        public int IdTipoInmueble { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(100, MinimumLength = 3)]
        [RegularExpression(@"^[A-Za-zÀ-ÿ\s]+$", ErrorMessage = "La descripción solo puede contener letras")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = "";
    }
}