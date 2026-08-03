using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MilligramServer;

public static class Constants
{
    public const string DescSuffix = "Desc";

    public const int FirstPage = 1;
    public const int PageSize = 5;

    public const string AllowedUserNameCharacters = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯabcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    public const string MultiAuthScheme = "MultiAuthScheme";

    public static readonly TimeSpan CookieLifetime = TimeSpan.FromMinutes(60);

    public const string JwtIssuer = "MilligramServer";
    public const string JwtAudience = "MilligramServerClient";
    public static readonly TimeSpan JwtLifetime = TimeSpan.FromMinutes(60);
    private const string JwtKey = "AnySecretKeyForMilligramServer!123456";

    public static SymmetricSecurityKey GetJwtSymmetricSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
    }
}