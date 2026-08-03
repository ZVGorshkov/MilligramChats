using GalaSoft.MvvmLight;

namespace MilligramClient.Wpf.Models;

public class UserModel : ViewModelBase, ICloneable
{
	private string _username;
	private string _password;
	private bool _isRememberUser;
	public UserModel()
	{
		Username = string.Empty;
		Password = string.Empty;
		IsRememberUser = false;
	}

    public string Username
	{
		get => _username;
		set => Set(ref _username, value);
	}

	public string Password
	{
		get => _password;
		set => Set(ref _password, value);
	}

	public bool IsRememberUser
	{
		get => _isRememberUser;
		set => Set(ref _isRememberUser, value);
	}

	public object Clone()
	{
		var clone = (UserModel) MemberwiseClone();
		return clone;
	}
}