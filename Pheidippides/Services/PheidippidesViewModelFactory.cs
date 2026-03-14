using Avalonia.Collections;
using Gaia.Services;
using Inanna.Models;
using Inanna.Services;
using Pheidippides.Models;
using Pheidippides.Ui;

namespace Pheidippides.Services;

public interface IPheidippidesViewModelFactory
{
    AlarmsParametersViewModel CreateAlarmsParameters(AlarmNotify item);
    AlarmsParametersViewModel CreateAlarmsParameters(ValidationMode mode, bool isShowEdit);
    AlarmListViewModel CreateAlarmList(IAvaloniaReadOnlyList<AlarmNotify> alarms);
    AlarmsViewModel CreateAlarms();
}

public sealed class PheidippidesViewModelFactory : IPheidippidesViewModelFactory
{
    public PheidippidesViewModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public AlarmsParametersViewModel CreateAlarmsParameters(AlarmNotify item)
    {
        return new(
            item,
            ValidationMode.ValidateAll,
            false,
            _serviceProvider.GetService<ISafeExecuteWrapper>()
        );
    }

    public AlarmsParametersViewModel CreateAlarmsParameters(ValidationMode mode, bool isShowEdit)
    {
        return new(mode, isShowEdit, _serviceProvider.GetService<ISafeExecuteWrapper>());
    }

    public AlarmListViewModel CreateAlarmList(IAvaloniaReadOnlyList<AlarmNotify> alarms)
    {
        return new(
            alarms,
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            _serviceProvider.GetService<PheidippidesCommands>()
        );
    }

    public AlarmsViewModel CreateAlarms()
    {
        return new(
            _serviceProvider.GetService<IAppResourceService>(),
            _serviceProvider.GetService<IAlarmUiCache>(),
            this,
            _serviceProvider.GetService<IObjectStorage>(),
            _serviceProvider.GetService<ISafeExecuteWrapper>(),
            _serviceProvider.GetService<PheidippidesCommands>()
        );
    }

    private readonly IServiceProvider _serviceProvider;
}
