using Broker.Application.MachineManagement.DTOs;
using MediatR;
using Shared.Core.Wrapper;

namespace Broker.Application.MachineManagement.Queries;

public record QueryMachines : IRequest<PaginatedResult<MachineDto>>;