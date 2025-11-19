using Chirp.Infrastructure;
    
namespace Chirp.IntegrationTests;

public class UnitTest1
{
    public record Cheep(Guid CheepId, Guid AuthorId, Author Author, string Text, DateTime TimeStamp);
    
    [Fact]
    public void Test1()
    {
        // Arrange
        var CheepId = Guid.NewGuid();
        var AuthorId = Guid.NewGuid();
        var author = new Author { AuthorId = AuthorId, Name = Author.Name, Email = Author.Email };
        var Text = "Kill meh";
        var TimeStamp = DateTime.Now;
        
        // Act

        // Assert
    }
}