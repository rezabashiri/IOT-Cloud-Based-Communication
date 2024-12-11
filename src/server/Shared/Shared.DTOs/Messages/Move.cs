using System;

namespace Shared.DTOs.Messages;

public record Move(
    Guid ClientId,
    string Direction) : BaseMessage(ClientId, nameof(Move));