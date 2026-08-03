using GalaSoft.MvvmLight.Messaging;
using MilligramClient.Api.Clients.Account;
using MilligramClient.Api.Token;
using MilligramClient.Common.Wpf.MessageBox;
using MilligramClient.Services.Token;

namespace MilligramClient.Wpf.Views.Login.Controls.Register;

public class DemoModel : RegisterControlViewModel
{
    public DemoModel() : base(
        DemoLocator.Locate<IMessenger>(),
        DemoLocator.Locate<ITokenStorage>(),
        DemoLocator.Locate<IAccountClient>(),
        DemoLocator.Locate<ITokenProvider>(),
        DemoLocator.Locate<IMessageBoxService>())
    {
    }
}