using Core.Interfaces;
using Core.Model;

namespace StudentInfoManager.Job
{
    public class StudentJobManager
    {
        private readonly IStudentService _studentService;

        public StudentJobManager(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public async Task AddDailyStudent()
        {
            var student = new Students
            {
                FirstName = "Daily Student FirstName" + DateTime.Now.ToString("yyyyMMddhhmm"),
                LastName = "Daily Student LastName" + DateTime.Now.ToString("yyyyMMddhhmm"),
                Email = "DailyStudent" + DateTime.Now.ToString("yyyyMMddhhmm") + "@test.com",
                Address = "Daily Student Address" + DateTime.Now.ToString("yyyyMMdd"),
                PhoneNumber = "9988776655",
                Gender = "Male",
                CityId = 1,
                StateId = 1,
                Birthplace = "Surat" + DateTime.Now.ToString("yyyyMMdd"),
                DateOfBirth = DateTime.Now,
                PhotoPath = "/uploads/Profile.png"
            };

            await _studentService.AddAsync(student);
        }
    }
}
