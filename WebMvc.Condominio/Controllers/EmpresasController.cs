using WebMvc.Condominio.Data;
using WebMvc.Condominio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebMvc.Condominio.Controllers
{
    [Authorize(Roles = "Suporte")]
    public class EmpresasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmpresasController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Lista e detalhes

        [Authorize(Roles = "Suporte")]
        public async Task<IActionResult> ListarEmpresas()
        {
            return View(await _context.Empresa.ToListAsync());
        }

        [Authorize(Roles = "Suporte")]
        public async Task<IActionResult> DetalharEmpresa(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Empresa não foi encontrada para visualizar os detalhes" });
            }
            var empresa = await _context.Empresa.FirstOrDefaultAsync(m => m.Id == id);
            if (empresa == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Essa Empresa esta vazia para visualizar os detalhes" });
            }
            return View(empresa);
        }

        #endregion

        #region Cadastro

        [Authorize(Roles = "Suporte")]
        public IActionResult CadastrarEmpresas()
        {
            return View();
        }

        [Authorize(Roles = "Suporte")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastrarEmpresas(Empresa empresa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(empresa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ListarEmpresas));
            }
            return View(empresa);
        }

        #endregion

        #region Editar

        [Authorize(Roles = "Suporte")]
        public async Task<IActionResult> EditarEmpresa(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Empresa não foi encontrada para editar" });
            }
            var empresa = await _context.Empresa.FindAsync(id);
            if (empresa == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Essa empresa esta vazia para editar" });
            }
            return View(empresa);
        }

        [Authorize(Roles = "Suporte")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarEmpresa(int id, Empresa empresa)
        {
            if (id != empresa.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Empresa não foi encontrada para editar" });
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(empresa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmpresaExists(empresa.Id))
                    {
                        return RedirectToAction(nameof(Error), new { message = "Essa Empresa não foi encontrada para salva/update a edição" });
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(ListarEmpresas));
            }
            return View(empresa);
        }

        #endregion

        #region Excluir

        [Authorize(Roles = "Suporte")]
        public async Task<IActionResult> ExcluirEmpresa(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Empresa não foi encontrada para deletar" });
            }
            var empresa = await _context.Empresa.FirstOrDefaultAsync(m => m.Id == id);
            if (empresa == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Essa Empresa esta vazia para deletar" });
            }
            return View(empresa);
        }

        [Authorize(Roles = "Suporte")]
        [HttpPost, ActionName("ExcluirEmpresa")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirEmpresaConfirmed(int id)
        {
            var empresa = await _context.Empresa.FindAsync(id);
            _context.Empresa.Remove(empresa);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListarEmpresas));
        }

        #endregion

        #region Metodos padrao

        private bool EmpresaExists(int id)
        {
            return _context.Empresa.Any(e => e.Id == id);
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

        #endregion
    }
}
