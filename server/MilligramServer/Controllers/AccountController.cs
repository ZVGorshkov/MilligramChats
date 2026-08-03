using MilligramServer.Common.Extensions;
using MilligramServer.Models.Account;
using MilligramServer.Services.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MilligramServer.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly ApplicationContextSignInManager _applicationContextSignInManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(
        ApplicationContextSignInManager applicationContextSignInManager,
        ILogger<AccountController> logger)
    {
        _applicationContextSignInManager = applicationContextSignInManager;
        _logger = logger;
    }

    [AllowAnonymous]
    public IActionResult Login(
        [FromQuery] string? returnUrl = null)
    {
        return View(new LoginModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        [FromForm] LoginModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _applicationContextSignInManager.PasswordSignInAsync(model.Login, model.Password, false, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(nameof(model.Login), "Некорректные логин и(или) пароль");
            return View(model);
        }

        if (model.ReturnUrl.IsSignificant())
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await _applicationContextSignInManager.SignOutAsync();

        return RedirectToAction("Index", "Home");
    }

    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }
}