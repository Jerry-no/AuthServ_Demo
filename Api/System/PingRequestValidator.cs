using FluentValidation;

namespace AuthService.Api.System;

public sealed class PingRequestValidator : AbstractValidator<PingRequest>
{
    public PingRequestValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty()
            .WithMessage("Message is required")
            .MaximumLength(100)
            .WithMessage("Message must not exceed 100 characters");
    }
}