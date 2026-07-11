using CollegeControlSystem.Domain.Abstractions;
using MediatR;

namespace CollegeControlSystem.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}