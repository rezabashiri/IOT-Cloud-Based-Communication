using Broker.Application.MachineManagement.Commands;
using FluentValidation;

namespace Broker.Application.MachineManagement.Validators;

public class MoveCommandValidator : AbstractValidator<MoveCommand>
{
    public MoveCommandValidator()
    {
        RuleFor(x => x.ClientId).NotEmpty();
        RuleFor(x => x.Direction).NotEmpty();
    }
}