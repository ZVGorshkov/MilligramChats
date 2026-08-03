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
using MilligramServer.Extensions;

namespace MilligramServer.Controllers.Api;

[Authorize]
[ApiController]
[Route("api/contacts")]
[Produces(MediaTypeNames.Application.Json)]
public class ContactsApiController : ControllerBase
{
    private readonly IApplicationContextFactory _applicationContextFactory;
    private readonly ILogger<ContactsApiController> _logger;

    public ContactsApiController(
        IApplicationContextFactory applicationContextFactory,
        ILogger<ContactsApiController> logger)
    {
        _applicationContextFactory = applicationContextFactory;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ContactDto[]> GetAll(
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        var contacts = await context.Contacts
            .AsNoTracking()
            .Include(contact => contact.AddedUser)
            .Where(contact => !contact.IsDeleted && contact.OwnerUserId.ToString() == userId)
            .Select(contact => contact.ToDto())
            .ToArrayAsync(cancellationToken);

        return contacts;
    }

    [HttpGet("{id:Guid}")]
    public async Task<ContactDto> Get(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var contact = await context.Contacts
            .AsNoTracking()
            .Include(contact => contact.AddedUser)
            .FirstOrDefaultAsync(contact => !contact.IsDeleted && contact.Id == id, cancellationToken);

        if (contact == null)
            throw new NotFoundException($"Не найден контакт с id = {id}");

        return contact.ToDto();
    }
    [HttpGet("{name:String}")]
    public async Task<ContactDto> Get(
        [FromRoute] String name,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var contact = await context.Contacts
            .AsNoTracking()
            .Include(contact => contact.AddedUser)
            .FirstOrDefaultAsync(contact => !contact.IsDeleted && contact.Name == name, cancellationToken);

        if (contact == null)
            throw new NotFoundException($"Не найден контакт с Именем = {name}");

        return contact.ToDto();
    }

    [HttpPost]
    public async Task<ContactDto> Create(
        [FromBody] CreateContactDto createContactDto,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        if (createContactDto.Name != null && createContactDto.Name.Length > Common.Constants.MaxContactNameLength)
            ModelState.AddModelError(nameof(createContactDto.Name), $"Имя не должно превышать {Common.Constants.MaxContactNameLength} символов.");

        if (!ModelState.IsValid)
            throw new BadRequestException(ModelState.JoinErrors());

        var userId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var ownerUser = await context.Users
            .FirstOrDefaultAsync(user => !user.IsDeleted && user.Id.ToString() == userId, cancellationToken);

        var addedUser = await context.Users
            .FirstOrDefaultAsync(user => !user.IsDeleted && user.Id == createContactDto.AddedUserId, cancellationToken);

        if (addedUser == null)
            throw new NotFoundException($"Не найден пользователь с id = {createContactDto.AddedUserId}");

        var contact = new Contact
        {
            Id = Guid.NewGuid(),
            Name = createContactDto.Name!,
            OwnerUserId = ownerUser!.Id,
            OwnerUser = ownerUser,
            AddedUserId = createContactDto.AddedUserId,
            AddedUser = addedUser
        };

        context.Contacts.Add(contact);
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Contact with id = {contact.Id} created");

        return contact.ToDto();
    }

    [HttpPut("{id:Guid}")]
    public async Task<ContactDto> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateContactDto updateContactDto,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var contact = await context.Contacts
            .Include(contact => contact.AddedUser)
            .FirstOrDefaultAsync(contact => !contact.IsDeleted && contact.Id == id, cancellationToken);

        if (contact == null)
            throw new NotFoundException($"Не найден контакт с id = {id}");

        if (updateContactDto.Name != null && updateContactDto.Name.Length > Common.Constants.MaxContactNameLength)
            ModelState.AddModelError(nameof(updateContactDto.Name), $"Имя не должно превышать {Common.Constants.MaxContactNameLength} символов.");

        if (!ModelState.IsValid)
            throw new BadRequestException(ModelState.JoinErrors());

        contact.Name = updateContactDto.Name;

        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Contact with id = {id} updated");

        return contact.ToDto();
    }

    [HttpDelete("{id:Guid}")]
    public async Task Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        await using var context = _applicationContextFactory.Create();

        var contacts = await context.Contacts
            .FirstOrDefaultAsync(chat => chat.Id == id, cancellationToken);

        if (contacts == null)
            throw new NotFoundException($"Не найден контакт с id = {id}");

        contacts.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation($"Contact with id = {id} deleted");
    }
}