using Broker.Application.MachineManagement.DTOs;
using MediatR;
using Shared.Core.Wrapper;
using Shared.DTOs.Messages;

namespace Broker.Application.MachineManagement.Commands;

public record UpdateCommand(
    Guid ClientId,
    MachineStatus Status) : IRequest<IResult<MachineDto>>;