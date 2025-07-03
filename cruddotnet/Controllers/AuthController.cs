using Microsoft.AspNetCore.Mvc;
using cruddotnet.Data;
using cruddotnet.Models.Entities;

namespace cruddotnet.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public AuthController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(AppUser model)
        {
            var user = dbContext.Users.FirstOrDefault(x => x.Username == model.Username && x.Password == model.Password);
            if (user != null)
            {
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("Username", user.Username);
                return RedirectToAction("List", "Departments");
            }

            ViewBag.Error = "Login gagal";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AppUser model)
        {
            await dbContext.Users.AddAsync(model);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
