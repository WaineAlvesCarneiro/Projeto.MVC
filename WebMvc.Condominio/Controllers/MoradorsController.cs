using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebMvc.Condominio.Data;
using WebMvc.Condominio.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;

namespace WebMvc.Condominio.Controllers
{
    [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
    public class MoradorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public MoradorsController(ApplicationDbContext context,
                                UserManager<ApplicationUser> userManager,
                                IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        #region Lista e consulta
        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> ListarMoradores(string morador, bool Saida)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);
            int codigo = _applicationUser.EmpresaId;

			if (morador == null && !Saida)
			{
                var listMoradores = await LocalizaTodosMoradores(codigo);
                return View(listMoradores);
            }
			else
			{
                ViewData["morador"] = morador;
                ViewData["comDataSaida"] = Saida;
                var listMoradoes = await LocalizaMorador(morador, Saida, codigo);
                ViewData["morador"] = null;
                return View(listMoradoes);
            }
        }
        #endregion

        #region Cadastro

        [Authorize(Roles = "Suporte,AdminCond")]
        public async Task<IActionResult> CadastrarMoradores(int id)
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

        [Authorize(Roles = "Suporte,AdminCond")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastrarMoradores(Imovel imovel, Morador morador, string responda)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);
            int codigo = _applicationUser.EmpresaId;

            if (!ModelState.IsValid)
            {
                var listMoradores = await ListMoradoresImovelIdAsync(imovel.Id, morador.EmpresaId);
                var imovelReturn = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == imovel.Id);

                var viewModel = new ImovelViewModel
                {
                    Imovel = imovelReturn,
                    Moradores = listMoradores
                };
                return View(viewModel);
            }

            switch (responda)
            {
                case "Adicionar":
                    morador.ImovelId = imovel.Id;
                    morador.EmpresaId = imovel.EmpresaId;

                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(morador.FotoFile.FileName);
                    string extension = Path.GetExtension(morador.FotoFile.FileName);
                    morador.Foto = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
                    string path = Path.Combine(wwwRootPath + "/Imagens/", fileName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await morador.FotoFile.CopyToAsync(fileStream);
                    }

                    _context.Add(morador);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(CadastrarMoradores), imovel);

                default:
                    return RedirectToAction(nameof(CadastrarMoradores), imovel);
            }
        }

        #endregion

        #region Excluir

        [Authorize(Roles = "Suporte,AdminCond")]
        public async Task<IActionResult> ExcluirMorador(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Morador não foi encontrado para deletar" });
            }
            var morador = await _context.Morador.FirstOrDefaultAsync(m => m.Id == id);
            var imovel = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == morador.ImovelId);

            if (morador == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Esse morador esta vazio para deletar" });
            }
            var viewModel = new ImovelViewModel
            {
                Morador = morador,
                Imovel = imovel
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Suporte,AdminCond")]
        [HttpPost, ActionName("ExcluirMorador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirMoradorConfirmed(int id)
        {
            var morador = await _context.Morador.FindAsync(id);
            Imovel imovel = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == morador.ImovelId);

            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Imagens", morador.Foto);
            if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);

            _context.Morador.Remove(morador);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CadastrarMoradores), imovel);
        }

        #endregion

        #region Metodos especificos

        public async Task<List<Morador>> LocalizaTodosMoradores(int codigo)
        {
            var moradores = from obj in _context.Morador select obj;
            moradores = moradores.Where(x => x.EmpresaId == codigo)
                .Include(i => i.Imovel);

            moradores = moradores.Where(i => i.Imovel.DataSaida == null)
                .OrderByDescending(i => i.Imovel.DataEntrada);
            return await moradores.ToListAsync();
        }

        public async Task<List<Morador>> LocalizaMorador(string pessoa, bool Saida, int codigo)
        {
            var moradores = from obj in _context.Morador select obj;
            moradores = moradores.Where(x => x.Nome.Contains(pessoa) && x.EmpresaId == codigo)
                .Include(i => i.Imovel);

            if (!Saida)
            {
                moradores = moradores.Where(i => i.Imovel.DataSaida == null)
                .OrderByDescending(i => i.Imovel.DataEntrada);
            }
            else
            {
                moradores = moradores.Where(i => i.Imovel.DataSaida != null)
                .OrderByDescending(i => i.Imovel.DataEntrada);
            }
            return await moradores.ToListAsync();
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

        private bool MoradorExists(int id)
        {
            return _context.Morador.Any(e => e.Id == id);
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
