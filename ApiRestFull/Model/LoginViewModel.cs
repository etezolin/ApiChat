using System.ComponentModel.DataAnnotations;

namespace ApiRestFull.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "UserName é um campo obrigatório.")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password é um campo obrigatório.")]
        public string Password { get; set; }
    }
}