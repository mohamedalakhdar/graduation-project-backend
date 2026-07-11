using CollegeControlSystem.Domain.Abstractions;
using MediatR;

namespace CollegeControlSystem.Application.Abstractions.Messaging;

public interface ICommand : IRequest<Result>, IBaseCommand
{
}

public interface ICommand<TReponse> : IRequest<Result<TReponse>>, IBaseCommand
{
}

// this is interface used in pipeline behaviors to identify commands
public interface IBaseCommand
{
}