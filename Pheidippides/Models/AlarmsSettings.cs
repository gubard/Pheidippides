using Gaia.Services;
using Pheidippides.Ui;

namespace Pheidippides.Models;

public sealed class AlarmsSettings : ObjectStorageValue<AlarmsSettings>
{
    public AlarmsOrderBy OrderBy { get; set; }
}
