using Chirp.Infrastructure;

namespace Chirp.Tests;

public class ConvertToCheepDtoTests
{
    [Fact]
    public void ToCheepDTO_ShouldConvertCheepToDTO()
    {
        //Arrange
        var author = new Author { AuthorId = Guid.NewGuid(), Name = "Tester", Email = "tester@example.com"};
        var cheep = new Cheep
        {
            CheepId = Guid.NewGuid(),
            Author = author,
            Text = "Test, Test 123.",
            TimeStamp = new DateTime(98765431)
        };
        
        //Act
        var dto = cheep.ToCheepDTO();
        
        //Assert
        Assert.Equal(cheep.CheepId, dto.CheepId);
        Assert.Equal(cheep.Text, dto.Cheep);
        Assert.Equal(cheep.TimeStamp, dto.TimeStamp);
        Assert.Equal(cheep.Author.AuthorId, dto.Author.AuthorId);
        Assert.Equal(cheep.Author.Name, dto.Author.Name);
        Assert.Equal(cheep.Author.Email, dto.Author.Email);
    }
}