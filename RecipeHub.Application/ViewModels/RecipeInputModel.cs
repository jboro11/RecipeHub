using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeHub.Application.ViewModels
{
    public class IngredientInputModel
    {
        [Required(ErrorMessage = "Název ingredience je povinný.")]
        [StringLength(100, ErrorMessage = "Název ingredience může mít max. 100 znaků.")]
        [Display(Name = "Ingredience")]
        public string Name { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Množství může mít max. 100 znaků.")]
        [Display(Name = "Množství")]
        public string? Quantity { get; set; }
    }

    public class RecipeInputModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Název je povinný.")]
        [StringLength(150, MinimumLength = 3, ErrorMessage = "Název musí mít 3–150 znaků.")]
        [Display(Name = "Název")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Krátký popis")]
        [StringLength(500, ErrorMessage = "Krátký popis může mít max. 500 znaků.")]
        public string? ShortDescription { get; set; }

        [Required(ErrorMessage = "Popis je povinný.")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Popis musí mít 10–2000 znaků.")]
        [Display(Name = "Postup")]
        public string Description { get; set; } = string.Empty;

        [Url(ErrorMessage = "Zadejte platnou URL adresu.")]
        [StringLength(500, ErrorMessage = "URL může mít max. 500 znaků.")]
        [Display(Name = "Obrázek (URL)")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Zadejte kategorii.")]
        [Range(1, int.MaxValue, ErrorMessage = "Vyberte platnou kategorii.")]
        [Display(Name = "Kategorie")]
        public int? CategoryId { get; set; }

        [Required(ErrorMessage = "Zadejte časovou náročnost.")]
        [StringLength(100, ErrorMessage = "Časová náročnost může mít max. 100 znaků.")]
        [Display(Name = "Časová náročnost")]
        public string PrepTime { get; set; } = string.Empty;

        [Range(1, 3, ErrorMessage = "Obtížnost musí být 1–3.")]
        [Display(Name = "Obtížnost")]
        public int Difficulty { get; set; } = 1;

        [MinLength(2, ErrorMessage = "Přidejte alespoň dvě ingredience.")]
        public List<IngredientInputModel> Ingredients { get; set; } = new();
    }
}


