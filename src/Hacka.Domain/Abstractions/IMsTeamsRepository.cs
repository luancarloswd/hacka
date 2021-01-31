using System.Threading.Tasks;

namespace Hacka.Domain.Abstractions
{
    public interface IMsTeamsRepository
    {
        Task SendProbleamToSquad(EventZabbixParams eventZabbix);
        Task SendInAnalisys(EventZabbixParams eventZabbix);
    }
}
