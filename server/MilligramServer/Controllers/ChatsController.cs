using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilligramServer.Database.Context;
using MilligramServer.Database.Context.Factory;
using MilligramServer.Domain;
using MilligramServer.Extensions.Models;
using MilligramServer.Models.Chats;
using MilligramServer.Models.Users;
using MilligramServer.Services.Managers;

namespace MilligramServer.Controllers;

[Authorize(Roles = RoleTokens.AdminRole)]
public class ChatsController : Controller
{
    private readonly ApplicationContextUserManager _applicationContextUserManager;
    private readonly ApplicationContext _context;
    private readonly ILogger<UsersController> _logger;

    public ChatsController(
        ApplicationContextUserManager applicationContextUserManager,
        IApplicationContextFactory context,
        ILogger<UsersController> logger)
    {
        _applicationContextUserManager = applicationContextUserManager;
        _context = context.Create();
        _logger = logger;
    }

    [Route("Users/{userId:guid}/[controller]")]
    public async Task<IActionResult> Index(
        [FromRoute] Guid userId,
        [FromQuery] ChatsIndexModel? model,
        CancellationToken cancellationToken)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return NotFound();

        var chatsQuery = _context.Chats
            .Where(chat => chat.UsersChats
                .Any(userChat => userChat.UserId == userId))
            .Include(chat => chat.OwnerUser)
            .AsNoTracking();

        var searchString = model?.SearchString;

        if (!string.IsNullOrEmpty(searchString))
        {
            chatsQuery = chatsQuery.Where(chat =>
                EF.Functions.Like(chat.Id.ToString(), $"%{searchString}%") ||
                EF.Functions.Like(chat.Name, $"%{searchString}%"));
        }

        chatsQuery = model?.SortBy switch
        {
            nameof(ChatModel.Id) => chatsQuery.OrderBy(chat => chat.Id),
            nameof(ChatModel.Id) + Constants.DescSuffix => chatsQuery.OrderByDescending(chat => chat.Id),
            nameof(ChatModel.Name) => chatsQuery.OrderBy(chat => chat.Name),
            nameof(ChatModel.Name) + Constants.DescSuffix => chatsQuery.OrderByDescending(chat => chat.Name),
            nameof(ChatModel.OwnerUserName) => chatsQuery.OrderBy(chat => chat.OwnerUser == null ? "" : chat.OwnerUser.Name),
            nameof(ChatModel.OwnerUserName) + Constants.DescSuffix => chatsQuery.OrderByDescending(chat => chat.OwnerUser == null ? "" : chat.OwnerUser.Name),
            nameof(ChatModel.IsDeleted) => chatsQuery.OrderBy(chat => chat.IsDeleted),
            nameof(ChatModel.IsDeleted) + Constants.DescSuffix => chatsQuery.OrderByDescending(chat => chat.IsDeleted),
            _ => chatsQuery.OrderBy(chat => chat.Id)
        };

        var page = Math.Max(Constants.FirstPage, model?.Page ?? Constants.FirstPage);
        var totalCount = await chatsQuery.CountAsync(cancellationToken);
        var chats = await chatsQuery
            .Skip((page - Constants.FirstPage) * Constants.PageSize)
            .Take(Constants.PageSize)
            .ToListAsync(cancellationToken);

        return View(new ChatsIndexModel
        {
            User = await user.ToModelAsync(_applicationContextUserManager),
            UserId = user.Id,
            Chats = chats.ToModels().ToArray(),
            SortBy = model?.SortBy,
            Page = page,
            TotalCount = totalCount
        });
    }

    [HttpGet("Users/{userId:guid}/[controller]/{chatId:guid}")]
    public async Task<IActionResult> Details(
        [FromRoute] Guid userId,
        [FromRoute] Guid chatId,
        [FromQuery] ChatDetailsModel model,
        CancellationToken cancellationToken)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var chat = await _context.Chats
            .Include(chat => chat.OwnerUser)
            .AsNoTracking()
            .FirstOrDefaultAsync(chat => chat.Id == chatId);

        if (user == null || chat == null)
            return NotFound();

        var usersQuery = _context.Users
            .Where(chat => chat.UsersChats
                .Any(userChat => userChat.ChatId == chatId))           
            .Include(chat => chat.UsersChats)
            .ThenInclude(userChat => userChat.Chat)
            .AsNoTracking();

        usersQuery = model?.SortBy switch
        {
            nameof(UserModel.Id) => usersQuery.OrderBy(user => user.Id),
            nameof(UserModel.Id) + Constants.DescSuffix => usersQuery.OrderByDescending(user => user.Id),
            nameof(UserModel.Name) => usersQuery.OrderBy(user => user.Name),
            nameof(UserModel.Name) + Constants.DescSuffix => usersQuery.OrderByDescending(user => user.Name),
            _ => usersQuery.OrderBy(user => user.Id)
        };

        var page = Math.Max(Constants.FirstPage, model?.Page ?? Constants.FirstPage);
        var totalCount = await usersQuery.CountAsync(cancellationToken);
        var users = await usersQuery
            .Skip((page - Constants.FirstPage) * Constants.PageSize)
            .Take(Constants.PageSize)
            .ToListAsync(cancellationToken);

        return View(new ChatDetailsModel
        {
            Chat = chat.ToModel(),
            Users = await users.ToModelsAsync(_applicationContextUserManager).ToArrayAsync(cancellationToken),
            SortBy = model?.SortBy,
            Page = page,
            TotalCount = totalCount
        });
    }

    [HttpGet("Users/{userId:guid}/[controller]/[action]/{chatId:guid}")]
    public async Task<IActionResult> Edit(
        [FromRoute] Guid userId,
        [FromRoute] Guid chatId)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var chat = await _context.Chats.AsNoTracking().FirstOrDefaultAsync(chat => chat.Id == chatId);

        if (user == null || chat == null)
            return NotFound();

        return View(new ChatUserModel
        {
            UserId = userId,
            ChatId = chatId,
            ChatName = chat.Name,
            Chat = chat.ToModel()
        });
    }

    [HttpPost("Users/{userId:guid}/[controller]/[action]/{chatId:guid}")]
    public async Task<IActionResult> Edit(
        [FromRoute] Guid userId,
        [FromRoute] Guid chatId,
        [FromForm] ChatUserModel model)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var chat = await _context.Chats.FirstOrDefaultAsync(chat => chat.Id == chatId);

        if (!ModelState.IsValid)
        {
            if (chat != null) 
                model.Chat = chat.ToModel();
            return View(model);
        }

        if (user == null || chat == null) 
            return NotFound();

        chat.Name = model.ChatName;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Chats", new { userId });
    }

    [HttpPost("Users/{userId:guid}/[controller]/[action]/{chatId:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid userId,
        [FromRoute] Guid chatId)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var chat = await _context.Chats.FirstOrDefaultAsync(chat => chat.Id == chatId);

        if (user == null || chat == null)
            return NotFound();

        chat.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Chats", new { userId });
    }

    [HttpPost("Users/{userId:guid}/[controller]/[action]/{chatId:guid}")]
    public async Task<IActionResult> Restore(
        [FromRoute] Guid userId,
        [FromRoute] Guid chatId)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var chat = await _context.Chats.FirstOrDefaultAsync(chat => chat.Id == chatId);

        if (user == null || chat == null)
            return NotFound();

        chat.IsDeleted = false;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Chats", new { userId });
    }
}