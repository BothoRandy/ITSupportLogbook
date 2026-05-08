using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITSupportLogbook.Data;
using ITSupportLogbook.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITSupportLogbook.Controllers
{
    [Authorize] // 🔐 Must be logged in for EVERYTHING in this controller
    public class IssuesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IssuesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Issues
        // Supports:
        // /Issues?searchString=printer
        // /Issues?fromDate=2026-02-24&toDate=2026-02-28
        // Combined filtering supported
        [HttpGet]
        public async Task<IActionResult> Index(string? searchString, DateTime? fromDate, DateTime? toDate)
        {
            ViewData["CurrentFilter"] = searchString;
            ViewData["FromDate"] = fromDate?.ToString("yyyy-MM-dd");
            ViewData["ToDate"] = toDate?.ToString("yyyy-MM-dd");

            IQueryable<Issue> issuesQuery = _context.Issues;

            // 🔎 Search filter
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                issuesQuery = issuesQuery.Where(i =>
                    i.OfficerName.Contains(searchString) ||
                    i.Extension.Contains(searchString) ||
                    i.IssueDescription.Contains(searchString) ||
                    i.Status.Contains(searchString));
            }

            // 📅 Date filter (inclusive)
            if (fromDate.HasValue)
            {
                var from = fromDate.Value.Date;
                issuesQuery = issuesQuery.Where(i => i.DateReported >= from);
            }

            if (toDate.HasValue)
            {
                var toExclusive = toDate.Value.Date.AddDays(1);
                issuesQuery = issuesQuery.Where(i => i.DateReported < toExclusive);
            }

            var issues = await issuesQuery
                .OrderByDescending(i => i.DateReported)
                .ToListAsync();

            return View(issues);
        }

        // GET: /Issues/ExportCsv
        [HttpGet]
        public async Task<IActionResult> ExportCsv(string? searchString, DateTime? fromDate, DateTime? toDate)
        {
            IQueryable<Issue> issuesQuery = _context.Issues;

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                issuesQuery = issuesQuery.Where(i =>
                    i.OfficerName.Contains(searchString) ||
                    i.Extension.Contains(searchString) ||
                    i.IssueDescription.Contains(searchString) ||
                    i.Status.Contains(searchString));
            }

            if (fromDate.HasValue)
            {
                var from = fromDate.Value.Date;
                issuesQuery = issuesQuery.Where(i => i.DateReported >= from);
            }

            if (toDate.HasValue)
            {
                var toExclusive = toDate.Value.Date.AddDays(1);
                issuesQuery = issuesQuery.Where(i => i.DateReported < toExclusive);
            }

            var issues = await issuesQuery
                .OrderByDescending(i => i.DateReported)
                .ToListAsync();

            // Build CSV
            var sb = new StringBuilder();
            sb.AppendLine("Officer,Extension,Issue,Status,DateReported,DateResolved");

            foreach (var i in issues)
            {
                sb.AppendLine(string.Join(",",
                    CsvEscape(i.OfficerName),
                    CsvEscape(i.Extension),
                    CsvEscape(i.IssueDescription),
                    CsvEscape(i.Status),
                    CsvEscape(i.DateReported.ToString("yyyy-MM-dd HH:mm")),
                    CsvEscape(i.DateResolved?.ToString("yyyy-MM-dd HH:mm") ?? "")
                ));
            }

            var utf8WithBom = new UTF8Encoding(true);
            var bytes = utf8WithBom.GetBytes(sb.ToString());

            var fileName = $"ITSupportLogs_{DateTime.Now:yyyyMMdd_HHmm}.csv";
            return File(bytes, "text/csv", fileName);
        }

        private static string CsvEscape(string? value)
        {
            value ??= "";

            if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
            {
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }

            return value;
        }

        // ---------------- CRUD ----------------

        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var issue = await _context.Issues.FirstOrDefaultAsync(m => m.Id == id);
            if (issue == null) return NotFound();

            return View(issue);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Issue issue)
        {
            if (!ModelState.IsValid)
                return View(issue);

            if (issue.DateReported == default)
                issue.DateReported = DateTime.Now;

            _context.Issues.Add(issue);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var issue = await _context.Issues.FindAsync(id);
            if (issue == null) return NotFound();

            return View(issue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Issue issue)
        {
            if (id != issue.Id) return NotFound();

            if (!ModelState.IsValid)
                return View(issue);

            try
            {
                _context.Update(issue);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueExists(issue.Id)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        // 🔒 OPTIONAL: Only Admin can delete
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var issue = await _context.Issues.FirstOrDefaultAsync(m => m.Id == id);
            if (issue == null) return NotFound();

            return View(issue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        // 🔒 OPTIONAL: Only Admin can delete
        // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var issue = await _context.Issues.FindAsync(id);
            if (issue != null)
            {
                _context.Issues.Remove(issue);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool IssueExists(int id)
        {
            return _context.Issues.Any(e => e.Id == id);
        }
    }
}