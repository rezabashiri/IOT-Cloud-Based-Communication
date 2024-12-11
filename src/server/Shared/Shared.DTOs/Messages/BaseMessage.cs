using System;

namespace Shared.DTOs.Messages;

[Serializable]
public record BaseMessage(Guid ClientId, string Type);