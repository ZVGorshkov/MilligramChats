#pragma warning disable CS8618

using MilligramServer.Models.Users;
using System.ComponentModel;

namespace MilligramServer.Models.Chats;

public class ChatDetailsModel : SortingPaginationModelBase
{
    [DisplayName("Чат")]
    public ChatModel Chat { get; set; }

    [DisplayName("Участники")]
    public IReadOnlyCollection<UserModel> Users { get; set; }
}