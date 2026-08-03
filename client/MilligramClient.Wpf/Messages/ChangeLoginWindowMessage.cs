using MilligramClient.Wpf.Views.Login;

namespace MilligramClient.Wpf.Messages;

public class ChangeLoginWindowMessage
{
    public LoginState LoginState { get; }

    public ChangeLoginWindowMessage(LoginState loginState)
    {
        LoginState = loginState;
    }
}