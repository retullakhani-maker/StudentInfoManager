using Core.Interfaces;
using Core.Model;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Services.Services;
using System;
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
            return View(students);
        }

        // Return grid partial for grid refresh
        [HttpGet]
        public async Task<IActionResult> LoadGrid()
        {
            var students = await _studentService.GetAllAsync();
            return PartialView("_StudentGrid", students);
        }

        // Create GET -> returns blank form partial
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            // remove stale Id from ModelState if present
            ModelState.Remove("Id");

            await PopulateDropdownsAsync();
            return PartialView("_StudentForm", new Students());
        }

        [HttpGet]
        public async Task<JsonResult> GetCitiesByState(int stateId)
        {
            var cities = await _cityService.GetCitiesByStateIdAsync(stateId);
            return Json(cities.Select(c => new { c.Id, c.Name }));
        }

        // Edit GET -> return form populated for editing
        [HttpGet]
        public async Task<IActionResult> StudentEdit(Guid id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null) return NotFound();

            await PopulateDropdownsAsync(student.StateId, student.CityId);
            return PartialView("_StudentForm", student);
        }

        // Unified Save - handles create + update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(Students student, IFormFile? PhotoFile, string? ExistingPhotoPath)
        {
            // handle file upload (if any)
            if (PhotoFile != null && PhotoFile.Length > 0)
            {
                var uploads = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploads);

                var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(PhotoFile.FileName);
                var filePath = Path.Combine(uploads, uniqueName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await PhotoFile.CopyToAsync(stream);
                }

                // delete old file if present
                if (!string.IsNullOrEmpty(ExistingPhotoPath))
                {
                    try
                    {
                        var oldFullPath = Path.Combine(_env.WebRootPath, ExistingPhotoPath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldFullPath))
                            System.IO.File.Delete(oldFullPath);
                    }
                    catch { /* log if desired */ }
                }

                student.PhotoPath = "/uploads/" + uniqueName;
            }
            else
            {
                // keep existing path if no new file uploaded
                student.PhotoPath = ExistingPhotoPath;
            }

            // determine create vs update by Id
            if (student.Id == Guid.Empty || student.Id == null)
            {
                // CREATE
                var user = new ApplicationUser
                {
                    UserName = student.Email,
                    Email = student.Email,
                    PhoneNumber = student.PhoneNumber,
                    Role = "Student"
                };

                var result = await _userManager.CreateAsync(user, "Student@123");
                if (!result.Succeeded)
                {
                    foreach (var err in result.Errors)
                        ModelState.AddModelError("", err.Description);

                    return PartialView("_StudentForm", student);
                }

                await _userManager.AddToRoleAsync(user, "Student");
                await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.FullName, student.FirstName + " " + student.LastName));
                await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.City, student.City?.Name ?? ""));
                await _userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.State, student.State?.Name ?? ""));

                student.User = user;
                student.ApplicationUserId = user.Id.ToString();
                student.Id = Guid.NewGuid();

                await _studentService.AddAsync(student);
            }
            else
            {
                // UPDATE
                var existing = await _studentService.GetByIdAsync(student.Id);
                if (existing == null) return NotFound();

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

                // update photo path if changed (we already set student.PhotoPath above)
                existing.PhotoPath = student.PhotoPath;

                await _studentService.UpdateAsync(existing);
            }

            // success -> return JSON that client understands
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> StudentDelete(Guid id)
        {
            await _studentService.DeleteAsync(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> StudentDetails(Guid id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student == null) return NotFound();
            return PartialView("_StudentDetails", student);
        }

        // make PopulateDropdowns async and use await rather than .Result
        private async Task PopulateDropdownsAsync(int? stateId = null, int? cityId = null)
        {
            var states = await _stateService.GetAllStatesAsync();
            ViewBag.States = states.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name,
                Selected = (stateId != null && s.Id == stateId)
            }).ToList();

            if (stateId != null)
            {
                var cities = await _cityService.GetCitiesByStateIdAsync(Convert.ToInt32(stateId));
                ViewBag.Cities = cities.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = (cityId != null && c.Id == cityId)
                }).ToList();
            }
            else
            {
                ViewBag.Cities = new List<SelectListItem>();
            }
        }
    }
}
