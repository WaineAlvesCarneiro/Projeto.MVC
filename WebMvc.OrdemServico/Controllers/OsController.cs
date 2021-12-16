using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebMvc.OrdemServico.Data;
using WebMvc.OrdemServico.Models;

namespace WebMvc.OrdemServico.Controllers
{
    public class OsController : Controller
    {
        private readonly OrdemServicoContext _context;

        public OsController(OrdemServicoContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var appOSContext = _context.Os.Include(o => o.Cliente);
            return View(await appOSContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "O.S. não foi encontrada para visualizar detalhes" });
            }
            var os = await _context.Os.Include(o => o.Cliente).FirstOrDefaultAsync(m => m.Id == id);
            if (os == null)
            {
                return RedirectToAction(nameof(Error), new { message = "O.S. não contem dados para exibição de detalhes" });
            }
            var listServicos = await ListServicosIdAsync(os.Id);
            var viewModel = new OsPrestadoViewModel
            {
                Os = os,
                Prestadoes = listServicos
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var listCliente = await _context.Cliente.OrderBy(x => x.Nome).ToListAsync();
            var viewModel = new OsPrestadoViewModel
            {
                Clientees = listCliente
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Os os)
        {
            if (ModelState.IsValid)
            {
                _context.Add(os);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IncluirCreate), os);
            }
            return View();
        }

        public async Task<IActionResult> IncluirCreate(Os os)
        {
            var ClienteOs = await _context.Cliente.FirstOrDefaultAsync(obj => obj.Id == os.ClienteId);
            var listServico = await _context.Servico.OrderBy(x => x.NomeServico).ToListAsync();
            var listPrestado = await ListPrestadoOsIdAsync(os.Id);
            var viewModel = new OsPrestadoViewModel
            {
                Servicoes = listServico,
                ClienteList = ClienteOs,
                Os = os,
                Prestadoes = listPrestado
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IncluirCreate(Os os, Prestado prestado, string responda)
        {
            if (ModelState.IsValid)
            {
                switch (responda)
                {
                    case "Adicionar":
                        prestado.OsId = os.Id;
                        var valorServico = await _context.Servico.FirstOrDefaultAsync(obj => obj.Id == prestado.ServicoId);
                        prestado.ValorItem = valorServico.Preco;
                        prestado.TotalItem = prestado.QtdeServico * prestado.ValorItem;

                        _context.Add(prestado);
                        await _context.SaveChangesAsync();

                        os.TotalOs = TotalValorOs(os);
                        _context.Update(os);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(IncluirCreate), os);

                    default:
                        return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }

        public async Task<IActionResult> DeletePrestadoCreate(int? id)
        {
            var PrestadoExcluir = await _context.Prestado.FirstOrDefaultAsync(obj => obj.Id == id);
            Os os = await _context.Os.FirstOrDefaultAsync(obj => obj.Id == PrestadoExcluir.OsId);

            _context.Prestado.Remove(PrestadoExcluir);
            await _context.SaveChangesAsync();

            os.TotalOs = TotalValorOs(os);
            _context.Update(os);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IncluirCreate), os);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "O.S. não foi encontrada para visualizar editar" });
            }
            var os = await _context.Os.Include(o => o.Cliente).FirstOrDefaultAsync(m => m.Id == id);
            if (os == null)
            {
                return RedirectToAction(nameof(Error), new { message = "O.S. não contem dados para exibição de editar" });
            }
            var listCliente = await _context.Cliente.OrderBy(x => x.Nome).ToListAsync();
            var listServico = await _context.Servico.OrderBy(x => x.NomeServico).ToListAsync();
            var listPrestado = await ListPrestadoOsIdAsync(os.Id);
            var viewModel = new OsPrestadoViewModel
            {
                Os = os,
                Clientees = listCliente,
                Servicoes = listServico,
                Prestadoes = listPrestado
            };
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "Nome", os.ClienteId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Os os)
        {
            if (id != os.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "O.S. não foi encontrada para visualizar editar" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    os.TotalOs = TotalValorOs(os);
                    _context.Update(os);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OsExists(os.Id))
                    {
                        return RedirectToAction(nameof(Error), new { message = "Essa O.S não foi encontrado para update" });
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit));
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "Nome", os.ClienteId);
            return View(os);
        }

        public async Task<IActionResult> DeletePrestadoEdit(int? id)
        {
            var PrestadoExcluir = await _context.Prestado.FirstOrDefaultAsync(obj => obj.Id == id);
            Os os = await _context.Os.FirstOrDefaultAsync(obj => obj.Id == PrestadoExcluir.OsId);

            _context.Prestado.Remove(PrestadoExcluir);
            await _context.SaveChangesAsync();

            os.TotalOs = TotalValorOs(os);
            _context.Update(os);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), os);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IncluirEdit(Os os, Prestado prestado, string responda)
        {
            if (ModelState.IsValid)
            {
                switch (responda)
                {
                    case "Adicionar":
                        prestado.OsId = os.Id;
                        var valorServico = await _context.Servico.FirstOrDefaultAsync(obj => obj.Id == prestado.ServicoId);
                        prestado.ValorItem = valorServico.Preco;
                        prestado.TotalItem = prestado.QtdeServico * prestado.ValorItem;

                        _context.Add(prestado);
                        await _context.SaveChangesAsync();

                        os.TotalOs = TotalValorOs(os);
                        _context.Update(os);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Edit), os);

                    default:
                        return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "O.S. não foi encontrada para visualizar deletar" });
            }
            var os = await _context.Os.Include(o => o.Cliente).FirstOrDefaultAsync(m => m.Id == id);
            if (os == null)
            {
                return RedirectToAction(nameof(Error), new { message = "O.S. não contem dados para exibição de detalhes" });
            }
            var listServicos = await ListServicosIdAsync(os.Id);
            var viewModel = new OsPrestadoViewModel
            {
                Os = os,
                Prestadoes = listServicos
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var os = await _context.Os.FindAsync(id);
            _context.Os.Remove(os);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OsExists(int id)
        {
            return _context.Os.Any(e => e.Id == id);
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

        public async Task<List<Prestado>> ListPrestadoOsIdAsync(int? id)
        {
            var result = from obj in _context.Prestado select obj;
            result = result.Where(x => x.OsId == id);
            return await result.ToListAsync();
        }

        public async Task<List<Prestado>> ListServicosIdAsync(int? id)
        {
            var result = from obj in _context.Prestado select obj;
            result = result.Where(x => x.OsId == id).Include(o => o.Servico);
            return await result.ToListAsync();
        }

        public decimal TotalValorOs(Os os)
        {
            var result = from obj in _context.Prestado select obj;
            return os.TotalOs = result.Where(x => x.OsId == os.Id).Sum(selector: x => x.TotalItem);
        }
    }
}
