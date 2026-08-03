using GalaSoft.MvvmLight.Messaging;
using MilligramClient.Api.Token;
using MilligramClient.Common.Wpf.MessageBox;
using MilligramClient.Services.Token;

namespace MilligramClient.Wpf.Views.Login.Controls.Login;

public class DemoModel : LoginControlViewModel
{
    public DemoModel() : base(
        DemoLocator.Locate<IMessenger>(),
        DemoLocator.Locate<ITokenStorage>(),
        DemoLocator.Locate<ITokenProvider>(),
        DemoLocator.Locate<IMessageBoxService>())
    {
	}
}