using MilligramClient.Domain.Dtos;
using MilligramClient.Domain.Model;

namespace MilligramClient.Domain.Extensions;

public static class ChatExtensions
{
    public static ChatDto ToDto (this ChatModel chat)
    {
        return new ChatDto
        {
            Name = chat.Name,
        };
    }
}
