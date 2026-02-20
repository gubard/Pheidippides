using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using Gaia.Helpers;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Pheidippides.Models;

namespace Pheidippides.Ui;

public sealed partial class AlarmListViewModel : ViewModelBase, IInit, ISaveUi
{
    public AlarmListViewModel(IAvaloniaReadOnlyList<AlarmNotify> alarms)
    {
        _alarms = alarms;
        _items = [];
    }

    public IAvaloniaReadOnlyList<AlarmNotify> Items => _items;

    [ObservableProperty]
    private AlarmsOrderBy _orderBy;

    private readonly IAvaloniaReadOnlyList<AlarmNotify> _alarms;
    private readonly AvaloniaList<AlarmNotify> _items;

    private void UpdateItems()
    {
        _items.UpdateOrder(
            OrderBy switch
            {
                AlarmsOrderBy.Name => _alarms
                    .OrderBy(x => x.Name)
                    .ThenBy(x => x.DueDateTime)
                    .ToArray(),
                AlarmsOrderBy.DueDate => _alarms
                    .OrderBy(x => x.DueDateTime)
                    .ThenBy(x => x.Name)
                    .ToArray(),
                _ => throw new ArgumentOutOfRangeException(),
            }
        );
    }

    public ConfiguredValueTaskAwaitable InitAsync(CancellationToken ct)
    {
        Dispatcher.UIThread.Post(UpdateItems);
        _alarms.CollectionChanged += AlarmsCollectionChanged;

        return TaskHelper.ConfiguredCompletedTask;
    }

    public ConfiguredValueTaskAwaitable SaveUiAsync(CancellationToken ct)
    {
        _alarms.CollectionChanged -= AlarmsCollectionChanged;

        return TaskHelper.ConfiguredCompletedTask;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        if (e.PropertyName == nameof(OrderBy))
        {
            UpdateItems();
        }
    }

    private void AlarmsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateItems();
    }
}

public enum AlarmsOrderBy
{
    Name,
    DueDate,
}
