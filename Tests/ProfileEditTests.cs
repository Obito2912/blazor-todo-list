using System.ComponentModel.DataAnnotations;
using blazor_todo_list.Components;

public sealed class ProfileEditTests
{
    [Fact]
    public void ProfileForm_RejectsInvalidEmailAndUrl()
    {
        var model = new ProfileFormValue
        {
            FullName = "Quest User",
            Email = "not-an-email",
            ProfileImageUrl = "not-a-url"
        };
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, new ValidationContext(model), results, true);

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public void PasswordForm_RequiresMatchingConfirmation()
    {
        var model = new ChangePasswordFormValue
        {
            CurrentPassword = "current-password",
            NewPassword = "new-password",
            ConfirmNewPassword = "different-password"
        };
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, new ValidationContext(model), results, true);

        Assert.Contains(results, result => result.MemberNames.Contains(nameof(ChangePasswordFormValue.ConfirmNewPassword)));
    }
}
