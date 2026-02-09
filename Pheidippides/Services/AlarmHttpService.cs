using System;
using System.Net.Http;
using System.Text.Json;
using Gaia.Models;
using Gaia.Services;
using Rooster.Contract.Models;
using Rooster.Contract.Services;

namespace Pheidippides.Services;

public sealed class AlarmHttpService(
    HttpClient httpClient,
    JsonSerializerOptions options,
    ITryPolicyService tryPolicyService,
    IFactory<Memory<HttpHeader>> headersFactory
)
    : HttpService<RoosterGetRequest, RoosterPostRequest, RoosterGetResponse, RoosterPostResponse>(
        httpClient,
        options,
        tryPolicyService,
        headersFactory
    ),
        IAlarmHttpService
{
    protected override RoosterGetRequest CreateHealthCheckGetRequest()
    {
        return new() { IsGetAlarms = true };
    }
}
