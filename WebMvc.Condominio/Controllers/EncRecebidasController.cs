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
    public class EncRecebidasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public EncRecebidasController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        #region Lista e consulta
        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> ListarRecebidas(bool Saida)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);
            int codigo = _applicationUser.EmpresaId;

            if (!Saida)
            {
                var listEncRecebida = await ListarTodasRecebidas(codigo);
                return View(listEncRecebida);
            }
            else
            {
                ViewData["comDataSaida"] = Saida;
                var listImoveis = await ListaRecebidasComSaidaDoImovel(Saida, codigo);
                return View(listImoveis);
            }
        }
        #endregion

        #region Detalhes

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> DetalharRecebida(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Encomenda Entregue não foi encontrada para visualizar os detalhes" });
            }
            var encRecebida = await _context.EncRecebida.FirstOrDefaultAsync(m => m.Id == id);
            var imovel = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == encRecebida.ImovelId);
            var morador = await _context.Morador.FirstOrDefaultAsync(obj => obj.Id == encRecebida.MoradorId);

            if (encRecebida == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Essa Encomenda Recebida esta vazio para deletar" });
            }
            var viewModel = new ImovelViewModel
            {
                EncRecebida = encRecebida,
                Imovel = imovel,
                Morador = morador
            };
            return View(viewModel);
        }

        #endregion

        #region Cadastro

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> CadastrarRecebida(int id)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);
            int codigo = _applicationUser.EmpresaId;

            var imovel = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == id);
            var listMoradores = await ListMoradoresImovelIdAsync(imovel.Id, codigo);

            var viewModel = new ImovelViewModel
            {
                Imovel = imovel,
                Moradores = listMoradores
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastrarRecebida(Imovel imovel, EncRecebida encRecebida)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);
            int codigo = _applicationUser.EmpresaId;

            if (!ModelState.IsValid)
            {
                var imovelReturn = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == imovel.Id);
                var listMoradores = await ListMoradoresImovelIdAsync(imovel.Id, codigo);

                var viewModel = new ImovelViewModel
                {
                    Imovel = imovelReturn,
                    Moradores = listMoradores,
                    EncRecebida = encRecebida
                };
                return View(viewModel);
            }

            encRecebida.ImovelId = imovel.Id;
            encRecebida.EmpresaId = imovel.EmpresaId;

            encRecebida.DataRecebimento = DateTime.Now;
            encRecebida.Entregue_Sim_Nao = "Não";

            _context.Add(encRecebida);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListarRecebidas));
        }

        #endregion

        #region Excluir

        [Authorize(Roles = "Suporte,AdminCond")]
        public async Task<IActionResult> ExcluirRecebida(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Encomenda Recebida não foi encontrada para deletar" });
            }
            var encRecebida = await _context.EncRecebida.FirstOrDefaultAsync(m => m.Id == id);
            var imovel = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == encRecebida.ImovelId);
            var morador = await _context.Morador.FirstOrDefaultAsync(obj => obj.Id == encRecebida.MoradorId);
            if (encRecebida == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Essa Encomenda Recebida esta vazio para deletar" });
            }
            var viewModel = new ImovelViewModel
            {
                EncRecebida = encRecebida,
                Imovel = imovel,
                Morador = morador
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Suporte,AdminCond")]
        [HttpPost, ActionName("ExcluirRecebida")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirRecebidaConfirmed(int id)
        {
            var encRecebida = await _context.EncRecebida.FindAsync(id);
            var encEntrega = await _context.EncEntrega.FirstOrDefaultAsync(m => m.EncRecebidaId == encRecebida.Id);

            _context.EncRecebida.Remove(encRecebida);
            await _context.SaveChangesAsync();

            if (encEntrega != null)
            {
                var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Imagens", encEntrega.Foto);
                if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);

                _context.EncEntrega.Remove(encEntrega);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListarRecebidas));
        }

        #endregion

        #region Metodos especificos

        public async Task<List<EncRecebida>> ListarTodasRecebidas(int codigo)
        {
            var recebidas = from obj in _context.EncRecebida select obj;
            recebidas = recebidas.Where(x => x.EmpresaId == codigo)
                .Include(j => j.Imovel)
                .OrderByDescending(i => i.DataRecebimento);

            recebidas = recebidas.Where(i => i.Imovel.DataSaida == null);
            return await recebidas.ToListAsync();
        }

        public async Task<List<EncRecebida>> ListaRecebidasComSaidaDoImovel(bool Saida, int codigo)
        {
            var recebidas = from obj in _context.EncRecebida select obj;
            recebidas = recebidas.Where(x => x.EmpresaId == codigo)
                .Include(j => j.Imovel)
                .OrderByDescending(i => i.DataRecebimento);

            if (!Saida)
            {
                recebidas = recebidas.Where(i => i.Imovel.DataSaida == null);
            }
            else
            {
                recebidas = recebidas.Where(i => i.Imovel.DataSaida != null);
            }
            return await recebidas.ToListAsync();
        }

        public async Task<List<Morador>> ListMoradoresImovelIdAsync(int idImovel, int codigo)
        {
            var moradores = from obj in _context.Morador select obj;
            moradores = moradores.Where(x => x.ImovelId == idImovel && x.EmpresaId == codigo)
                .OrderBy(x => x.Nome);

            return await moradores.ToListAsync();
        }

        #endregion

        #region Metodos padrao

        private bool EncRecebidaExists(int id)
        {
            return _context.EncRecebida.Any(e => e.Id == id);
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
