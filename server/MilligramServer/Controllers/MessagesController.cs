using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MilligramServer.Database.Context.Factory;
using MilligramServer.Database.Context;
using MilligramServer.Domain;
using MilligramServer.Services.Managers;
using Microsoft.EntityFrameworkCore;
using MilligramServer.Extensions.Models;
using MilligramServer.Models.Messages;

namespace MilligramServer.Controllers;

[Authorize(Roles = RoleTokens.AdminRole)]
public class MessagesController : Controller
{
    private readonly ApplicationContextUserManager _applicationContextUserManager;
    private readonly ApplicationContext _context;
    private readonly ILogger<UsersController> _logger;

    public MessagesController(
        ApplicationContextUserManager applicationContextUserManager,
        IApplicationContextFactory context,
        ILogger<UsersController> logger)
    {
        _applicationContextUserManager = applicationContextUserManager;
        _context = context.Create();
        _logger = logger;
    }

    [Route("Users/{userId:guid}/Chats/{chatId:guid}/[controller]")]
    public async Task<IActionResult> Index(
        [FromRoute] Guid userId,
        [FromRoute] Guid chatId,
        [FromQuery] MessagesIndexModel? model,
        CancellationToken cancellationToken)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var chat = await _context.Chats.FirstOrDefaultAsync(chat => chat.Id == chatId);

        if (user == null || chat == null)
            return NotFound();

        var messagesQuery = _context.Messages
            .Include(message => message.Chat)
            .Include(message => message.User)
            .Where(message => message.ChatId == chatId)
            .AsNoTracking();

        var searchString = model?.SearchString;

        if (!string.IsNullOrEmpty(searchString))
        {
            messagesQuery = messagesQuery.Where(message =>
                EF.Functions.Like(message.Id.ToString(), $"%{searchString}%"));
        }

        messagesQuery = model?.SortBy switch
        {
            nameof(MessageModel.Id) => messagesQuery.OrderBy(message => message.Id),
            nameof(MessageModel.Id) + Constants.DescSuffix => messagesQuery.OrderByDescending(message => message.Id),
            nameof(MessageModel.CreationTime) => messagesQuery.OrderBy(message => message.CreationTime),
            nameof(MessageModel.CreationTime) + Constants.DescSuffix => messagesQuery.OrderByDescending(message => message.CreationTime),
            nameof(MessageModel.LastChangeTime) => messagesQuery.OrderBy(message => message.LastChangeTime),
            nameof(MessageModel.LastChangeTime) + Constants.DescSuffix => messagesQuery.OrderByDescending(message => message.LastChangeTime),
            nameof(MessageModel.UserName) => messagesQuery.OrderBy(message => message.User.Name),
            nameof(MessageModel.UserName) + Constants.DescSuffix => messagesQuery.OrderByDescending(message => message.User.Name),
            nameof(MessageModel.IsDeleted) => messagesQuery.OrderBy(message => message.IsDeleted),
            nameof(MessageModel.IsDeleted) + Constants.DescSuffix => messagesQuery.OrderByDescending(message => message.IsDeleted),
            _ => messagesQuery.OrderBy(message => message.Id)
        };

        var page = Math.Max(Constants.FirstPage, model?.Page ?? Constants.FirstPage);
        var totalCount = await messagesQuery.CountAsync(cancellationToken);
        var messages = await messagesQuery
            .Skip((page - Constants.FirstPage) * Constants.PageSize)
            .Take(Constants.PageSize)
            .ToListAsync(cancellationToken);

        return View(new MessagesIndexModel
        {
            ChatId = chatId,
            Chat = chat.ToModel(),
            User = await user.ToModelAsync(_applicationContextUserManager),
            UserId = user.Id,
            Messages = messages.ToModels().ToArray(),
            SortBy = model?.SortBy,
            Page = page,
            TotalCount = totalCount
        });
    }

    [HttpGet("Users/{userId:guid}/Chats/{chatId:guid}/[controller]/{messageId:guid}")]
    public async Task<IActionResult> Details(
        [FromRoute] Guid userId,
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var chat = await _context.Chats.FirstOrDefaultAsync(chat => chat.Id == chatId);

        var message = await _context.Messages
            .Include(message => message.Chat)
            .Include(message => message.User)
            .Include(message => message.File)
            .FirstOrDefaultAsync(message => message.Id == messageId);

        if (user == null || chat == null || message == null)
            return NotFound();

        return View(message.ToModel());
    }

    [HttpPost("Users/{userId:guid}/Chats/{chatId:guid}/[controller]/[action]/{messageId:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid userId,
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var chat = await _context.Chats.FirstOrDefaultAsync(chat => chat.Id == chatId);
        var message = await _context.Messages.FirstOrDefaultAsync(message => message.Id == messageId);

        if (user == null || chat == null || message == null)
            return NotFound();

        message.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Messages", new { userId, chatId });
    }

    [HttpPost("Users/{userId:guid}/Chats/{chatId:guid}/[controller]/[action]/{messageId:guid}")]
    public async Task<IActionResult> Restore(
        [FromRoute] Guid userId,
        [FromRoute] Guid chatId,
        [FromRoute] Guid messageId)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var chat = await _context.Chats.FirstOrDefaultAsync(chat => chat.Id == chatId);
        var message = await _context.Messages.FirstOrDefaultAsync(message => message.Id == messageId);

        if (user == null || chat == null || message == null)
            return NotFound();

        message.IsDeleted = false;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Messages", new { userId, chatId });
    }

    [HttpGet("Files/[action]/{fileId:guid}")]
    public async Task<IActionResult> DownloadFile(
        [FromRoute] Guid fileId)
    {
        var file = await _context.Files
            .FirstOrDefaultAsync(file => file.Id == fileId);

        if (file == null)
            return NotFound();

        return File(file.Content, GetMimeType(file.Extension), $"{file.Name}{file.Extension}");
    }

    private static string GetMimeType(string extension)
    {
        return extension.ToLower() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".pdf" => "application/pdf",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
    }
}