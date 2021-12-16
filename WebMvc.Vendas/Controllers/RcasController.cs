using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebMvc.Vendas.Data;
using WebMvc.Vendas.Models;

namespace WebMvc.Vendas.Controllers
{
    public class RcasController : Controller
    {
        private readonly VendasContext _context;

        public RcasController(VendasContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Rca.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rca = await _context.Rca
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rca == null)
            {
                return NotFound();
            }

            return View(rca);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Rca rca)
        {
            if (ModelState.IsValid)
            {
                _context.Add(rca);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(rca);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rca = await _context.Rca.FindAsync(id);
            if (rca == null)
            {
                return NotFound();
            }
            return View(rca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Rca rca)
        {
            if (id != rca.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rca);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RcaExists(rca.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(rca);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var rca = await _context.Rca
                .FirstOrDefaultAsync(m => m.Id == id);
            if (rca == null)
            {
                return NotFound();
            }

            return View(rca);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rca = await _context.Rca.FindAsync(id);
            _context.Rca.Remove(rca);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RcaExists(int id)
        {
            return _context.Rca.Any(e => e.Id == id);
        }

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }
    }
}
