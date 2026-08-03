using MilligramServer.Domain.Entities;
using MilligramServer.Models.Chats;

namespace MilligramServer.Extensions.Models;

public static class ChatExtensions
{
    public static IEnumerable<ChatModel> ToModels(
        this IEnumerable<Chat> chats)
    {
        foreach (var chat in chats)
            yield return chat.ToModel();
    }

    public static ChatModel ToModel(this Chat chat)
    {
        return new ChatModel
        {
            Id = chat.Id,
            Name = chat.Name,
            OwnerUserId = chat.OwnerUserId,
            OwnerUserName = chat.OwnerUser?.Name,
            IsDeleted = chat.IsDeleted
        };
    }
}