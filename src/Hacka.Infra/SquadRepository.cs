using System;
using Hacka.Domain;
using Hacka.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hacka.Infra
{
    public class SquadRepository : ISquadRepository
    {
        private readonly HackaContext _context;

        public SquadRepository(HackaContext context) => _context = context;

        public async Task<IEnumerable<Squad>> GetAllAsync() => await _context.Squad.ToListAsync();
        public async Task<Squad> AddAsync(Squad squad)
        {
            await _context.Squad.AddAsync(squad);
            await _context.SaveChangesAsync();

            return squad;
        }

        public Task<Squad> GetByIdAsync(int id) => _context.Squad.FirstOrDefaultAsync(ez => ez.Id == id);

        public Task<Squad> GetByNameAsync(string name) => 
            _context.Squad.FirstOrDefaultAsync(s => string.Equals(s.Name.Trim(), name.Trim(), StringComparison.InvariantCultureIgnoreCase));

        public async Task<Squad> UpdateAsync(Squad squad)
        {
            _context.Update(squad);
            await _context.SaveChangesAsync();
            return squad;
        }

        public async Task<Squad> RemoveAsync(int id)
        {
            var squad = await GetByIdAsync(id);
            _context.Remove(squad);
            await _context.SaveChangesAsync();
            return squad;
        }
    }
}
