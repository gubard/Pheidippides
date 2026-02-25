using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Platform;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Helpers;
using Inanna.Models;
using Inanna.Services;
using Pheidippides.Models;

namespace Pheidippides.Services;

public interface IAlarmScheduler
{
    ConfiguredValueTaskAwaitable UpdateAlarmsAsync(
        ReadOnlySpan<AlarmNotify> items,
        CancellationToken ct
    );
}

public sealed class EmptyAlarmScheduler : IAlarmScheduler
{
    public ConfiguredValueTaskAwaitable UpdateAlarmsAsync(
        ReadOnlySpan<AlarmNotify> items,
        CancellationToken ct
    )
    {
        return TaskHelper.ConfiguredCompletedTask;
    }
}

public sealed class DefaultAlarmScheduler : IAlarmScheduler, IDisposable
{
    public DefaultAlarmScheduler(
        ISoundPlayer soundPlayer,
        IDialogService dialogService,
        IAppResourceService appResourceService
    )
    {
        _soundPlayer = soundPlayer;
        _dialogService = dialogService;
        _appResourceService = appResourceService;

        using var stream = AssetLoader.Open(new("avares://Sprava/Assets/Sounds/Alarm.wav"));
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;
        _soundData = memoryStream.ToArray();
    }

    public ConfiguredValueTaskAwaitable UpdateAlarmsAsync(
        ReadOnlySpan<AlarmNotify> items,
        CancellationToken ct
    )
    {
        foreach (var cts in _ctsMap)
        {
            cts.Value.Cancel();
            cts.Value.Dispose();
        }

        _ctsMap.Clear();

        foreach (var item in items)
        {
            CreateTask(item);
        }

        return TaskHelper.ConfiguredCompletedTask;
    }

    public void Dispose()
    {
        foreach (var cts in _ctsMap)
        {
            cts.Value.Dispose();
        }
    }

    private readonly Dictionary<Guid, CancellationTokenSource> _ctsMap = new();
    private readonly ISoundPlayer _soundPlayer;
    private readonly IDialogService _dialogService;
    private readonly IAppResourceService _appResourceService;
    private readonly ReadOnlyMemory<byte> _soundData;

    private async ValueTask CreateTask(AlarmNotify item)
    {
        var cts = new CancellationTokenSource();
        _ctsMap[item.Id] = cts;
        var time = item.DueDateTime - DateTime.Now;
        await Task.Delay(time, cts.Token);
        var ct = CancellationToken.None;

        await TaskHelper.WhenAllAsync(
            [
                _soundPlayer.PlayAsync(_soundData, true, cts.Token),
                _dialogService.ShowMessageBoxAsync(
                    new(
                        _appResourceService
                            .GetResource<string>("Lang.Alarm")
                            .DispatchToDialogHeader(),
                        item.Name,
                        new DialogButton(
                            _appResourceService.GetResource<string>("Lang.Ok"),
                            UiHelper.CreateCommand(async c =>
                            {
                                await cts.CancelAsync();
                                await _dialogService.CloseMessageBoxAsync(c);

                                await DiHelper
                                    .ServiceProvider.GetService<IAlarmUiService>()
                                    .PostAsync(
                                        Guid.NewGuid(),
                                        new()
                                        {
                                            Edits =
                                            [
                                                new()
                                                {
                                                    Ids = [item.Id],
                                                    IsCompleted = true,
                                                    IsEditIsCompleted = true,
                                                },
                                            ],
                                        },
                                        c
                                    );
                            }),
                            null,
                            DialogButtonType.Primary
                        )
                    ),
                    ct
                ),
            ],
            ct
        );
    }
}
