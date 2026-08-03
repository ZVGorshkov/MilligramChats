using MilligramServer.Domain.Dtos;
using MilligramServer.Domain.Entities;

namespace MilligramServer.Domain.Extensions;

public static class UserExtensions
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            Nickname = user.Nickname
        };
    }
}