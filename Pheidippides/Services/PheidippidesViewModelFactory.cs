using Avalonia.Collections;
using Inanna.Models;
using Pheidippides.Models;
using Pheidippides.Ui;

namespace Pheidippides.Services;

public interface IPheidippidesViewModelFactory
{
    AlarmsParametersViewModel CreateAlarmsParameters(AlarmNotify item);
    AlarmsParametersViewModel CreateAlarmsParameters(ValidationMode mode, bool isShowEdit);
    AlarmListViewModel CreateAlarmList(IAvaloniaReadOnlyList<AlarmNotify> alarms);
}

public sealed class PheidippidesViewModelFactory : IPheidippidesViewModelFactory
{
    public AlarmsParametersViewModel CreateAlarmsParameters(AlarmNotify item)
    {
        return new(item, ValidationMode.ValidateAll, false);
    }

    public AlarmsParametersViewModel CreateAlarmsParameters(ValidationMode mode, bool isShowEdit)
    {
        return new(mode, isShowEdit);
    }

    public AlarmListViewModel CreateAlarmList(IAvaloniaReadOnlyList<AlarmNotify> alarms)
    {
        return new(alarms);
    }
}
