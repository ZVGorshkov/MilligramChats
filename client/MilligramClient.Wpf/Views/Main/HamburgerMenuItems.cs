using System.Collections.ObjectModel;
using System.Windows;
using MahApps.Metro.IconPacks;

namespace MilligramClient.Wpf.Views.Main;

public class HamburgerMenuItems
{
	public ObservableCollection<HamburgerMenuItem> MenuItems { get; } = new()
	{
        new HamburgerMenuItem { Label = "Contacts", Icon = PackIconIoniconsKind.ContactsMD, Tag = "contacts", IsVisible = Visibility.Visible },
        new HamburgerMenuItem { Label = "Chats", Icon = PackIconIoniconsKind.ChatboxesMD, Tag = "chats", IsVisible = Visibility.Visible },
        new HamburgerMenuItem { Label = "Settings", Icon = PackIconIoniconsKind.SettingsMD, Tag = "settings", IsVisible = Visibility.Visible },
        new HamburgerMenuItem { Label = "New contact", Icon = PackIconIoniconsKind.PersonAddMD, Tag = "newContact", IsVisible = Visibility.Collapsed },
        new HamburgerMenuItem { Label = "Delete contact", Icon = PackIconIoniconsKind.TrashMD, Tag = "deleteContact", IsVisible = Visibility.Collapsed },
        new HamburgerMenuItem { Label = "Find chat", Icon = PackIconIoniconsKind.SearchMD, Tag = "findChat", IsVisible = Visibility.Collapsed },
        new HamburgerMenuItem { Label = "New chat", Icon = PackIconIoniconsKind.AddCircleMD, Tag = "newChat", IsVisible = Visibility.Collapsed },
        new HamburgerMenuItem { Label = "New private chat", Icon = PackIconIoniconsKind.LockMD, Tag = "newPrivateChat", IsVisible = Visibility.Collapsed },
        new HamburgerMenuItem { Label = "Delete chat", Icon = PackIconIoniconsKind.TrashMD, Tag = "deleteChat", IsVisible = Visibility.Collapsed },
        new HamburgerMenuItem { Label = "Back", Icon = PackIconIoniconsKind.ArrowBackMD, Tag = "back", IsVisible = Visibility.Collapsed }
    };

	public ObservableCollection<HamburgerMenuItem> MenuOptionsItems { get; } = new()
	{
		new HamburgerMenuItem { Label = "Log out", Icon = PackIconIoniconsKind.LogOutMD, Tag = "logOut", IsVisible = Visibility.Visible },
		new HamburgerMenuItem { Label = "Exit", Icon = PackIconIoniconsKind.ExitMD, Tag = "exit", IsVisible = Visibility.Visible }
    };
}