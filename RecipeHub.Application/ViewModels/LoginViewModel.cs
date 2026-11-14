using System.ComponentModel.DataAnnotations;

namespace RecipeHub.Application.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Zadejte uživatelské jméno nebo e-mail.")]
        [Display(Name = "Uživatelské jméno / e-mail")]
        public string UserNameOrEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte heslo.")]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Zapamatovat")]
        public bool RememberMe { get; set; }
    }
}


