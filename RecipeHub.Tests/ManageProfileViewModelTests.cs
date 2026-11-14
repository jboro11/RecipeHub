using System.ComponentModel.DataAnnotations;
using RecipeHub.Application.ViewModels;
using Xunit;

namespace RecipeHub.Tests;

public class ManageProfileViewModelTests
{
    [Theory]
    [InlineData(null, null, true)]
    [InlineData("Current42!", "NewPassword42!", true)]
    [InlineData(null, "NewPassword42!", false)]
    [InlineData("Current42!", null, false)]
    public void PasswordChangeRequiresBothFields(string? current, string? updated, bool expectedValid)
    {
        var model = new ManageProfileViewModel
        {
            NickName = "tester",
            Email = "tester@example.invalid",
            CurrentPassword = current,
            NewPassword = updated
        };

        var results = new List<ValidationResult>();
        Assert.Equal(expectedValid, Validator.TryValidateObject(model, new ValidationContext(model), results, true));
    }
}
