namespace MilligramClient.Domain.Model;

public class MessageModel
{
	public Guid Id { get; set; }
	public string Sender { get; set; }
	public string Text { get; set; }
	public DateTime Timestamp { get; set; }
	public DateTime LastChangeTime { get; set; }
	public bool IsDeleted { get; set; }
}