using System.ComponentModel.DataAnnotations;

namespace RecipeHub.Application.ViewModels
{
    public class ManageProfileViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Zadejte přezdívku.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Přezdívka musí mít 3–50 znaků.")]
        [Display(Name = "Přezdívka")]
        public string NickName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadejte e-mail.")]
        [EmailAddress(ErrorMessage = "Zadejte platný e-mail.")]
        [Display(Name = "E-mail")]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Současné heslo")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Heslo musí mít 6–100 znaků.")]
        [Display(Name = "Nové heslo")]
        public string? NewPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(NewPassword) && string.IsNullOrWhiteSpace(CurrentPassword))
            {
                yield return new ValidationResult("Pro změnu hesla zadejte současné heslo.", new[] { nameof(CurrentPassword) });
            }

            if (!string.IsNullOrWhiteSpace(CurrentPassword) && string.IsNullOrWhiteSpace(NewPassword))
            {
                yield return new ValidationResult("Zadejte také nové heslo.", new[] { nameof(NewPassword) });
            }
        }
    }
}


