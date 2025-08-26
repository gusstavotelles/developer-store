using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Common.Events
{
    public interface IEventPublisher
    {
        Task PublishAsync(object @event, CancellationToken cancellationToken = default);
    }
}
