using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace StudentInfoManager.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentsController : Controller
    {
        private readonly StudentService _studentService;

        public StudentsController(StudentService studentService)
        {
            _studentService = studentService;
        }

        public async Task<IActionResult> LoadGrid()
        {
            var students = await _studentService.GetAllAsync();
            return PartialView("_StudentGrid", students);
        }
    }
}
