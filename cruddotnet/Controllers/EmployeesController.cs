using Microsoft.AspNetCore.Mvc;
using cruddotnet.Data;
using cruddotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace cruddotnet.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public EmployeesController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> List(string search)
        {
            var query = dbContext.Employees
                .Include(e => e.Department)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(e => e.EmployeeName.Contains(search));
            }

            var employees = await query.ToListAsync();
            return View(employees);
        }


        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Departments = dbContext.Departments.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List", "Employees");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int employeeId)
        {
            if (HttpContext.Session.GetString("UserRole") == "User")
            {
                TempData["Alert"] = "Akses ditolak: hanya admin yang dapat melakukan aksi ini.";
                return RedirectToAction("List");
            }

            var employee = await dbContext.Employees.FindAsync(employeeId);
            ViewBag.Departments = dbContext.Departments.ToList();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee viewModel)
        {
            if (HttpContext.Session.GetString("UserRole") == "User")
            {
                TempData["Alert"] = "Akses ditolak: hanya admin yang dapat melakukan aksi ini.";
                return RedirectToAction("List");
            }

            var employee = await dbContext.Employees.FindAsync(viewModel.EmployeeId);

            if (employee is not null)
            {
                employee.EmployeeName = viewModel.EmployeeName;
                employee.DepartmentId = viewModel.DepartmentId;
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Employees");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Employee viewModel)
        {
            if (HttpContext.Session.GetString("UserRole") == "User")
            {
                TempData["Alert"] = "Akses ditolak: hanya admin yang dapat melakukan aksi ini.";
                return RedirectToAction("List");
            }

            var employee = await dbContext.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == viewModel.EmployeeId);

            if (employee is not null)
            {
                dbContext.Employees.Remove(viewModel);
                await dbContext.SaveChangesAsync();
            }
            return RedirectToAction("List", "Employees");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int employeeId)
        {
            var employee = await dbContext.Employees
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee is null)
                return NotFound();

            return View(employee);
        }
    }
}
