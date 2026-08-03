using System.Diagnostics;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Grace.DependencyInjection;
using MilligramClient.Common.Wpf.Base;
using MilligramClient.Common.Wpf.Messages;

namespace MilligramClient.Common.Wpf.View;

public class ViewService : IViewService
{
    private readonly ILocatorService _locatorService;
    private readonly List<Window> _openedWindows;

    public ViewService(IMessenger messenger, ILocatorService locatorService)
    {
        _locatorService = locatorService;
        _openedWindows = new List<Window>();

        // Listen for the close event
        messenger.Register<RequestCloseMessage>(this, OnRequestClose);
    }

    [DebuggerStepThrough]
    public void OpenWindow<TViewModel>() where TViewModel : IViewModel
    {
        // Create window for that view tabModel.
        var window = CreateWindow<TViewModel>(WindowMode.Window);

        // Open the window.
        window.Show();
    }

    [DebuggerStepThrough]
    public void OpenWindow(IViewModel viewModel)
    {
        // Create window for that view tabModel.
        var window = CreateWindow(viewModel, WindowMode.Window);

        // Open the window.
        window.Show();
    }

    [DebuggerStepThrough]
    public bool? OpenDialog<TViewModel>() where TViewModel : IViewModel
    {
        // Create window for that viewModel.
        var window = CreateWindow<TViewModel>(WindowMode.Dialog);

        // Open the window and return the result.
        return window.ShowDialog();
    }

    [DebuggerStepThrough]
    public bool? OpenDialog(IViewModel viewModel)
    {
        // Create window for that viewModel.
        var window = CreateWindow(viewModel, WindowMode.Dialog);

        // Open the window and return the result.
        return window.ShowDialog();
    }

    [DebuggerStepThrough]
    public Window CreateWindow<TViewModel>(WindowMode windowMode) where TViewModel : IViewModel
    {
        var viewModel = _locatorService.Locate<TViewModel>();

        return CreateWindow(viewModel, windowMode);
    }

    [DebuggerStepThrough]
    public Window CreateWindow(IViewModel viewModel, WindowMode windowMode)
    {
        var window = viewModel.View as Window;
        ArgumentNullException.ThrowIfNull(window);

        window.DataContext = viewModel;
        window.Closed += OnClosed;

        lock (_openedWindows)
        {
            // Last window opened is considered the 'owner' of the window.
            // May not be 100% correct in some situations but it is more
            // then good enough for handling dialog windows
            if (windowMode == WindowMode.Dialog && _openedWindows.Any())
            {
                var lastOpened = _openedWindows.Last();
                if (lastOpened.IsActive && !Equals(window, lastOpened))
                    window.Owner = lastOpened;
            }

            _openedWindows.Add(window);
        }

        return window;
    }

    public int GetOpenedWindowsCount()
    {
        lock (_openedWindows)
            return _openedWindows.Count;
    }

    private void OnRequestClose(RequestCloseMessage message)
    {
        var window = _openedWindows.SingleOrDefault(w => w.DataContext == message.ViewModel);
        if (window == null)
            return;

        if (message.DialogResult != null)
            window.DialogResult = message.DialogResult;
        else
            window.Close();
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        var window = sender as Window;
        ArgumentNullException.ThrowIfNull(window);

        window.Closed -= OnClosed;

        lock (_openedWindows)
            _openedWindows.Remove(window);
    }
}