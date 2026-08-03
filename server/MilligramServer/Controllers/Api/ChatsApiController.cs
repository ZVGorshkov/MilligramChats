using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilligramServer.Domain.Extensions;
using MilligramServer.Database.Context.Factory;
using MilligramServer.Domain.Dtos;
using System.Net.Mime;
using System.Security.Claims;
using MilligramServer.Exceptions.Api;
using MilligramServer.Domain.Entities;
using MilligramServer.Common.Extensions;
using MilligramServer.Extensions;
using File = MilligramServer.Domain.Entities.File;

namespace MilligramServer.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/chats")]
[Produces(MediaTypeNames.Application.Json)]
public class ChatsApiController : ControllerBase
{
    private readonly IApplicationContextFactory _applicationContextFactory;
    private readonly ILogger<ChatsApiController> _logger;

    public ChatsApiController(
        IApplicationContextFactory applicationContextFactory,
        ILogger<ChatsApiController> logger)
    {
        _applicationContextFactory = applicationContextFactory;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ChatDetailsDto[]> GetAll(
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var chats = await context.UsersChats
            .AsNoTracking()
            .Where(userChat => !userChat.Chat.IsDeleted && !userChat.User.IsDeleted && userChat.UserId.ToString() == userId)
            .Select(userChat => userChat.Chat)
            .OrderBy(chat => chat.Name)
            .Select(chat => chat.ToDetailsDto())
            .ToArrayAsync(cancellationToken);

        return chats;
    }

    [HttpGet("{id:Guid}")]
    public async Task<ChatDetailsDto> Get(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var chat = await context.Chats
            .AsNoTracking()
            .FirstOrDefaultAsync(chat => !chat.IsDeleted && chat.Id == id, cancellationToken);

        if (chat == null)
            throw new NotFoundException($"Не найден чат с id = {id}");

        var chatDetailsDto = new ChatDetailsDto
        {
            Id = chat.Id,
            Name = chat.Name,
            OwnerUserId = chat.OwnerUserId
        };

        return chatDetailsDto;
    }

    [HttpPost]
    public async Task<ChatDetailsDto> Create(
        [FromBody] ChatDto chatDto,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        if (chatDto.Name.IsNullOrEmpty())
            ModelState.AddModelError(nameof(chatDto.Name), "Название обязательно для заполнения.");

        if (chatDto.Name.Length > Common.Constants.MaxChatNameLength)
            ModelState.AddModelError(nameof(chatDto.Name), $"Название не должно превышать {Common.Constants.MaxChatNameLength} символов.");

        if (!ModelState.IsValid)
            throw new BadRequestException(ModelState.JoinErrors());

        var chat = new Chat
        {
            Id = Guid.NewGuid(),
            Name = chatDto.Name,
            OwnerUserId = chatDto.OwnerUserId
        };

        context.Chats.Add(chat);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Chat with id = {chat.Id} created");

        return chat.ToDetailsDto();
    }

    [HttpPut("{id:Guid}")]
    public async Task<ChatDetailsDto> Update(
        [FromRoute] Guid id,
        [FromBody] ChatDto chatDto,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var chat = await context.Chats
            .FirstOrDefaultAsync(chat => !chat.IsDeleted && chat.Id == id, cancellationToken);

        if (chat == null)
            throw new NotFoundException($"Не найден чат с id = {id}");

        if (chatDto.Name.IsNullOrEmpty())
            ModelState.AddModelError(nameof(chatDto.Name), "Название обязательно для заполнения.");

        if (chatDto.Name.Length > Common.Constants.MaxChatNameLength)
            ModelState.AddModelError(nameof(chatDto.Name), $"Название не должно превышать {Common.Constants.MaxChatNameLength} символов.");

        if (!ModelState.IsValid)
            throw new BadRequestException(ModelState.JoinErrors());

        chat.Name = chatDto.Name;
        if (chat.OwnerUserId != null)
            chat.OwnerUserId = chatDto.OwnerUserId;

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Chat with id = {id} updated");

        return chat.ToDetailsDto();
    }

    [HttpDelete("{id:Guid}")]
    public async Task Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var chat = await context.Chats
            .FirstOrDefaultAsync(chat => chat.Id == id, cancellationToken);

        if (chat == null)
            throw new NotFoundException($"Не найден чат с id = {id}");

        chat.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Chat with id = {id} deleted");
    }

    [HttpGet("{id:Guid}/users")]
    public async Task<UserDto[]> GetUsers(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var chatExists = await context.Chats
            .AsNoTracking()
            .AnyAsync(chat => !chat.IsDeleted && chat.Id == id, cancellationToken);

        if (!chatExists)
            throw new NotFoundException($"Не найден чат с id = {id}");

        var users = await context.UsersChats
            .AsNoTracking()
            .Where(userChat => !userChat.Chat.IsDeleted && !userChat.User.IsDeleted && userChat.ChatId == id)
            .Select(userChat => userChat.User)
            .OrderBy(user => user.Name)
            .Select(user => user.ToDto())
            .ToArrayAsync(cancellationToken);

        return users;
    }

    [HttpPost("{chatId:Guid}/users/{userId:Guid}")]
    public async Task<ChatDetailsDto> AddUser(
        [FromRoute] Guid chatId,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var chat = await context.Chats
            .FirstOrDefaultAsync(chat => !chat.IsDeleted && chat.Id == chatId, cancellationToken);

        if (chat == null)
            throw new NotFoundException($"Не найден чат с id = {chatId}");

        var userExists = await context.Users
            .AsNoTracking()
            .AnyAsync(user => !user.IsDeleted && user.Id == userId, cancellationToken);

        if (!userExists)
            throw new NotFoundException($"Не найден пользователь с id = {userId}");

        var isUserInChat = await context.UsersChats
            .AnyAsync(userChat => userChat.ChatId == chatId && userChat.UserId == userId, cancellationToken);

        if (isUserInChat)
            throw new BadRequestException($"Пользователь уже добавлен в этот чат");

        var userChat = new UserChat
        {
            UserId = userId,
            ChatId = chatId
        };

        context.UsersChats.Add(userChat);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"User with id = {userId} add to chat with id = {chatId}");

        return chat.ToDetailsDto();
    }

    [HttpDelete("{chatId:Guid}/users/{userId:Guid}")]
    public async Task<ChatDetailsDto> DeleteUser(
        [FromRoute] Guid chatId,
        [FromRoute] Guid userId,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var chat = await context.Chats
            .FirstOrDefaultAsync(chat => !chat.IsDeleted && chat.Id == chatId, cancellationToken);

        if (chat == null)
            throw new NotFoundException($"Не найден чат с id = {chatId}");

        var userExists = await context.Users
            .AsNoTracking()
            .AnyAsync(user => !user.IsDeleted && user.Id == userId, cancellationToken);

        if (!userExists)
            throw new NotFoundException($"Не найден пользователь с id = {userId}");

        var userChat = await context.UsersChats
            .FirstOrDefaultAsync(userChat => userChat.UserId == userId && userChat.ChatId == chatId, cancellationToken);

        if (userChat == null)
            throw new BadRequestException($"Пользователя нет в этом чат");

        context.UsersChats.Remove(userChat);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"User with id = {userId} deleted from chat with id = {chatId}");

        return chat.ToDetailsDto();
    }

    [HttpGet("{id:Guid}/messages")]
    public async Task<MessageDetailsDto[]> GetMessages(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var messagesQuery = context.Messages
            .AsNoTracking()
            .Include(message => message.File)
            .Include(message => message.User)
            .Where(message => message.ChatId == id);

        messagesQuery = messagesQuery.OrderByDescending(message => message.CreationTime);

        var messages = await messagesQuery
            .Select(message => message.ToDetailsDto())
            .ToArrayAsync(cancellationToken);

        return messages;
    }

    [HttpGet("{chatId:Guid}/messages/{messageId:Guid}")]
    public async Task<MessageDetailsDto> GetMessage(
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var chatExists = await context.Chats
            .AsNoTracking()
            .AnyAsync(chat => !chat.IsDeleted && chat.Id == chatId, cancellationToken);

        if (!chatExists)
            throw new NotFoundException($"Не найден чат с id = {chatId}");

        var message = await context.Messages
            .AsNoTracking()
            .Include(message => message.File)
            .Include(message => message.User)
            .FirstOrDefaultAsync(message => !message.IsDeleted && message.Id == messageId && message.ChatId == chatId, cancellationToken);

        if (message == null)
            throw new NotFoundException($"Не найдено сообщение с id = {messageId} в чате с id = {chatId}");

        return message.ToDetailsDto();
    }

    [HttpPost("{chatId:Guid}/messages")]
    public async Task<MessageDetailsDto> AddMessage(
        [FromRoute] Guid chatId,
        [FromBody] MessageDto messageDto,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await context.Users
            .FirstOrDefaultAsync(user => !user.IsDeleted && user.Id.ToString() == userId, cancellationToken);

        var chat = await context.Chats
            .FirstOrDefaultAsync(chat => !chat.IsDeleted && chat.Id == chatId, cancellationToken);

        if (chat == null)
            throw new NotFoundException($"Не найден чат с id = {chatId}");

        var file = new File();

        if (messageDto.FileDto != null)
        {
            file.Id = Guid.NewGuid();
            file.Content = messageDto.FileDto.Content;
            file.Name = messageDto.FileDto.Name;
            file.Extension = messageDto.FileDto.Extension;
            file.IsImage = messageDto.FileDto.IsImage;
            file.SizeBytes = messageDto.FileDto.SizeBytes;

            context.Files.Add(file);
        }

        var message = new Message
        {
            Id = Guid.NewGuid(),
            Text = messageDto.Text,
            FileId = file.Id == default ? null : file.Id, 
            CreationTime = DateTime.UtcNow,
            LastChangeTime = DateTime.UtcNow,
            ChatId = chatId,
            Chat = chat,
            UserId = user!.Id,
            User = user
        };

        context.Messages.Add(message);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Message with id = {message.Id} added to chat with id = {chatId}");

        return message.ToDetailsDto();
    }

    [HttpPut("{chatId:Guid}/messages/{messageId:Guid}")]
    public async Task<MessageDetailsDto> UpdateMessage(
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId,
        [FromBody] MessageDto messageDto,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var chatExists = await context.Chats
            .AsNoTracking()
            .AnyAsync(chat => !chat.IsDeleted && chat.Id == chatId, cancellationToken);

        if (!chatExists)
            throw new NotFoundException($"Не найден чат с id = {chatId}");

        if (messageDto.FileDto != null && messageDto.FileDto.SizeBytes > Common.Constants.MaxFileSizeBytes)
            ModelState.AddModelError(nameof(messageDto.FileDto), "Максимальный размер файла - 10Мб.");

        if (!ModelState.IsValid)
            throw new BadRequestException(ModelState.JoinErrors());

        var message = await context.Messages
            .Include(message => message.File)
            .Include(message => message.User)
            .FirstOrDefaultAsync(message => !message.IsDeleted && message.Id == messageId && message.ChatId == chatId, cancellationToken);

        if (message == null)
            throw new NotFoundException($"Не найдено сообщение с id = {messageId} в чате с id = {chatId}");

        message.Text = messageDto.Text;
        message.LastChangeTime = DateTime.UtcNow;
        if (messageDto.FileDto == null)
            message.File = null;

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Message with id = {messageId} updated");

        return message.ToDetailsDto();
    }

    [HttpDelete("{chatId:Guid}/messages/{messageId:Guid}")]
    public async Task DeleteMessage(
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var chatExists = await context.Chats
            .AsNoTracking()
            .AnyAsync(chat => !chat.IsDeleted && chat.Id == chatId, cancellationToken);

        if (!chatExists)
            throw new NotFoundException($"Не найден чат с id = {chatId}");

        var message = await context.Messages
            .FirstOrDefaultAsync(message => !message.IsDeleted && message.Id == messageId && message.ChatId == chatId, cancellationToken);

        if (message == null)
            throw new NotFoundException($"Не найдено сообщение с id = {messageId} в чате с id = {chatId}");

        message.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Message with id = {messageId} deleted");
    }
}