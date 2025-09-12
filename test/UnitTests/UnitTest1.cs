namespace UnitTests;

public class UnitTest1
{
    public record Cheep(string Author, string Message, long Timestamp);
    [Fact]
    public void Test1()
    {
        var Author = "Chris";
        var Message = "Hey There";
        var TimeStamp = 2242298352;
        
        var MyCheep = new Cheep(Author, Message, TimeStamp);
        Assert.Equal(Author, MyCheep.Author);
        Assert.Equal(Message, MyCheep.Message);
        Assert.Equal(TimeStamp, MyCheep.Timestamp);
    }
}
