namespace UnitTestsDB;

using Xunit;

public class UnitTest1
{
    public record Cheep(string Author, string Message, long Timestamp);
    
    [Fact]
    public void StoringCheepToDatabase()
    {
        
        //arrange
        var author = "User";
        var message = "Storage";
        var timestamp = 1234567890L;
        var cheep = new Cheep(author, message, timestamp);
        
        Cheep[] storage = { cheep };
        
        //act
        var savedCheep = storage[0];

         
            
        //assert
        Assert.Equal(author, savedCheep.Author);
        Assert.Equal(message, savedCheep.Message);
        Assert.Equal(timestamp, savedCheep.Timestamp);
    }
}
