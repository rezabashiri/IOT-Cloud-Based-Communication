using Broker.Application.MachineManagement.Queries;
using Broker.Interface.Protos;
using Grpc.Core;
using MediatR;

namespace Broker.Interface.Services;

public class MachineService : Machine.MachineBase
{
    private readonly IMediator _mediator;

    public MachineService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<GetMachinesReply> GetMachines(GetMachinesRequest request,
        ServerCallContext context)
    {
        var result = await _mediator.Send(new QueryMachines());
        var response = new GetMachinesReply();

        response.Machines.AddRange(
            result.Data.Select(
                x => new MachineType
                {
                    Id = x.Id.ToString(),
                    Name = x.Name,
                    MachineStatus = Enum.GetName(x.Status.GetType(), x.Status)
                }));
        return response;
    }
}