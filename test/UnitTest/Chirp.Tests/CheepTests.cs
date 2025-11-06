using Chirp.Infrastructure;

namespace Chirp.Tests;

public class CheepTests
{
    public record Cheep(int CheepId, int AuthorId, Author Author, string Text, DateTime TimeStamp);
    [Fact]
    public void Test1()
    {
        //Arrange
        var CheepId = 1234;
        var AuthorId = 9876;
        var author = new Author { AuthorId = AuthorId, Name = "Creator" };
        var Text = "Let there be text!";
        var TimeStamp = DateTime.Now;
        
        
        //Act
        var MyCheep = new Cheep(CheepId, AuthorId, author, Text, TimeStamp);
        
        
        //Assert
        Assert.Equal(CheepId, MyCheep.CheepId);
        Assert.Equal(AuthorId, MyCheep.AuthorId);
        Assert.Equal(author.Name, MyCheep.Author.Name);
        Assert.Equal(author.Email, MyCheep.Author.Email);
        Assert.Equal(Text, MyCheep.Text);
        Assert.Equal(TimeStamp, MyCheep.TimeStamp, TimeSpan.FromSeconds(1));
    }
}