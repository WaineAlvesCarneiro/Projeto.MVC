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
    public class EncEntregasController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public EncEntregasController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        #region Lista e consulta
        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> ListarEntregas(bool Saida)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);
            int codigo = _applicationUser.EmpresaId;

            if (!Saida)
            {
                var listEncEntregue = await ListarTodasEntregas(codigo);
                return View(listEncEntregue);
            }
            else
            {
                ViewData["comDataSaida"] = Saida;
                var listImoveis = await ListaEntregasComSaidaDoImovel(Saida, codigo);
                return View(listImoveis);
            }
        }
        #endregion

        #region Detalhes

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> DetalharEntrega(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Encomenda Entregue não foi encontrada para visualizar os detalhes" });
            }
            var encEntrega = await _context.EncEntrega.FirstOrDefaultAsync(m => m.Id == id);
            var encRecebida = await _context.EncRecebida.FirstOrDefaultAsync(m => m.Id == encEntrega.EncRecebidaId);

            //busca o morador que esta descrito na encomenda e conta na tabela da encomenda recebida
            var moradorNaEncomenda = await _context.Morador.FirstOrDefaultAsync(obj => obj.Id == encRecebida.MoradorId);
            var imovel = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == encEntrega.ImovelId);

            //busca o morador que RECEBEU a encomenda na portaria e conta na tabela da encomenda Entregue
            var moradorRecebeuEntrega = await _context.Morador.FirstOrDefaultAsync(obj => obj.Id == encEntrega.MoradorId);

            if (encEntrega == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Encomenda Entregue sem dados para visualização de detalhes" });
            }
            var viewModel = new ImovelViewModel
            {
                EncEntrega = encEntrega,
                EncRecebida = encRecebida,
                Imovel = imovel,
                MoradorRecebeuEntrega = moradorRecebeuEntrega,
                Morador = moradorNaEncomenda
            };
            return View(viewModel);
        }

        #endregion

        #region Cadastro

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> CadastrarEntrega(int id)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);
            int codigo = _applicationUser.EmpresaId;

            var encRecebida = await _context.EncRecebida.FirstOrDefaultAsync(m => m.Id == id);
            var imovel = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == encRecebida.ImovelId);
            var moradorNaEncomenda = await _context.Morador.FirstOrDefaultAsync(obj => obj.Id == encRecebida.MoradorId);
            var listMoradores = await ListMoradoresImovelIdAsync(imovel.Id, codigo);

            var viewModel = new ImovelViewModel
            {
                Moradores = listMoradores,
                EncRecebida = encRecebida,
                Imovel = imovel,
                Morador = moradorNaEncomenda,
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastrarEntrega(EncRecebida encRecebida, EncEntrega encEntrega)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);
            int codigo = _applicationUser.EmpresaId;

            if (!ModelState.IsValid)
            {
                var imovelReturn = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == encRecebida.ImovelId);
                var moradorNaEncomenda = await _context.Morador.FirstOrDefaultAsync(obj => obj.Id == encRecebida.MoradorId);
                var listMoradores = await ListMoradoresImovelIdAsync(imovelReturn.Id, codigo);

                var viewModel = new ImovelViewModel
                {
                    Moradores = listMoradores,
                    EncRecebida = encRecebida,
                    Imovel = imovelReturn,
                    Morador = moradorNaEncomenda,
                };
                return View(viewModel);
            }

            encEntrega.ImovelId = encRecebida.ImovelId;
            encEntrega.EmpresaId = encRecebida.EmpresaId;
            encEntrega.EncRecebidaId = encRecebida.Id;

            encEntrega.DataEntrega = DateTime.Now;

            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(encEntrega.FotoFile.FileName);
            string extension = Path.GetExtension(encEntrega.FotoFile.FileName);
            encEntrega.Foto = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/Imagens/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await encEntrega.FotoFile.CopyToAsync(fileStream);
            }

            _context.Add(encEntrega);
            await _context.SaveChangesAsync();

            encRecebida.Entregue_Sim_Nao = "Sim";
            _context.Update(encRecebida);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListarEntregas));
        }

        #endregion

        #region Excluir

        [Authorize(Roles = "Suporte,AdminCond")]
        public async Task<IActionResult> ExcluirEntrega(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Encomenda Recebida não foi encontrada para deletar" });
            }
            var encEntrega = await _context.EncEntrega.FirstOrDefaultAsync(m => m.Id == id);
            var encRecebida = await _context.EncRecebida.FirstOrDefaultAsync(m => m.Id == encEntrega.EncRecebidaId);

            //busca o morador que esta descrito na encomenda e conta na tabela da encomenda recebida
            var moradorNaEncomenda = await _context.Morador.FirstOrDefaultAsync(obj => obj.Id == encRecebida.MoradorId);
            var imovel = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == encEntrega.ImovelId);

            //busca o morador que RECEBEU a encomenda na portaria e conta na tabela da encomenda Entregue
            var moradorRecebeuEntrega = await _context.Morador.FirstOrDefaultAsync(obj => obj.Id == encEntrega.MoradorId);

            if (encEntrega == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Essa Encomenda Recebida esta vazio para deletar" });
            }
            var viewModel = new ImovelViewModel
            {
                EncEntrega = encEntrega,
                EncRecebida = encRecebida,
                Imovel = imovel,
                MoradorRecebeuEntrega = moradorRecebeuEntrega,
                Morador = moradorNaEncomenda
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Suporte,AdminCond")]
        [HttpPost, ActionName("ExcluirEntrega")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirEntregaConfirmed(int id)
        {
            var encEntrega = await _context.EncEntrega.FindAsync(id);
            var encRecebida = await _context.EncRecebida.FirstOrDefaultAsync(m => m.Id == encEntrega.EncRecebidaId);

            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Imagens", encEntrega.Foto);
            if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);

            _context.EncEntrega.Remove(encEntrega);
            await _context.SaveChangesAsync();

            if (encRecebida != null)
            {
                _context.EncRecebida.Remove(encRecebida);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListarEntregas));
        }

        #endregion

        #region Metodos especificos

        public async Task<List<EncEntrega>> ListarTodasEntregas(int codigo)
        {
            var entregas = from obj in _context.EncEntrega select obj;
            entregas = entregas.Where(x => x.EmpresaId == codigo)
                .Include(j => j.Imovel)
                .OrderByDescending(x => x.DataEntrega);

            entregas = entregas.Where(i => i.Imovel.DataSaida == null);
            return await entregas.ToListAsync();
        }

        public async Task<List<EncEntrega>> ListaEntregasComSaidaDoImovel(bool Saida, int codigo)
        {
            var entregas = from obj in _context.EncEntrega select obj;
            entregas = entregas.Where(x => x.EmpresaId == codigo)
                .Include(j => j.Imovel)
                .OrderByDescending(x => x.DataEntrega);

            if (!Saida)
            {
                entregas = entregas.Where(i => i.Imovel.DataSaida == null);
            }
            else
            {
                entregas = entregas.Where(i => i.Imovel.DataSaida != null);
            }
            return await entregas.ToListAsync();
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

        private bool EncEntregaExists(int id)
        {
            return _context.EncEntrega.Any(e => e.Id == id);
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
