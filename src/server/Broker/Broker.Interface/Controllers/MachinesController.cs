using Broker.Application.MachineManagement.Commands;
using Broker.Application.MachineManagement.DTOs;
using Broker.Application.MachineManagement.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Core.Wrapper;
using Shared.Infrastructure.Controllers;

namespace Broker.Interface.Controllers;

[ApiVersion("1")]
public class MachinesController : CommonBaseController
{
    protected new const string BasePath = $"{CommonBaseController.BasePath}/Machines";

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginatedResult<MachineDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<IActionResult> GetMachines()
    {
        var result = await Mediator.Send(new QueryMachines());
        if (result.Succeeded)
        {
            return Ok(result);
        }

        return NotFound(result);
    }

    [HttpPut("[Action]")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<IActionResult> Move(MoveCommand command)
    {
        var result = await Mediator.Send(command);
        if (result.Succeeded)
        {
            return Ok(result);
        }

        return NotFound(result);
    }
}