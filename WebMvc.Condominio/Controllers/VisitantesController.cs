using WebMvc.Condominio.Data;
using WebMvc.Condominio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebMvc.Condominio.Controllers
{
    [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
    public class VisitantesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public VisitantesController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        #region Lista e consulta
        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> ListarVisitantes(bool Saida)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);
            int codigo = _applicationUser.EmpresaId;

            if (!Saida)
            {
                var listVisitantes = await ListandoVisitantes(codigo);
                return View(listVisitantes);
            }
            else
            {
                ViewData["comDataSaida"] = Saida;
                var listImoveis = await ListaVisitantesComSaidaDoImovel(Saida, codigo);
                return View(listImoveis);
            }
        }
        #endregion

        #region Detalhes

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> DetalharVisitante(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Visitante não foi encontrado para visualizar os detalhes" });
            }
            var visitante = await _context.Visitante
                .FirstOrDefaultAsync(m => m.Id == id);
            var imovel = await _context.Imovel
                .FirstOrDefaultAsync(obj => obj.Id == visitante.ImovelId);

            if (visitante == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Visitante sem dados para visualização de detalhes" });
            }
            var viewModel = new ImovelViewModel
            {
                Visitante = visitante,
                Imovel = imovel
            };
            return View(viewModel);
        }

        #endregion

        #region Cadastro

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> CadastrarVisitante(int id)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);

            var imovel = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == id);

            var viewModel = new ImovelViewModel
            {
                Imovel = imovel
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastrarVisitante(Imovel imovel, Visitante visitante)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);

            if (!ModelState.IsValid)
            {
                var imovelReturn = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == imovel.Id);

                var viewModel = new ImovelViewModel
                {
                    Imovel = imovelReturn
                };
                return View(viewModel);
            }

            visitante.ImovelId = imovel.Id;
            visitante.EmpresaId = imovel.EmpresaId;

            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(visitante.FotoFile.FileName);
            string extension = Path.GetExtension(visitante.FotoFile.FileName);
            visitante.Foto = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/Imagens/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await visitante.FotoFile.CopyToAsync(fileStream);
            }

            _context.Add(visitante);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListarVisitantes));
        }

        #endregion

        #region Excluir

        [Authorize(Roles = "Suporte,AdminCond")]
        public async Task<IActionResult> ExcluirVisitante(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Visitante ão foi encontrada para deletar" });
            }
            var visitante = await _context.Visitante.FirstOrDefaultAsync(m => m.Id == id);
            var imovel = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == visitante.ImovelId);

            if (visitante == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Esse visitante esta vazio para deletar" });
            }
            var viewModel = new ImovelViewModel
            {
                Visitante = visitante,
                Imovel = imovel
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Suporte,AdminCond")]
        [HttpPost, ActionName("ExcluirVisitante")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirVisitanteConfirmed(int id)
        {
            var visitante = await _context.Visitante.FindAsync(id);
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Imagens", visitante.Foto);

            if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);

            _context.Visitante.Remove(visitante);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListarVisitantes));
        }

        #endregion

        #region Metodos especificos

        public async Task<List<Visitante>> ListandoVisitantes(int codigo)
        {
            var visitantes = from obj in _context.Visitante select obj;
            visitantes = visitantes.Where(x => x.EmpresaId == codigo)
                .Include(i => i.Imovel)
                .OrderByDescending(x => x.DataSaida == null)
                .ThenByDescending(x => x.DataSaida);

            visitantes = visitantes.Where(i => i.Imovel.DataSaida == null);
            return await visitantes.ToListAsync();
        }

        public async Task<List<Visitante>> ListaVisitantesComSaidaDoImovel(bool Saida, int codigo)
        {
            var visitantes = from obj in _context.Visitante select obj;
            visitantes = visitantes.Where(x => x.EmpresaId == codigo)
                .Include(i => i.Imovel)
                .OrderByDescending(x => x.DataSaida == null)
                .ThenByDescending(x => x.DataSaida);

            if (!Saida)
            {
                visitantes = visitantes.Where(i => i.Imovel.DataSaida == null);
            }
            else
            {
                visitantes = visitantes.Where(i => i.Imovel.DataSaida != null);
            }
            return await visitantes.ToListAsync();
        }

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> DataHoraEntrada(int? id)
        {
            var visitante = await _context.Visitante.FindAsync(id);
            if (visitante.DataSaida == null)
            {
                visitante.DataEntrada = DateTime.Now;

                _context.Update(visitante);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListarVisitantes));
        }

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> DataHoraSaida(int? id)
        {
            var visitante = await _context.Visitante.FindAsync(id);
            if (visitante.DataEntrada != null)
            {
                visitante.DataSaida = DateTime.Now;

                _context.Update(visitante);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListarVisitantes));
        }

        #endregion

        #region Metodos padrao

        private bool VisitanteExists(int id)
        {
            return _context.Visitante.Any(e => e.Id == id);
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
