namespace MilligramClient.Domain.Dtos;

public class CreateContactDto
{
	public string? Name { get; set; }
	public Guid AddedUserId { get; set; }
}
