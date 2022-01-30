using WASMClean.Domain.Common;

namespace WASMClean.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}
