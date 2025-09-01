using Core.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICityRepository
    {
        Task<IEnumerable<Cities>> GetByStateIdAsync(int stateId);
    }
}