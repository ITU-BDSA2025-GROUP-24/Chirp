using Chirp.Infrastructure;

namespace Chirp.Tests;

public class ConvertToAuthorDTOTests
{
    [Fact]
    public void ToAuthorDTO_ShouldConvertCheepToDTO()
    {
        //Arrange
        var author = new Author { AuthorId = Guid.NewGuid(), Name = "Tester", Email = "tester@example.com"};
        
        //Act
        var dto = author.ToAuthorDTO();
        
        //Assert
        Assert.Equal(author.AuthorId, dto.AuthorId);
        Assert.Equal(author.Name, dto.Name);
        Assert.Equal(author.Email, dto.Email);
    }
}