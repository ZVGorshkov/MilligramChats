namespace MilligramClient.Common;

public class ExecutionTracker
{
    private readonly Action? _onSuspend;
    private readonly Action? _onResume;
    private readonly CancellationTokenSource _cancellationSource;

    private long _nestingLevel;

    public bool IsExecuted => Interlocked.Read(ref _nestingLevel) != 0;

    public ExecutionTracker()
        : this(null, null)
    {
    }

    public ExecutionTracker(Action? onSuspend, Action? onResume)
    {
        _onSuspend = onSuspend;
        _onResume = onResume;

        _cancellationSource = new CancellationTokenSource();
    }

    public ExecutionSuspenderContext TrackExecution()
    {
        return new ExecutionSuspenderContext(this, _cancellationSource.Token);
    }

    public class ExecutionSuspenderContext : IDisposable
    {
        private readonly ExecutionTracker _executionTracker;
        private readonly CancellationToken _cancellationToken;

        public ExecutionSuspenderContext(ExecutionTracker executionTracker, CancellationToken cancellationToken)
        {
            _executionTracker = executionTracker;
            _cancellationToken = cancellationToken;

            if (_cancellationToken.IsCancellationRequested)
                return;

            if (Interlocked.Increment(ref _executionTracker._nestingLevel) == 1)
                _executionTracker._onSuspend?.Invoke();
        }

        public void Dispose()
        {
            if (_cancellationToken.IsCancellationRequested)
                return;

            if (Interlocked.Decrement(ref _executionTracker._nestingLevel) == 0)
                _executionTracker._onResume?.Invoke();
        }
    }
}