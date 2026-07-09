using MediatR;

namespace AuthService.Common.Abstractions.Messaging;

public interface ICommand<out TResponse>
    : IRequest<TResponse>;