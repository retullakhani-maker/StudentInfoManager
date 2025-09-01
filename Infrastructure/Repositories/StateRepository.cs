using Core.Interfaces;
using Core.Model;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class StateRepository : IStateRepository
    {
        private readonly ApplicationDbContext _context;

        public StateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<States>> GetAllAsync()
        {
            return await _context.States.ToListAsync();
        }

        public async Task<States> GetByIdAsync(int id)
        {
            return await _context.States.FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
