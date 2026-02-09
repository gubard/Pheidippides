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
    IResponseHandler responseHandler
)
    : UiService<
        RoosterGetRequest,
        RoosterPostRequest,
        RoosterGetResponse,
        RoosterPostResponse,
        IAlarmHttpService,
        IAlarmDbService,
        IAlarmUiCache
    >(toDoHttpService, toDoDbService, uiCache, navigator, serviceName, responseHandler),
        IAlarmUiService
{
    protected override RoosterGetRequest CreateGetRequestRefresh()
    {
        return new() { IsGetAlarms = true };
    }
}
