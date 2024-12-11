using Broker.Application.IOTCommunication;
using MediatR;
using Shared.Core.Constants;
using Shared.Core.Wrapper;
using Shared.DTOs.Messages;

namespace Broker.Application.MachineManagement.Commands;

public class MoveCommandHandler : IRequestHandler<MoveCommand, IResult>
{
    private readonly IMessagingService _messagingService;

    public MoveCommandHandler(IMessagingService messagingService)
    {
        _messagingService = messagingService;
    }

    public async Task<IResult> Handle(MoveCommand request,
        CancellationToken cancellationToken)
    {
        // TODO : Check if machine added to database
        bool sendToIot = await _messagingService.PublishMessage(
            new Move(request.ClientId, request.Direction),
            IOTConstants.PLC_Command_Topic);

        return sendToIot ? Result.Success() : Result.Fail();
    }
}