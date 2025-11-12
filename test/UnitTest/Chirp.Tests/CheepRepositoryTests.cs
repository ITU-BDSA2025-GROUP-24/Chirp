using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using Chirp.Core;

namespace Chirp.Tests;



public class CheepRepositoryTests
{
    
    private ChirpDBContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB for each test
            .Options;

        return new ChirpDBContext(options);
    }
    
    /*
    [Fact]
    public async Task CreateCheep_ShouldAddNewAuthorAndCheep()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repo = new CheepRepository(dbContext, skipMigrations: true);


        var username = "Chris";
        var email = "Chris@123.com";
        var newCheep = "This is a test cheep!";
       

        // Act
        var resultId = await repo.CreateCheep(username, email, newCheep);

        // Assert
        var cheep = await dbContext.Cheeps.Include(c => c.Author).FirstOrDefaultAsync();
    
        Assert.NotNull(cheep);
        Assert.Equal("Chris", cheep.Author.Name);
        Assert.Equal("This is a test cheep!", cheep.Text);
        Assert.Equal(resultId, cheep.CheepId);
    }*/
}
