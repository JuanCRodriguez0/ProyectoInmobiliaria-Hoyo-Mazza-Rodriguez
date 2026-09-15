using System.ComponentModel.DataAnnotations;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class LoginViewModel
    {
        [Required, EmailAddress, Display(Name = "Email")]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password), Display(Name = "Contraseña")]
        public string Clave { get; set; } = "";
    }
}