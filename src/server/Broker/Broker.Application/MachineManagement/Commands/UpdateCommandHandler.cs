using AutoMapper;
using Broker.Application.MachineManagement.DTOs;
using Broker.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Wrapper;

namespace Broker.Application.MachineManagement.Commands;

public class UpdateCommandHandler : IRequestHandler<UpdateCommand, IResult<MachineDto>>
{
    private readonly IBrokerDbContext _brokerDbContext;
    private readonly IMapper _mapper;

    public UpdateCommandHandler(IBrokerDbContext brokerDbContext,
        IMapper mapper)
    {
        _brokerDbContext = brokerDbContext;
        _mapper = mapper;
    }

    public async Task<IResult<MachineDto>> Handle(UpdateCommand request,
        CancellationToken cancellationToken)
    {
        var machine = await _brokerDbContext.Machines.FirstOrDefaultAsync(x => x.Id == request.ClientId);

        if (machine is null)
        {
            return Result<MachineDto>.Fail("Machine does not exist");
        }

        machine.UpdateStatus(request.Status);
        await _brokerDbContext.SaveChangesAsync(cancellationToken);

        return Result<MachineDto>.Success(_mapper.Map<MachineDto>(machine));
    }
}