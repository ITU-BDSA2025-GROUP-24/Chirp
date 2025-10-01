namespace IntegrationTests;

using Xunit;
using SimpleDB;
using System.Linq;
using System.IO;
using Chirp.CLI;
using System.Globalization;


public class IntegrationTest
{ 
    [Fact]
    public void CheepStorageAndRetrieval() {
        
        //Arrange
        string testFilePath = "test_chirps.csv";
        
        IDatabaseRepository<Cheep> repo = new CsvDatabase<Cheep>(testFilePath);
        var cheep = new Cheep("Author", "Hellow", 12345);
        
        //Act
        repo.Store(cheep);
        
        var retrieved = repo.Read().FirstOrDefault(c => c.Author == "Author");
        
        
        //Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Author", retrieved.Author);
        Assert.Equal("Hellow", retrieved.Message);
        Assert.Equal(12345, retrieved.Timestamp);

        //Cleanup
        if (File.Exists(testFilePath)) File.Delete(testFilePath);
    }
}
