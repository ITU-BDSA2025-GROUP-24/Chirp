using Microsoft.EntityFrameworkCore;
using Chirp.Infrastructure;
using Chirp.Core;

namespace Chirp.Tests;


public class AuthorRepositoryTests
{
    private ChirpDBContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ChirpDBContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // fresh DB for each test
            .Options;

        return new ChirpDBContext(options);
    }
    
    [Fact]
    public async Task CreateNewAuthor_ShouldAddAuthorToDatabase()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var repo = new AuthorRepository(dbContext);

        // Act
        await repo.CreateNewAuthor("Tester", "tester@example.com");

        // Assert
        var author = await dbContext.Authors.FirstOrDefaultAsync(a => a.Name == "Tester");
        Assert.NotNull(author);
        Assert.Equal("Tester", author.Name);
        Assert.Equal("tester@example.com", author.Email);
    }
    
    [Fact]
    public async Task UserExists_ShouldReturnTrue_IfAuthorExists()
    {
        
        //Arrange
        var dbContext = GetInMemoryDbContext();
        dbContext.Authors.Add(new Author { Name = "Tester", Email = "test@example.com", AuthorId = Guid.NewGuid() });
        await dbContext.SaveChangesAsync();

        var repo = new AuthorRepository(dbContext);

        //Act
        var exists = await repo.UserExists("Tester");

        //Assert
        Assert.True(exists);
    }
    
    [Fact]
    public async Task UserExists_ShouldReturnFalse_IfAuthorDoesNotExist()
    {
        //Arrange
        var dbContext = GetInMemoryDbContext();
        var repo = new AuthorRepository(dbContext);

        //Act
        var exists = await repo.UserExists("Tester");

        //Assert
        Assert.False(exists);
    }
    
    [Fact]
    public async Task GetAuthorByName_ShouldReturnAuthorInfo_IfExists()
    {
        //Arrange
        var dbContext = GetInMemoryDbContext();
        dbContext.Authors.Add(new Author { Name = "Tester", Email = "tester@example.com", AuthorId = Guid.NewGuid() });
        await dbContext.SaveChangesAsync();

        var repo = new AuthorRepository(dbContext);

        //Act
        var result = await repo.GetAuthorByName("Tester");

        //Assert
        Assert.Equal("Tester", result.Name);
        Assert.Equal("tester@example.com", result.Email);
    }
    
    [Fact]
    public async Task GetAuthorByName_ShouldThrow_IfNotExists()
    {
        //Arrange
        var dbContext = GetInMemoryDbContext();
        var repo = new AuthorRepository(dbContext);

        //Act & Assert
        await Assert.ThrowsAsync<UserNotFound>(() => repo.GetAuthorByName("NonExistent"));
    }

    [Fact]
    public async Task GetAuthorByEmail_ShouldReturnAuthorInfo_IfExists()
    {
        //Arrange
        var dbContext = GetInMemoryDbContext();
        dbContext.Authors.Add(new Author { Name = "Tester", Email = "tester@example.com",  AuthorId = Guid.NewGuid() });
        await dbContext.SaveChangesAsync();

        var repo = new AuthorRepository(dbContext);
        
        //Act
        var result = await repo.GetAuthorByEmail("tester@example.com");
        
        //Assert
        Assert.Equal("Tester", result.Name);
        Assert.Equal("tester@example.com", result.Email);
    }
    
    [Fact]
    public async Task GetAuthorByEmail_ShouldThrow_IfNotExists()
    {
        //Arrange
        var dbContext = GetInMemoryDbContext();
        var repo = new AuthorRepository(dbContext);

        //Act & Assert
        await Assert.ThrowsAsync<UserNotFound>(() => repo.GetAuthorByEmail("NonExistent"));
    }
    
    [Fact]
    public async Task FollowUser()
    {
        //Arrange
        var dbContext = GetInMemoryDbContext();
        var repo = new AuthorRepository(dbContext);
        await repo.CreateNewAuthor("Tester1", "Tester1@example.com");
        await repo.CreateNewAuthor("Tester2", "Tester2@example.com");
        AuthorDTO author1 = await repo.GetAuthorByName("Tester1");
        AuthorDTO author2 = await repo.GetAuthorByName("Tester2");
        //Act
        await repo.AddFollowAsync(author1.Name, author2.AuthorId);
        var assert = author1.FollowsId[0];
        //Assert
        Assert.Equal(author2.AuthorId, assert);
    }
    [Fact]
    public async Task UnFollowUser()
    {
        //Arrange
        var dbContext = GetInMemoryDbContext();
        var repo = new AuthorRepository(dbContext);
        await repo.CreateNewAuthor("Tester1", "Tester1@example.com");
        await repo.CreateNewAuthor("Tester2", "Tester2@example.com");
        AuthorDTO author1 = await repo.GetAuthorByName("Tester1");
        AuthorDTO author2 = await repo.GetAuthorByName("Tester2");
        //Act
        await repo.AddFollowAsync(author1.Name, author2.AuthorId);
        await repo.UnFollowAsync(author1.Name, author2.AuthorId);
        var assert = author1.FollowsId;
        //Assert
        Assert.Empty(assert);
    }
}