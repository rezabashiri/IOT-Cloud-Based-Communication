using MediatR;
using Shared.Core.Wrapper;
using Shared.DTOs.Messages;

namespace Broker.Application.MachineManagement.Commands;

public record RegisterCommand(
    ClientRegister Client) : IRequest<IResult<Guid>>;