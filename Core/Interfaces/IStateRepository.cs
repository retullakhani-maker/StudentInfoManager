using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IStateRepository
    {
        Task<IEnumerable<States>> GetAllAsync();
        Task<States> GetByIdAsync(int id);
    }
}