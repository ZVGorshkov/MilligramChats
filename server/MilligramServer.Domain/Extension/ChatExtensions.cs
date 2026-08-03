using MilligramServer.Domain.Dtos;
using MilligramServer.Domain.Entities;

namespace MilligramServer.Domain.Extensions;

public static class ChatExtensions
{
    public static ChatDetailsDto ToDetailsDto(this Chat chat)
    {
        return new ChatDetailsDto
        {
            Id = chat.Id,
            Name = chat.Name,
            OwnerUserId = chat.OwnerUserId
        };
    }
}