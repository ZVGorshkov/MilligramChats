using GalaSoft.MvvmLight.Messaging;
using MilligramClient.Wpf.Views.Login.Controls.Login;
using MilligramClient.Wpf.Views.Login.Controls.Register;
using MilligramClient.Wpf.Views.Main.Logic;

namespace MilligramClient.Wpf.Views.Login;

public class DemoModel : LoginViewModel
{
	public DemoModel() : base(
		DemoLocator.Locate<IMessenger>(),
		DemoLocator.Locate<IMainWindowProvider>(),
		DemoLocator.Locate<LoginControlViewModel>(),
		DemoLocator.Locate<RegisterControlViewModel>())
	{
		LoginState = LoginState.Login;

	}
}