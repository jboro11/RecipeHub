using System.ComponentModel.DataAnnotations;

namespace RecipeHub.Application.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Zadejte přezdívku.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Přezdívka musí mít 3–50 znaků.")]
        [Display(Name = "Přezdívka")]
        public string NickName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte e-mail.")]
        [EmailAddress(ErrorMessage = "Zadejte platný e-mail.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte heslo.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Heslo musí mít alespoň 6 znaků.")]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrďte heslo.")]
        [Compare(nameof(Password), ErrorMessage = "Hesla se neshodují.")]
        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení hesla")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}


