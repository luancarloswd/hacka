using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hacka.Domain.Abstractions
{
    public interface IEventZabbixRepository
    {
        Task<IEnumerable<EventZabbixParams>> GetAllAsync();
        Task<IEnumerable<EventZabbixParams>> GetAllAsync(Expression<Func<EventZabbixParams, bool>> expression);
        Task<EventZabbixParams> AddAsync(EventZabbixParams eventZabbix);
        Task<EventZabbixParams> GetByIdAsync(string eventId);
        Task<EventZabbixParams> UpdateAsync(EventZabbixParams eventZabbix);
        Task<EventZabbixParams> RemoveAsync(string eventId);
    }
}
