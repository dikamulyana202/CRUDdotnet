using Microsoft.AspNetCore.Mvc;
using cruddotnet.Data;
using cruddotnet.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

namespace cruddotnet.Controllers
{
    public class SpendingsController : Controller
    {
        private readonly ApplicationDbContext dbContext;

        public SpendingsController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> List(decimal? min, decimal? max)
        {
            var query = dbContext.Spendings
                .Include(s => s.Employee)
                .ThenInclude(e => e.Department)
                .Where(s => s.SpendingDate.Year >= 2020 && s.SpendingDate.Year <= 2025)
                .Where(s => s.SpendingDate.Month >= 1 && s.SpendingDate.Month <= 12)
                .AsQueryable();

            if (min.HasValue)
                query = query.Where(s => s.Value >= min.Value);

            if (max.HasValue)
                query = query.Where(s => s.Value <= max.Value);

            var spendings = await query.OrderBy(s => s.Value).ToListAsync();
            return View(spendings);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Employees = dbContext.Employees.Include(e => e.Department).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Spending spending)
        {
            await dbContext.Spendings.AddAsync(spending);
            await dbContext.SaveChangesAsync();
            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int spendingId)
        {
            if (HttpContext.Session.GetString("UserRole") == "User")
            {
                TempData["Alert"] = "Akses ditolak: hanya admin yang dapat melakukan aksi ini.";
                return RedirectToAction("List");
            }

            var spending = await dbContext.Spendings.FindAsync(spendingId);
            ViewBag.Employees = dbContext.Employees.Include(e => e.Department).ToList();
            return View(spending);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Spending viewModel)
        {
            if (HttpContext.Session.GetString("UserRole") == "User")
            {
                TempData["Alert"] = "Akses ditolak: hanya admin yang dapat melakukan aksi ini.";
                return RedirectToAction("List");
            }

            var spending = await dbContext.Spendings.FindAsync(viewModel.SpendingId);
            if (spending != null)
            {
                spending.EmployeeId = viewModel.EmployeeId;
                spending.SpendingDate = viewModel.SpendingDate;
                spending.Value = viewModel.Value;
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Spending viewModel)
        {
            if (HttpContext.Session.GetString("UserRole") == "User")
            {
                TempData["Alert"] = "Akses ditolak: hanya admin yang dapat melakukan aksi ini.";
                return RedirectToAction("List");
            }

            var spending = await dbContext.Spendings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SpendingId == viewModel.SpendingId);

            if (spending != null)
            {
                dbContext.Spendings.Remove(viewModel);
                await dbContext.SaveChangesAsync();
            }

            return RedirectToAction("List");
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int spendingId)
        {
            var spending = await dbContext.Spendings
                .Include(s => s.Employee)
                .ThenInclude(e => e.Department)
                .FirstOrDefaultAsync(s => s.SpendingId == spendingId);

            if (spending == null) return NotFound();

            return View(spending);
        }

        [HttpGet]
        public IActionResult ExportExcel()
        {
            var data = dbContext.Spendings
                .Include(s => s.Employee)
                .ThenInclude(e => e.Department)
                .Where(s => s.SpendingDate.Year >= 2020 && s.SpendingDate.Year <= 2025)
                .OrderBy(s => s.Value)
                .ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Spending Report");

            ws.Cell(1, 1).Value = "Tanggal";
            ws.Cell(1, 2).Value = "Karyawan";
            ws.Cell(1, 3).Value = "Departemen";
            ws.Cell(1, 4).Value = "Nilai";

            for (int i = 0; i < data.Count; i++)
            {
                ws.Cell(i + 2, 1).Value = data[i].SpendingDate.ToString("yyyy-MM-dd");
                ws.Cell(i + 2, 2).Value = data[i].Employee?.EmployeeName;
                ws.Cell(i + 2, 3).Value = data[i].Employee?.Department?.DepartmentName;
                ws.Cell(i + 2, 4).Value = data[i].Value;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            stream.Seek(0, SeekOrigin.Begin);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "spending.xlsx");
        }

        [HttpGet]
        public IActionResult ExportPdf()
        {
            var data = dbContext.Spendings
                .Include(s => s.Employee)
                .ThenInclude(e => e.Department)
                .Where(s => s.SpendingDate.Year >= 2020 && s.SpendingDate.Year <= 2025)
                .OrderBy(s => s.Value)
                .ToList();

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            var doc = new Document(pdf);

            doc.Add(new Paragraph("Laporan Spending").SetFontSize(16).SetMarginBottom(10));

            foreach (var s in data)
            {
                var line = $"{s.SpendingDate:yyyy-MM-dd} - {s.Employee?.EmployeeName} - {s.Employee?.Department?.DepartmentName} - {s.Value:N2}";
                doc.Add(new Paragraph(line));
            }

            doc.Close();
            return File(stream.ToArray(), "application/pdf", "spending.pdf");
        }
    }
}
