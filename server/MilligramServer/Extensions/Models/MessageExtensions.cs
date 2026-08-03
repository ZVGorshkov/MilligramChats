using MilligramServer.Domain.Entities;
using MilligramServer.Models.Messages;

namespace MilligramServer.Extensions.Models;

public static class MessageExtensions
{
    public static IEnumerable<MessageModel> ToModels(
        this IEnumerable<Message> messages)
    {
        foreach (var message in messages)
            yield return message.ToModel();
    }

    public static MessageModel ToModel(this Message message)
    {
        return new MessageModel
        {
            Id = message.Id,
            Text = message.Text,
            FileId = message.FileId,
            File = message.File?.ToModel(),
            CreationTime = message.CreationTime,
            LastChangeTime = message.LastChangeTime,
            ChatId = message.ChatId,
            ChatName = message.Chat.Name,
            UserId = message.UserId,
            UserName = message.User.Name,
            IsDeleted = message.IsDeleted
        };
    }
}