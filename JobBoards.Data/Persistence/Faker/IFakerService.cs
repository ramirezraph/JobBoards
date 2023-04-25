using JobBoards.Data.Identity;

namespace JobBoards.Data.Persistence.Faker;

public interface IFakerService
{
    public Task<List<ApplicationUser>> GenerateFakeUsers(int size, string role = "User");
}