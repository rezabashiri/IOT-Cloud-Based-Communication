using System;

namespace Shared.DTOs.Messages;

public record ClientRegister(Guid ClientId, string ClientName) : BaseMessage(ClientId, nameof(ClientRegister));