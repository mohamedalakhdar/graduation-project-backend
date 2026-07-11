using CollegeControlSystem.Domain.Abstractions;
using MediatR;

namespace CollegeControlSystem.Application.Abstractions.Messaging;

public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
}