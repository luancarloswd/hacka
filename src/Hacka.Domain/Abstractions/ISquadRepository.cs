using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hacka.Domain.Abstractions
{
    public interface ISquadRepository
    {
        Task<IEnumerable<Squad>> GetAllAsync();
        Task<Squad> AddAsync(Squad eventZabbix);
        Task<Squad> GetByIdAsync(int id);
        Task<Squad> GetByNameAsync(string name);
        Task<Squad> UpdateAsync(Squad eventZabbix);
        Task<Squad> RemoveAsync(int id);
    }
}
