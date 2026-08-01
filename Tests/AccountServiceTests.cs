public sealed class AccountServiceTests
{
    [Fact]
    public void ApplicationUser_HasSafeDefaults()
    {
        var user = new ApplicationUser();

        Assert.Equal(string.Empty, user.FullName);
        Assert.Null(user.ProfileImageUrl);
    }
}
