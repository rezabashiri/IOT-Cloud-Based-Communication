using Broker.Interface.Services;

namespace Broker.Interface.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IEndpointRouteBuilder UsegRpcServices(this IEndpointRouteBuilder builder)
    {
        builder.MapGrpcService<MachineService>();

        return builder;
    }
}