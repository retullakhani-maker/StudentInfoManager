using Core.Interfaces;
using Core.Model;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using System.Security.Claims;

namespace StudentInfoManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IStudentService _studentService;
        private readonly IStateService _stateService;
        private readonly ICityService _cityService;

        public AdminController(ApplicationDbContext context, 
                                IWebHostEnvironment env, 
                                UserManager<ApplicationUser> userManager,
                                IStudentService studentService,
                                IStateService stateService, 
                                ICityService cityService)
        {
            _context = context;
            _env = env;
            _userManager = userManager;
            _studentService = studentService;
            _stateService = stateService;
            _cityService = cityService;
        }
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllAsync();

            //var fullName = User.FindFirst(CustomClaimTypes.FullName)?.Value;
            //var city = User.FindFirst(CustomClaimTypes.City)?.Value;
            //var state = User.FindFirst(CustomClaimTypes.State)?.Value;

            return View(students); // grid view of students
        }

        public async Task<IActionResult> LoadGrid()
        {
            var students = await _studentService.GetAllAsync();
            return PartialView("_StudentGrid", students);
        }

        public async Task<IActionResult> Create()
        {
            var states = await _stateService.GetAllStatesAsync();

            ViewBag.States = states
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.Name })
                .ToList();

            ViewBag.Cities = new List<SelectListItem>(); // Empty initially

            return PartialView("_Create");
        }

        // Get Cities by StateId (AJAX)
        [HttpGet]
        public async Task<JsonResult> GetCitiesByState(int stateId)
        {
            var cities = await _cityService.GetCitiesByStateIdAsync(stateId);
            return Json(cities.Select(c => new { c.Id, c.Name }));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Students student, IFormFile PhotoFile)
        {
            try
            {
                if (PhotoFile != null && PhotoFile.Length > 0)
                {
                    var uploads = Path.Combine(_env.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploads);
                    var filePath = Path.Combine(uploads, PhotoFile.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await PhotoFile.CopyToAsync(stream);
                    }

                    student.PhotoPath = "/uploads/" + PhotoFile.FileName;
                }

                if (ModelState.IsValid || (student?.State == null && student?.City == null))
                {
                    var user = new ApplicationUser
                    {
                        UserName = student.Email,
                        Email = student.Email,
                        PhoneNumber = student.PhoneNumber,
                        Role= "Student"
                    };

                    var result = await _userManager.CreateAsync(user, "Student@123");


                    if (result.Succeeded)
                    {
                        // 2️⃣ Assign Role
                        await _userManager.AddToRoleAsync(user, "Student");

                        // 3️⃣ Assign Claims (based on your Students entity)
                        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.FullName, student.FirstName + " " + student.LastName));
                        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.City, student.City?.Name ?? ""));
                        await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.State, student.State?.Name ?? ""));

                        // 4️⃣ Save mapping in Students table
                        student.User = user;
                        student.ApplicationUserId = user.Id.ToString();
                        student.Id = Guid.NewGuid();

                        _context.Add(student);
                        await _context.SaveChangesAsync();

                        return Ok(); // success
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
                // Re-populate dropdowns if validation fails
                ViewBag.States = new SelectList(_context.States, "Id", "Name", student.StateId);
                ViewBag.Cities = new SelectList(_context.Cities, "Id", "Name", student.CityId);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
           
            return BadRequest(ModelState);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
                return NotFound();
            
            return PartialView("_Create", student); // reuse same form
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Students student, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                var existing = await _context.Students.FindAsync(student.Id);
                if (existing == null) return NotFound();

                // Update scalar properties
                existing.FirstName = student.FirstName;
                existing.LastName = student.LastName;
                existing.Email = student.Email;
                existing.Address = student.Address;
                existing.PhoneNumber = student.PhoneNumber;
                existing.Gender = student.Gender;
                existing.CityId = student.CityId;
                existing.StateId = student.StateId;
                existing.Birthplace = student.Birthplace;
                existing.DateOfBirth = student.DateOfBirth;

                // Update photo if uploaded
                if (Photo != null && Photo.Length > 0)
                {
                    var fileName = Guid.NewGuid() + Path.GetExtension(Photo.FileName);
                    var filePath = Path.Combine("wwwroot/uploads", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await Photo.CopyToAsync(stream);
                    }

                    existing.PhotoPath = "/uploads/" + fileName;
                }

                _context.Update(existing);
                await _context.SaveChangesAsync();
                return Ok();
            }

            return PartialView("_Create", student);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return Ok();
        }

        // ========================= DETAILS =========================

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();

            return PartialView("_Details", student); // you can create a _Details partial
        }
    }
}
