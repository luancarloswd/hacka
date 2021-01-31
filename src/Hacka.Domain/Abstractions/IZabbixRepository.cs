using System.Threading.Tasks;

namespace Hacka.Domain.Abstractions
{
    public interface IZabbixRepository
    {
        Task<EStatusEvent> GetActualStatusEvent(string eventId);
    }
}
