using MediatR;

namespace AuthService.Common.Abstractions.Messaging;

public interface IQuery<out TResponse>
    : IRequest<TResponse>
{
}