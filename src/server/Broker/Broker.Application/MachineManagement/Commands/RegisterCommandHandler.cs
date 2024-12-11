using Broker.Domain.Abstractions;
using Broker.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.Core.Wrapper;
using Shared.DTOs.Messages;

namespace Broker.Application.MachineManagement.Commands;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, IResult<Guid>>
{
    private readonly IBrokerDbContext _brokerDbContext;
    private readonly IStringLocalizer<RegisterCommandHandler> _localizer;

    public RegisterCommandHandler(IBrokerDbContext brokerDbContext,
        IStringLocalizer<RegisterCommandHandler> localizer)
    {
        _brokerDbContext = brokerDbContext;
        _localizer = localizer;
    }

    public async Task<IResult<Guid>> Handle(RegisterCommand request,
        CancellationToken cancellationToken)
    {
        var machine = await _brokerDbContext.Machines.FirstOrDefaultAsync(
            x => request.Client.ClientId.Equals(x.Id),
            cancellationToken);
        if (machine != null)
        {
            // Messages have to move to resources in real application
            return Result<Guid>.Fail(_localizer["Machine already exists!"]);
        }

        var entity = Machine.Create(
            request.Client.ClientId,
            request.Client.ClientName,
            MachineStatus.Connected);
        var add = _brokerDbContext.Machines.Add(entity);
        await _brokerDbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(add.Entity.Id);
    }
}