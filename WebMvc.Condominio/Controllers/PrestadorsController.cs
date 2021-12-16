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
    public class PrestadorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrestadorsController(ApplicationDbContext context,
                                    UserManager<ApplicationUser> userManager,
                                    IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        #region Lista e consulta
        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> ListarPrestador(bool Saida)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);
            int codigo = _applicationUser.EmpresaId;

            if (!Saida)
            {
                var listPrestadores = await ListarPrestadores(codigo);
                return View(listPrestadores);
            }
            else
            {
                ViewData["comDataSaida"] = Saida;
                var listImoveis = await ListaPrestadoresComSaidaDoImovel(Saida, codigo);
                return View(listImoveis);
            }
        }
        #endregion

        #region Detalhes

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> DetalharPrestador(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Prestador de serviço não foi encontrado para visualizar os detalhes" });
            }
            var prestador = await _context.Prestador
                .FirstOrDefaultAsync(m => m.Id == id);
            var imovel = await _context.Imovel
                .FirstOrDefaultAsync(obj => obj.Id == prestador.ImovelId);

            if (prestador == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Prestador de serviço sem dados para visualização de detalhes" });
            }
            var viewModel = new ImovelViewModel
            {
                Prestador = prestador,
                Imovel = imovel
            };
            return View(viewModel);
        }

        #endregion

        #region Cadastro

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> CadastrarPrestador(int id)
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
        public async Task<IActionResult> CadastrarPrestador(Imovel imovel, Prestador prestador)
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

            prestador.ImovelId = imovel.Id;
            prestador.EmpresaId = imovel.EmpresaId;

            string wwwRootPath = _hostEnvironment.WebRootPath;
            string fileName = Path.GetFileNameWithoutExtension(prestador.FotoFile.FileName);
            string extension = Path.GetExtension(prestador.FotoFile.FileName);
            prestador.Foto = fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;
            string path = Path.Combine(wwwRootPath + "/Imagens/", fileName);
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await prestador.FotoFile.CopyToAsync(fileStream);
            }

            _context.Add(prestador);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ListarPrestador));
        }

        #endregion

        #region Excluir

        [Authorize(Roles = "Suporte,AdminCond")]
        public async Task<IActionResult> ExcluirPrestador(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Prestador de serviço não foi encontrada para deletar" });
            }
            var prestador = await _context.Prestador.FirstOrDefaultAsync(m => m.Id == id);
            var imovel = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == prestador.ImovelId);

            if (prestador == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Esse prestador de serviço esta vazio para deletar" });
            }
            var viewModel = new ImovelViewModel
            {
                Prestador = prestador,
                Imovel = imovel
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Suporte,AdminCond")]
        [HttpPost, ActionName("ExcluirPrestador")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirPrestadorConfirmed(int id)
        {
            var prestador = await _context.Prestador.FindAsync(id);
            var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Imagens", prestador.Foto);

            if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);

            _context.Prestador.Remove(prestador);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListarPrestador));
        }

        #endregion

        #region Metodos especificos

        public async Task<List<Prestador>> ListarPrestadores(int codigo)
        {
            var prestadores = from obj in _context.Prestador select obj;
            prestadores = prestadores.Where(x => x.EmpresaId == codigo)
                .Include(i => i.Imovel)
                .OrderByDescending(x => x.DataHoraSaida == null)
                .ThenByDescending(x => x.DataHoraSaida);

            prestadores = prestadores.Where(i => i.Imovel.DataSaida == null);
            return await prestadores.ToListAsync();
        }

        public async Task<List<Prestador>> ListaPrestadoresComSaidaDoImovel(bool Saida, int codigo)
        {
            var prestadores = from obj in _context.Prestador select obj;
            prestadores = prestadores.Where(x => x.EmpresaId == codigo)
                .Include(i => i.Imovel)
                .OrderByDescending(x => x.DataHoraSaida == null)
                .ThenByDescending(x => x.DataHoraSaida);

            if (!Saida)
            {
                prestadores = prestadores.Where(i => i.Imovel.DataSaida == null);
            }
            else
            {
                prestadores = prestadores.Where(i => i.Imovel.DataSaida != null);
            }
            return await prestadores.ToListAsync();
        }

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> DataHoraEntrada(int? id)
        {
            var prestador = await _context.Prestador.FindAsync(id);
            if (prestador.DataHoraSaida == null)
            {
                prestador.DataHoraEntrada = DateTime.Now;

                _context.Update(prestador);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListarPrestador));
        }

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> DataHoraSaida(int? id)
        {
            var prestador = await _context.Prestador.FindAsync(id);
            if (prestador.DataHoraEntrada != null)
            {
                prestador.DataHoraSaida = DateTime.Now;

                _context.Update(prestador);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(ListarPrestador));
        }

        #endregion

        #region Metodos padrao
        private bool PrestadorExists(int id)
        {
            return _context.Prestador.Any(e => e.Id == id);
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
