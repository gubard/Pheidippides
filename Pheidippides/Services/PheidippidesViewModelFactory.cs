using Inanna.Models;
using Pheidippides.Models;
using Pheidippides.Ui;

namespace Pheidippides.Services;

public interface IPheidippidesViewModelFactory
{
    AlarmsParametersViewModel CreateAlarmsParameters(AlarmNotify item);
    AlarmsParametersViewModel CreateAlarmsParameters(ValidationMode mode, bool isShowEdit);
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
}
