using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IStudentRepository
    {
        Task<Students> GetByIdAsync(int id);
        Task<IEnumerable<Students>> GetAllAsync();
        Task AddAsync(Students student);
        Task UpdateAsync(Students student);
        Task DeleteAsync(int id);
    }
}
