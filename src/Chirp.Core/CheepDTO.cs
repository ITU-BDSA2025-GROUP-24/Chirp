using System.ComponentModel.DataAnnotations;

namespace Chirp.Core;

public class CheepDTO
{
    /*
    CheepId - Guid used to make ID's unpredictable, 
    since they are not created by incrementing the previous ID
    */
    public Guid CheepId { get; set; }
   
    //Cheep author - AuthorDTO type. Required! 
    public required AuthorDTO Author { get; set; }
    
    //Cheep text, also required - You cannot make a cheep without the cheep
    public required string Cheep  { get; set; }
    
    //When the cheep was created - Required, used to sort cheeps chronologically 
    public required DateTime TimeStamp { get; set; }
}