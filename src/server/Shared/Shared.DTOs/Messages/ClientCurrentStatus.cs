using System;

namespace Shared.DTOs.Messages;

public record ClientCurrentStatus(
    Guid ClientId,
    MachineStatus MachineStatus) : BaseMessage(ClientId, nameof(ClientCurrentStatus));