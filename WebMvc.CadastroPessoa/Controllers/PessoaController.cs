using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebMvc.CadastroPessoa.Data;
using WebMvc.CadastroPessoa.Models;

namespace WebMvc.CadastroPessoa.Controllers
{
    public class PessoaController : Controller
    {
        private readonly DbContextPessoa _context;

        public PessoaController(DbContextPessoa context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var dbpessoas = _context.Pessoa.Include(p => p.Endereco);
            return View(await dbpessoas.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoa
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pessoa pessoa, Endereco endereco)
        {
            pessoa.Endereco.Cep = ApenasNumeros(endereco.Cep);
            pessoa.EnderecoId = endereco.Id;

            pessoa.Celular = ApenasNumeros(pessoa.Celular);
            pessoa.Telefone = ApenasNumeros(pessoa.Telefone);

            if (ModelState.IsValid)
            {
                _context.Add(pessoa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pessoa);
        }

        #region Remove caracteres string
        public static string ApenasNumeros(string valor)
        {
            var onlyNumber = "";
            foreach (var s in valor)
            {
                if (char.IsDigit(s))
                {
                    onlyNumber += s;
                }
            }
            return onlyNumber.Trim();
        }
        #endregion

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoa
                .Include(p => p.Endereco)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pessoa = await _context.Pessoa.FindAsync(id);
            var endereco = await _context.Endereco.FirstOrDefaultAsync(m => m.Id == pessoa.EnderecoId);

            _context.Endereco.Remove(endereco);
            _context.Pessoa.Remove(pessoa);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PessoaExists(int id)
        {
            return _context.Pessoa.Any(e => e.Id == id);
        }
    }
}

