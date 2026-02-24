using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform;
using Gaia.Services;
using Pheidippides.Models;

namespace Pheidippides.Services;

public interface IAlarmScheduler
{
    void UpdateAlarms(params Span<AlarmSchedulerItem> items);
}

public sealed class EmptyAlarmScheduler : IAlarmScheduler
{
    public void UpdateAlarms(params Span<AlarmSchedulerItem> items) { }
}

public sealed class DefaultAlarmScheduler : IAlarmScheduler, IDisposable
{
    public DefaultAlarmScheduler(ISoundPlayer soundPlayer)
    {
        _soundPlayer = soundPlayer;
    }

    public void UpdateAlarms(params Span<AlarmSchedulerItem> items)
    {
        _cts.Cancel();
        _cts.Dispose();
        _cts = new();

        foreach (var item in items)
        {
            if (item.DueDateTime <= DateTime.Now)
            {
                continue;
            }

            CreateTask(item.DueDateTime);
        }
    }

    private async ValueTask CreateTask(DateTime dueDateTime)
    {
        await Task.Delay(dueDateTime - DateTime.Now, _cts.Token);
        await using var stream = AssetLoader.Open(new("avares://Sprava/Assets/Sounds/Alarm.wav"));
        await using var memoryStream = new MemoryStream();
        await stream.CopyToAsync(memoryStream);
        memoryStream.Position = 0;
        await _soundPlayer.PlayAsync(memoryStream.ToArray(), _cts.Token);
    }

    private CancellationTokenSource _cts = new();
    private ISoundPlayer _soundPlayer;

    public void Dispose()
    {
        _cts.Dispose();
    }
}
