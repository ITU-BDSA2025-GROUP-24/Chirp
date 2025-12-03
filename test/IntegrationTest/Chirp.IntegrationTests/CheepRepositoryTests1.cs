using Chirp.Infrastructure;
using Chirp.Core;
using Microsoft.EntityFrameworkCore;


namespace Chirp.IntegrationTests;


public class CheepRepositoryTests1 : TestBase
{
    private Task<(ChirpDBContext ctx, CheepRepository repo)> Setup()
    {
        var options = CreateInMemoryOptions();
        var ctx = new ChirpDBContext(options);
        ctx.Database.EnsureCreated();

        var repo = new CheepRepository(ctx, skipMigrations: true);
        return Task.FromResult((ctx, repo));
    }

    [Fact]
    public async Task CreateCheep_ShouldAddCheep()
    {
        //Arrange
        var (ctx, repo) = await Setup();

        var author = new Author
        {
            AuthorId = Guid.NewGuid(),
            Name = "Tester",
            Email = "test@Example.com"
        };
        
        ctx.Authors.Add(author);
        await ctx.SaveChangesAsync();
        
        //Act
        await repo.CreateCheep("Tester", "test@example.com", "Test World");
        
        //Assert
        var cheep = await ctx.Cheeps.Include(c => c.Author).FirstAsync();
        Assert.Equal("Test World", cheep.Text);
        Assert.Equal("Tester", cheep.Author.Name);
    }
    
    [Fact]
    public async Task FailsWhen_AuthorDontExists()
        {
            var (ctx, repo) = await Setup();
    
            await Assert.ThrowsAsync<UserNotFound>(() =>
                repo.CreateCheep("MissingUser", "", "Test message"));
        }

    [Fact]
    public async Task ReturnsCorrectPage()
    {
        var (ctx, repo) = await Setup();
        
        //Arrange
        var author = new Author
        {
            AuthorId = Guid.NewGuid(),
            Name = "Tester",
            Email = "test@example.com"
        };

        ctx.Authors.Add(author);

        for (int i = 0; i < 40; i++)
        {
            ctx.Cheeps.Add(new Cheep
            {
                CheepId = Guid.NewGuid(),
                Author = author,
                Text = "C" + i,
                TimeStamp = DateTime.UtcNow.AddMinutes(-i)
            });
        }

        await ctx.SaveChangesAsync();

        // Act
        var results = await repo.ReadCheep(pageNum: 1);

        // Assert
        Assert.Equal(32, results.Count);
    }

    [Fact]
    public async Task ReturnCheep_ByAuthor()
    {
        var (ctx, repo) = await Setup();
        
        //Arrange
        var tester1 = new Author { AuthorId = Guid.NewGuid(), Name = "Tester1", Email = "Tester@example.com" };
        var test2 = new Author {AuthorId = Guid.NewGuid(), Name = "Test2", Email = "test@example.com"};
        
        ctx.Authors.AddRange(tester1, test2);
        
        ctx.Cheeps.Add(new Cheep
        {
            CheepId = Guid.NewGuid(),
            Author = tester1,
            Text = "Tester msg",
            TimeStamp = DateTime.Now
        });

        ctx.Cheeps.Add(new Cheep
        {
            CheepId = Guid.NewGuid(),
            Author = test2,
            Text = "test msg",
            TimeStamp = DateTime.Now
        });

        await ctx.SaveChangesAsync();
        
        await ctx.SaveChangesAsync();

        // Act
        var tester1Cheeps = await repo.ReadCheep(author: "Tester1");

        // Assert
        Assert.Single(tester1Cheeps);
        Assert.Equal("Tester msg", tester1Cheeps[0].Cheep);
    }
}