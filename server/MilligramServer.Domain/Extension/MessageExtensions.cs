using MilligramServer.Domain.Dtos;
using MilligramServer.Domain.Entities;

namespace MilligramServer.Domain.Extensions;

public static class MessageExtensions
{
    public static MessageDetailsDto ToDetailsDto(this Message message)
    {
        return new MessageDetailsDto
        {
            Id = message.Id,
            Text = message.Text,
            FileDetailsDto = message.File?.ToDetailsDto(),
            CreationTime = message.CreationTime,
            LastChangeTime = message.LastChangeTime,
            UserId = message.UserId,
            UserNickname = message.User.Nickname,
            IsDeleted = message.IsDeleted
        };
    }
}