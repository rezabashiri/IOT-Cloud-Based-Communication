using AutoMapper;
using Broker.Application.MachineManagement.DTOs;
using Broker.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Core.Extensions;
using Shared.Core.Wrapper;

namespace Broker.Application.MachineManagement.Queries;

public class QueryMachinesCommandHandler : IRequestHandler<QueryMachines, PaginatedResult<MachineDto>>
{
    private readonly IBrokerDbContext _brokerDbContext;
    private readonly IMapper _mapper;

    public QueryMachinesCommandHandler(IBrokerDbContext brokerDbContext,
        IMapper mapper)
    {
        _brokerDbContext = brokerDbContext;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<MachineDto>> Handle(QueryMachines request,
        CancellationToken cancellationToken)
    {
        var paginatedResult = await _brokerDbContext.Machines.AsNoTracking().ToPaginatedListAsync(1, 100);
        if (paginatedResult is { TotalCount: > 0 })
        {
            var dtos = paginatedResult.Data.Select(_mapper.Map<MachineDto>).ToList();
            return PaginatedResult<MachineDto>.Success(
                dtos,
                paginatedResult.TotalCount,
                paginatedResult.CurrentPage,
                paginatedResult.PageSize);
        }

        return PaginatedResult<MachineDto>.Fail("No data found");
    }
}