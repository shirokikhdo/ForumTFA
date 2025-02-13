using Forums.Storage.Entities;

namespace Forums.Storage.Tests;

public class SignInStorageFixture : StorageTestFixture
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        await using var dbContext = GetDbContext();
        
        await dbContext.Users.AddRangeAsync(new User
        {
            UserId = Guid.Parse("8B41C23E-123E-4F4A-93F0-BEBF9916C8B3"),
            Login = "testUser",
            Salt = new byte[] { 1 },
            PasswordHash = new byte[] { 2 }
        }, new User
        {
            UserId = Guid.Parse("85895444-65F3-47D8-857D-88F289E83D56"),
            Login = "another User",
            Salt = new byte[] { 1 },
            PasswordHash = new byte[] { 2 }
        });

        await dbContext.SaveChangesAsync();
    }
}