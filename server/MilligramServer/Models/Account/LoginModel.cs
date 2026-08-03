#pragma warning disable CS8618

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MilligramServer.Models.Account;

public class LoginModel
{
    [DisplayName("Логин")]
    [Required(ErrorMessage = "Не указан логин")]
    public string Login { get; set; }

    [DisplayName("Пароль")]
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "Не указан пароль")]
    public string Password { get; set; }

    public string? ReturnUrl { get; set; }
}