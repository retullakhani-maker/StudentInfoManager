using Core.Interfaces;
using Core.Model;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class CityRepository : ICityRepository
    {
        private readonly ApplicationDbContext _context;

        public CityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cities>> GetByStateIdAsync(int stateId)
        {
            return await _context.Cities
                                 .Where(c => c.StateId == stateId)
                                 .ToListAsync();
        }
    }
}
