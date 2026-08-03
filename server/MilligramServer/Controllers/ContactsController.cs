using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MilligramServer.Database.Context;
using MilligramServer.Database.Context.Factory;
using MilligramServer.Domain;
using MilligramServer.Extensions.Models;
using MilligramServer.Models.Contacts;
using MilligramServer.Services.Managers;

namespace MilligramServer.Controllers;

[Authorize(Roles = RoleTokens.AdminRole)]
public class ContactsController : Controller
{
    private readonly ApplicationContextUserManager _applicationContextUserManager;
    private readonly ApplicationContext _context;
    private readonly ILogger<UsersController> _logger;

    public ContactsController(
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
        [FromQuery] ContactsIndexModel? model,
        CancellationToken cancellationToken)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        if (user == null)
            return NotFound();

        var contactsQuery = _context.Contacts
            .Where(chat => chat.OwnerUserId == userId)
            .Include(chat => chat.OwnerUser)
            .Include(chat => chat.AddedUser)
            .AsNoTracking();

        var searchString = model?.SearchString;

        if (!string.IsNullOrEmpty(searchString))
        {
            contactsQuery = contactsQuery.Where(contact =>
                EF.Functions.Like(contact.Id.ToString(), $"%{searchString}%") ||
                EF.Functions.Like(contact.AddedUserId.ToString(), $"%{searchString}%") ||
                EF.Functions.Like(contact.AddedUser.Name.ToString(), $"%{searchString}%") ||
                EF.Functions.Like(contact.Name == null ? "" : contact.Name.ToString(), $"%{searchString}%"));
        }

        contactsQuery = model?.SortBy switch
        {
            nameof(ContactModel.Id) => contactsQuery.OrderBy(contact => contact.Id),
            nameof(ContactModel.Id) + Constants.DescSuffix => contactsQuery.OrderByDescending(contact => contact.Id),
            nameof(ContactModel.AddedUserId) => contactsQuery.OrderBy(contact => contact.AddedUserId),
            nameof(ContactModel.AddedUserId) + Constants.DescSuffix => contactsQuery.OrderByDescending(contact => contact.AddedUserId),
            nameof(ContactModel.AddedUserName) => contactsQuery.OrderBy(contact => contact.AddedUser.Name),
            nameof(ContactModel.AddedUserName) + Constants.DescSuffix => contactsQuery.OrderByDescending(contact => contact.AddedUser.Name),
            nameof(ContactModel.Name) => contactsQuery.OrderBy(contact => contact.Name),
            nameof(ContactModel.Name) + Constants.DescSuffix => contactsQuery.OrderByDescending(contact => contact.Name),
            nameof(ContactModel.IsDeleted) => contactsQuery.OrderBy(contact => contact.IsDeleted),
            nameof(ContactModel.IsDeleted) + Constants.DescSuffix => contactsQuery.OrderByDescending(contact => contact.IsDeleted),
            _ => contactsQuery.OrderBy(contact => contact.Id)
        };

        var page = Math.Max(Constants.FirstPage, model?.Page ?? Constants.FirstPage);
        var totalCount = await contactsQuery.CountAsync(cancellationToken);
        var contacts = await contactsQuery
            .Skip((page - Constants.FirstPage) * Constants.PageSize)
            .Take(Constants.PageSize)
            .ToListAsync(cancellationToken);

        return View(new ContactsIndexModel
        {
            User = await user.ToModelAsync(_applicationContextUserManager),
            UserId = user.Id,
            Contacts = contacts.ToModels().ToArray(),
            SortBy = model?.SortBy,
            Page = page,
            TotalCount = totalCount
        });
    }

    [HttpGet("Users/{userId:guid}/[controller]/[action]/{contactId:guid}")]
    public async Task<IActionResult> Edit(
        [FromRoute] Guid userId,
        [FromRoute] Guid contactId)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var contact = await _context.Contacts
            .Include(contact => contact.AddedUser)
            .Include(contact => contact.OwnerUser)
            .FirstOrDefaultAsync(contact => contact.Id == contactId);

        if (user == null || contact == null)
            return NotFound();

        return View(contact.ToModel());
    }

    [HttpPost("Users/{userId:guid}/[controller]/[action]/{contactId:guid}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        [FromRoute] Guid userId,
        [FromRoute] Guid contactId,
        [FromForm] ContactModel model,
        CancellationToken cancellationToken)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var contact = await _context.Contacts.FirstOrDefaultAsync(contact => contact.Id == contactId);

        if (!ModelState.IsValid)
            return View(model);

        if (user == null || contact == null)
            return NotFound();

        contact.Name = model.Name;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Contacts", new { userId });
    }

    [HttpPost("Users/{userId:guid}/[controller]/[action]/{contactId:guid}")]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid userId,
        [FromRoute] Guid contactId)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var contact = await _context.Contacts.FirstOrDefaultAsync(contact => contact.Id == contactId);

        if (user == null || contact == null)
            return NotFound();

        contact.IsDeleted = true;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Contacts", new { userId });
    }

    [HttpPost("Users/{userId:guid}/[controller]/[action]/{contactId:guid}")]
    public async Task<IActionResult> Restore(
    [FromRoute] Guid userId,
    [FromRoute] Guid contactId)
    {
        var user = await _applicationContextUserManager.FindByIdAsync(userId.ToString());
        var contact = await _context.Contacts.FirstOrDefaultAsync(contact => contact.Id == contactId);

        if (user == null || contact == null)
            return NotFound();

        contact.IsDeleted = false;
        await _context.SaveChangesAsync();

        return RedirectToAction("Index", "Contacts", new { userId });
    }
}