using System.Collections.Generic;
using Avalonia.Controls;
using Inanna.Models;
using Inanna.Services;
using Pheidippides.Models;
using Pheidippides.Services;

namespace Pheidippides.Ui;

public sealed class AlarmsViewModel : ViewModelBase, IHeader
{
    public AlarmsViewModel(IAppResourceService appResourceService, IAlarmUiCache alarmUiCache)
    {
        Alarms = alarmUiCache.Alarms;

        Header = new TextBlock
        {
            Text = appResourceService.GetResource<string>("Lang.Alarms"),
            Classes = { "h3" },
        };
    }

    public IEnumerable<AlarmNotify> Alarms { get; }
    public object Header { get; }
}
