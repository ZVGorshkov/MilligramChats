namespace MilligramClient.Common.Wpf.Messages
{
    public class UsernameChangeMessage
    {
		public string CurrentUsername { get; }
		public string NewUsername { get; }

		public UsernameChangeMessage(string currentUsername, string newUsername)
		{
			CurrentUsername = currentUsername;
			NewUsername = newUsername;
		}
    }
}
