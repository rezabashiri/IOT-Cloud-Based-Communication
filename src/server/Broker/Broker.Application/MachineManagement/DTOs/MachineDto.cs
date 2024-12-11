using Shared.DTOs.Messages;

namespace Broker.Application.MachineManagement.DTOs;

public record MachineDto(
    Guid Id,
    string Name,
    MachineStatus Status);