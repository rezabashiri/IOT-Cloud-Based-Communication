using MediatR;
using Shared.Core.Wrapper;

namespace Broker.Application.MachineManagement.Commands;

public record MoveCommand(
    Guid ClientId,
    string Direction) : IRequest<IResult>;