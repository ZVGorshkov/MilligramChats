namespace MilligramClient.Domain.Dtos;

public class ContactDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
	  public string? AddedUserNickname { get; set; }
	  public Guid AddedUserId { get; set; }
}