
using Microsoft.AspNetCore.Mvc;
using cruddotnet.Models;
using cruddotnet.Data;
using cruddotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace cruddotnet.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public DepartmentsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddDepartmentViewModel viewModel)
        {
            var department = new Department
            {
                DepartmentName = viewModel.DepartmentName,
                Employees = viewModel.Employees
            };

            await dbContext.Departments.AddAsync(department);
            await dbContext.SaveChangesAsync();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> List(string search)
        {
            var query = dbContext.Departments.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(d => d.DepartmentName.Contains(search));
            }

            var departments = await query.ToListAsync();
            return View(departments);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int DepartmentId)
        {
            if (HttpContext.Session.GetString("UserRole") == "User")
            {
                TempData["Alert"] = "Akses ditolak: hanya admin yang dapat melakukan aksi ini.";
                return RedirectToAction("List");
            }

            var departments = await dbContext.Departments.FindAsync(DepartmentId);
            return View(departments);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Department viewModel)
        {
            if (HttpContext.Session.GetString("UserRole") == "User")
            {
                TempData["Alert"] = "Akses ditolak: hanya admin yang dapat melakukan aksi ini.";
                return RedirectToAction("List");
            }

            var departments = await dbContext.Departments.FindAsync(viewModel.DepartmentId);

            if (departments is not null)
            {
                departments.DepartmentName = viewModel.DepartmentName;
                departments.Employees = viewModel.Employees;

                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Departments");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Department viewModel)
        {
            if (HttpContext.Session.GetString("UserRole") == "User")
            {
                TempData["Alert"] = "Akses ditolak: hanya admin yang dapat melakukan aksi ini.";
                return RedirectToAction("List");
            }

            var department = await dbContext.Departments
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.DepartmentId == viewModel.DepartmentId);

            if (department is not null)
            {
                dbContext.Departments.Remove(viewModel);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List", "Departments");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int departmentId)
        {
            var department = await dbContext.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);

            if (department is null)
                return NotFound();

            return View(department);
        }
    }
}
