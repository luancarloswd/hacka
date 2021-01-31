using System;
using Hacka.Domain;
using Hacka.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hacka.Infra
{
    public class EventZabbixRepository: IEventZabbixRepository
    {
        private readonly HackaContext _context;

        public EventZabbixRepository(HackaContext context) => _context = context;

        public async Task<IEnumerable<EventZabbixParams>> GetAllAsync() => await _context.EventZabbix.ToListAsync();
        public async Task<IEnumerable<EventZabbixParams>> GetAllAsync(Expression<Func<EventZabbixParams, bool>> expression)
        {
            return await _context.EventZabbix.Where(expression).ToListAsync();
        }

        public async Task<EventZabbixParams> AddAsync(EventZabbixParams eventZabbix)
        {
            await _context.EventZabbix.AddAsync(eventZabbix);
            await _context.SaveChangesAsync();

            return eventZabbix;
        }

        public Task<EventZabbixParams> GetByIdAsync(string eventId)
        {
            return _context.EventZabbix.FirstOrDefaultAsync(ez => ez.EventId == eventId);
        }

        public async Task<EventZabbixParams> UpdateAsync(EventZabbixParams eventZabbix)
        {
            _context.Update(eventZabbix);
            await _context.SaveChangesAsync();
            return eventZabbix;
        }

        public async Task<EventZabbixParams> RemoveAsync(string eventId)
        {
            var eventZabbix = await GetByIdAsync(eventId);
            _context.Remove(eventZabbix);
            await _context.SaveChangesAsync();
            return eventZabbix;
        }
    }
}
