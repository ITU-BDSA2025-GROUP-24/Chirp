using Chirp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
    
namespace Chirp.IntegrationTests;

public class AuthorRepositoryTest1 : TestBase
{
    
    [Fact]
    public async Task Test1()
    { 
        // Arrange
           var options = CreateInMemoryOptions();
           using var context = new ChirpDBContext(options);
           context.Database.EnsureCreated();

           var repo = new AuthorRepository(context);

        // Act
           await repo.CreateNewAuthor("Tester", "test@example.com");

        // Assert
           Assert.Equal(1, await context.Authors.CountAsync());
           var author = await context.Authors.FirstAsync();
           Assert.Equal("Tester", author.Name);
           Assert.Equal("test@example.com", author.Email);
       
    }
    
    [Fact]
    public async Task Test2()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ChirpDBContext(options);
        context.Database.EnsureCreated();

        var repo = new AuthorRepository(context);

        await repo.CreateNewAuthor("Tester", "test@example.com");

        // Act + Assert
        await Assert.ThrowsAsync<Exception>(() =>
            repo.CreateNewAuthor("Tester", "test@example.com"));
    }
    
    [Fact]
    public async Task Test3()
    {
        // Arrange
        var options = CreateInMemoryOptions();
        using var context = new ChirpDBContext(options);
        context.Database.EnsureCreated();

        var repo = new AuthorRepository(context);
        await repo.CreateNewAuthor("Tester", "test@example.com");

        // Act
        var dto = await repo.GetAuthorByName("Tester");

        // Assert
        Assert.Equal("Tester", dto.Name);
        Assert.Equal("test@example.com", dto.Email);
    }
    
    [Fact]
    public async Task Test4()
    {
        //Arrange
        var options = CreateInMemoryOptions();
        using var context = new ChirpDBContext(options);
        context.Database.EnsureCreated();
        
        //Act
        var repo = new AuthorRepository(context);

        //Assert
        await Assert.ThrowsAsync<UserNotFound>(() =>
            repo.GetAuthorByName("NonexistentUser"));
    }
    
    [Fact]
    public async Task Test5()
    {
        //Arrange
        var options = CreateInMemoryOptions();
        using var context = new ChirpDBContext(options);
        context.Database.EnsureCreated();
        
        context.Authors.Add(new Author 
        {
            AuthorId = Guid.NewGuid(),
            Name = "Tester",
            Email = "test@Example.com"
        });
        await context.SaveChangesAsync();
        
        //Act
        var repo = new AuthorRepository(context);

        var dto = await repo.GetAuthorByEmail("test@Example.com");
        
        //Assert
        Assert.Equal("Tester", dto.Name);
    }
}