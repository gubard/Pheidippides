using System.Collections.Generic;
using Inanna.Models;
using Pheidippides.Models;

namespace Pheidippides.Ui;

public sealed class AlarmsViewModel : ViewModelBase
{
    public AlarmsViewModel(IEnumerable<AlarmNotify> alarms)
    {
        Alarms = alarms;
    }

    public IEnumerable<AlarmNotify> Alarms { get; }
}
