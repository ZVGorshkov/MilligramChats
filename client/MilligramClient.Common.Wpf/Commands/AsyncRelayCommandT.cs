using System.Windows.Input;
using MilligramClient.Common.Extensions;

namespace MilligramClient.Common.Wpf.Commands;

public class AsyncRelayCommand<T> : ICommand
{
	public event EventHandler? CanExecuteChanged
	{
		add => CommandManager.RequerySuggested += value;
		remove => CommandManager.RequerySuggested -= value;
	}

	private readonly Func<T?, Task> _execute;
	private readonly Func<T?, bool>? _canExecute;

	private long _isExecuting;

	public AsyncRelayCommand(Func<T?, Task> execute, Func<T?, bool>? canExecute = null)
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
		if (_canExecute == null)
			return true;

		if (Interlocked.Read(ref _isExecuting) != 0)
			return false;

		if (parameter == null)
			return _canExecute(default);

		if (parameter.GetType() != typeof(T) && parameter is IConvertible)
			return _canExecute((T) Convert.ChangeType(parameter, typeof(T), null));

		if (parameter is T t)
			return _canExecute(t);

		return false;
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
			if (parameter == null)
				await _execute(default).ConfigureAwait(false);
			else if (parameter.GetType() != typeof(T) && parameter is IConvertible)
				await _execute((T) Convert.ChangeType(parameter, typeof(T), null)).ConfigureAwait(false);
			else if (parameter is T t)
				await _execute(t).ConfigureAwait(false);
		}
		finally
		{
			Interlocked.Exchange(ref _isExecuting, 0);
			RaiseCanExecuteChanged();
		}
	}
}