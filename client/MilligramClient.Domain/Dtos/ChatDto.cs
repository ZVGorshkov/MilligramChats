namespace MilligramClient.Domain.Dtos
{
    public class ChatDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid OwnerUserId { get; set; }
    }
}
