using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Avalonia.Collections;
using Avalonia.Threading;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Services;
using Pheidippides.Models;
using Rooster.Contract.Models;
using Rooster.Contract.Services;

namespace Pheidippides.Services;

public interface IAlarmMemoryCache : IMemoryCache<RoosterPostRequest, RoosterGetResponse>
{
    IAvaloniaReadOnlyList<AlarmNotify> Alarms { get; }
}

public interface IAlarmUiCache : IUiCache<RoosterPostRequest, RoosterGetResponse, IAlarmMemoryCache>
{
    IAvaloniaReadOnlyList<AlarmNotify> Alarms { get; }
}

public sealed class AlarmUiCache
    : UiCache<RoosterPostRequest, RoosterGetResponse, IAlarmDbCache, IAlarmMemoryCache>,
        IAlarmUiCache
{
    public AlarmUiCache(IAlarmDbCache dbCache, IAlarmMemoryCache memoryCache)
        : base(dbCache, memoryCache) { }

    public IAvaloniaReadOnlyList<AlarmNotify> Alarms => MemoryCache.Alarms;
}

public sealed class AlarmMemoryCache
    : MemoryCache<AlarmNotify, RoosterPostRequest, RoosterGetResponse>,
        IAlarmMemoryCache
{
    public AlarmMemoryCache(IAlarmScheduler alarmScheduler)
    {
        _alarmScheduler = alarmScheduler;
        _alarms = new();
    }

    public IAvaloniaReadOnlyList<AlarmNotify> Alarms => _alarms;

    public override ConfiguredValueTaskAwaitable UpdateAsync(
        RoosterPostRequest source,
        CancellationToken ct
    )
    {
        Dispatcher.UIThread.Post(() =>
        {
            _alarms.AddRange(source.Creates.Select(Update).ToArray());

            foreach (var alarm in source.Edits)
            {
                foreach (var id in alarm.Ids)
                {
                    var item = GetItem(id);

                    if (alarm.IsEditDueDateTime)
                    {
                        item.DueDateTime = alarm.DueDateTime;
                    }

                    if (alarm.IsEditName)
                    {
                        item.Name = alarm.Name;
                    }
                }
            }

            _alarms.RemoveAll(_alarms.Where(a => source.DeleteIds.Contains(a.Id)));

            foreach (var id in source.DeleteIds)
            {
                Items.Remove(id);
            }

            _alarmScheduler.UpdateAlarms(_alarms.Where(x => !x.IsCompleted).ToArray());
        });

        return TaskHelper.ConfiguredCompletedTask;
    }

    public override ConfiguredValueTaskAwaitable UpdateAsync(
        RoosterGetResponse source,
        CancellationToken ct
    )
    {
        Dispatcher.UIThread.Post(() =>
        {
            _alarms.UpdateOrder(source.Alarms.Select(Update).OrderBy(x => x.DueDateTime).ToArray());
            _alarmScheduler.UpdateAlarms(_alarms.Where(x => !x.IsCompleted).ToArray());
        });

        return TaskHelper.ConfiguredCompletedTask;
    }

    private readonly AvaloniaList<AlarmNotify> _alarms;
    private readonly IAlarmScheduler _alarmScheduler;

    private AlarmNotify Update(Alarm alarm)
    {
        var item = GetItem(alarm.Id);
        item.DueDateTime = alarm.DueDateTime;
        item.Name = alarm.Name;

        return item;
    }
}
