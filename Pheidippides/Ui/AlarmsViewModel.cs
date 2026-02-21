using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using Gaia.Helpers;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Pheidippides.Models;
using Pheidippides.Services;

namespace Pheidippides.Ui;

public sealed class AlarmsViewModel : ViewModelBase, IHeader, IInit, ISave
{
    public AlarmsViewModel(
        IAppResourceService appResourceService,
        IAlarmUiCache alarmUiCache,
        IPheidippidesViewModelFactory factory,
        IObjectStorage objectStorage
    )
    {
        _objectStorage = objectStorage;
        ;

        Header = new TextBlock
        {
            Text = appResourceService.GetResource<string>("Lang.Alarms"),
            Classes = { "h3" },
        };

        AlarmList = factory.CreateAlarmList(alarmUiCache.Alarms);
    }

    public object Header { get; }
    public AlarmListViewModel AlarmList { get; }

    public ConfiguredValueTaskAwaitable InitAsync(CancellationToken ct)
    {
        return InitUiCore(ct).ConfigureAwait(false);
    }

    public ConfiguredValueTaskAwaitable SaveAsync(CancellationToken ct)
    {
        return SaveUiCore(ct).ConfigureAwait(false);
    }

    private readonly IObjectStorage _objectStorage;

    private async ValueTask InitUiCore(CancellationToken ct)
    {
        var settings = await _objectStorage.LoadAsync<AlarmsSettings>(ct);
        Dispatcher.UIThread.Post(() => AlarmList.OrderBy = settings.OrderBy);
        await AlarmList.InitAsync(ct);
    }

    private async ValueTask SaveUiCore(CancellationToken ct)
    {
        await _objectStorage.SaveAsync(new AlarmsSettings { OrderBy = AlarmList.OrderBy }, ct);
        await AlarmList.SaveAsync(ct);
    }
}
