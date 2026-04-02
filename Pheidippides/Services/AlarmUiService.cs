using System.Threading;
using System.Threading.Tasks;
using Gaia.Services;
using Inanna.Services;
using Rooster.Contract.Models;
using Rooster.Contract.Services;

namespace Pheidippides.Services;

public interface IAlarmUiService
    : IUiService<RoosterGetRequest, RoosterPostRequest, RoosterGetResponse, RoosterPostResponse>;

public sealed class AlarmUiService(
    IAlarmHttpService toDoHttpService,
    IAlarmDbService toDoDbService,
    IAlarmUiCache uiCache,
    INavigator navigator,
    string serviceName,
    IStatusBarService statusBarService,
    IInannaViewModelFactory factory
)
    : UiService<
        RoosterGetRequest,
        RoosterPostRequest,
        RoosterGetResponse,
        RoosterPostResponse,
        IAlarmHttpService,
        IAlarmDbService,
        IAlarmUiCache
    >(toDoHttpService, toDoDbService, uiCache, navigator, serviceName, statusBarService, factory),
        IAlarmUiService
{
    protected override async ValueTask<IValidationErrors> RefreshServiceCore(CancellationToken ct)
    {
        var request = new RoosterGetRequest { IsGetAlarms = true };
        var response = await DbService.GetAsync(request, ct);
        await UiCache.UpdateAsync(response, ct);

        return response;
    }
}
