using Forums.Domain.Models;

namespace Forums.Domain.UseCases.CreateTopic;

public interface ICreateTopicUseCase
{
    Task<Topic> Execute(CreateTopicCommand command, CancellationToken cancellationToken);
}