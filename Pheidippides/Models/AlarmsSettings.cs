using Gaia.Services;
using Pheidippides.Ui;

namespace Pheidippides.Models;

public sealed record AlarmsSettings
    : ObjectStorageValue<AlarmsSettings>,
        IStaticFactory<AlarmsSettings>
{
    public AlarmsOrderBy OrderBy { get; set; }

    public static AlarmsSettings Create()
    {
        return new() { OrderBy = AlarmsOrderBy.Name };
    }
}
