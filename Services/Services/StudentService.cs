using Core.Interfaces;
using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;

        public StudentService(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        public Task<IEnumerable<Students>> GetAllAsync()
        {
            var studentAllData = _studentRepository.GetAllAsync();
            
            foreach (var item in studentAllData.Result.ToList())
            {

            }

            return studentAllData;
        }

        public Task<Students> GetByIdAsync(int id)
        {
            return _studentRepository.GetByIdAsync(id);
        }

        public Task AddAsync(Students student)
        {
            return _studentRepository.AddAsync(student);
        }

        public Task UpdateAsync(Students student)
        {
            return _studentRepository.UpdateAsync(student);
        }

        public Task DeleteAsync(int id)
        {
            return _studentRepository.DeleteAsync(id);
        }
    }
}
