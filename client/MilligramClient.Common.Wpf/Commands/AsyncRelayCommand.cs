using System.Windows.Input;
using MilligramClient.Common.Extensions;

namespace MilligramClient.Common.Wpf.Commands;

public class AsyncRelayCommand : ICommand
{
	public event EventHandler? CanExecuteChanged
	{
		add => CommandManager.RequerySuggested += value;
		remove => CommandManager.RequerySuggested -= value;
	}

	private readonly Func<Task> _execute;
	private readonly Func<bool>? _canExecute;

	private long _isExecuting;

	public AsyncRelayCommand(Func<Task> execute, Func<bool>? canExecute = null)
	{
		_execute = execute ?? throw new ArgumentNullException(nameof(execute));
		_canExecute = canExecute;
	}

	public void RaiseCanExecuteChanged()
	{
		CommandManager.InvalidateRequerySuggested();
	}

	public bool CanExecute(object? parameter)
	{
		return _canExecute == null || (Interlocked.Read(ref _isExecuting) == 0 && _canExecute());
	}

	public void Execute(object? parameter)
	{
		ExecuteAsync(parameter).FireAndForgetSafeAsync();
	}

	private async Task ExecuteAsync(object? parameter)
	{
		if (!CanExecute(parameter))
			return;

		Interlocked.Exchange(ref _isExecuting, 1);
		RaiseCanExecuteChanged();

		try
		{
			await _execute().ConfigureAwait(false);
		}
		finally
		{
			Interlocked.Exchange(ref _isExecuting, 0);
			RaiseCanExecuteChanged();
		}
	}
}