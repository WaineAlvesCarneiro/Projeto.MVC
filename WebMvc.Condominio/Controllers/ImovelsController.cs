using WebMvc.Condominio.Data;
using WebMvc.Condominio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebMvc.Condominio.Controllers
{
    [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
    public class ImovelsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public ImovelsController(ApplicationDbContext context,
                                UserManager<ApplicationUser> userManager,
                                IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _hostEnvironment = hostEnvironment;
        }

        #region Lista e consulta
        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> ListarImoveis(string BlocoQuadra, string AptoLote, bool Saida)
        {
            ApplicationUser _applicationUser = await _userManager.GetUserAsync(User);
            int codigo = _applicationUser.EmpresaId;

            if (BlocoQuadra == null && AptoLote == null && !Saida)
            {
                var listImoveisTodos = await LocalizaTodosImoveis(codigo);
                return View(listImoveisTodos);
            }
            else
            {
                ViewData["blocoquadra"] = BlocoQuadra;
                ViewData["aptolote"] = AptoLote;
                ViewData["comDataSaida"] = Saida;
                var listImoveis = await LocalizaImovel(BlocoQuadra, AptoLote, Saida, codigo);
                return View(listImoveis);
            }
        }
        #endregion

        #region Detalhes

        [Authorize(Roles = "Suporte,AdminCond,UsuarioCond")]
        public async Task<IActionResult> DetalharImovel(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Imovel não foi encontrado para visualizar os detalhes" });
            }
            var imovel = await _context.Imovel.FirstOrDefaultAsync(m => m.Id == id);
            var listMoradores = await ListMoradoresImovelIdAsync(imovel.Id);
            var viewModel = new ImovelViewModel
            {
                Imovel = imovel,
                Moradores = listMoradores
            };
            return View(viewModel);
        }

        #endregion

        #region Cadastro

        [Authorize(Roles = "Suporte,AdminCond")]
        public IActionResult CadastrarImovel()
        {
            return View();
        }

        [Authorize(Roles = "Suporte,AdminCond")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CadastrarImovel(Imovel imovel)
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

            imovel.EmpresaId = _applicationUser.EmpresaId;

            _context.Add(imovel);
            await _context.SaveChangesAsync();

            var imovelId = await _context.Imovel.FirstOrDefaultAsync(m => m.Id == imovel.Id);
            return RedirectToAction(nameof(ListarImoveis));
        }

        #endregion

        #region Editar

        [Authorize(Roles = "Suporte,AdminCond")]
        public async Task<IActionResult> EditarImovel(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Imóvel não foi encontrado para editar" });
            }

            var imovelReturn = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == id);
            if (imovelReturn == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Esse imóvel esta vazio para editar" });
            }

            var viewModel = new ImovelViewModel
            {
                Imovel = imovelReturn
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Suporte,AdminCond")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarImovel(int id, Imovel imovel)
        {
            if (!ModelState.IsValid)
            {
                var imovelReturn = await _context.Imovel.FirstOrDefaultAsync(obj => obj.Id == imovel.Id);

                var viewModel = new ImovelViewModel
                {
                    Imovel = imovelReturn
                };
                return View(viewModel);
            }
            if (id != imovel.Id)
            {
                return RedirectToAction(nameof(Error), new { message = "Imóvel não foi encontrado para editar" });
            }

            try
            {
                _context.Update(imovel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(DetalharImovel), imovel);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ImovelExists(imovel.Id))
                {
                    return RedirectToAction(nameof(Error), new { message = "Esse imóvel não foi encontrado para salvar/update a edição" });
                }
                else
                {
                    throw;
                }
            }
        }

        #endregion

        #region Excluir

        [Authorize(Roles = "Suporte,AdminCond")]
        public async Task<IActionResult> ExcluirImovel(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Imovel não foi encontrada para deletar" });
            }
            var imovel = await _context.Imovel.FirstOrDefaultAsync(m => m.Id == id);
            if (imovel == null)
            {
                return RedirectToAction(nameof(Error), new { message = "Esse Imovel esta vazio para deletar" });
            }
            var listMoradores = await ListMoradoresImovelIdAsync(imovel.Id);
            var viewModel = new ImovelViewModel
            {
                Imovel = imovel,
                Moradores = listMoradores
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Suporte,AdminCond")]
        [HttpPost, ActionName("ExcluirImovel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExcluirImovelConfirmed(int id)
        {
            //Ao excluir imóvel será excluida todas as imagens de todos os moradores
            bool resultMorador = true;
            do
            {
                var morador = await _context.Morador.FirstOrDefaultAsync(obj => obj.ImovelId == id);
                if (morador != null)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Imagens", morador.Foto);
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);

                    _context.Morador.Remove(morador);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    resultMorador = false;
                }
            } while (resultMorador == true);

            //Ao excluir imóvel será excluida todas as imagens de todos os visitantes
            bool resultVisitante = true;
            do
            {
                var visitante = await _context.Visitante.FirstOrDefaultAsync(obj => obj.ImovelId == id);
                if (visitante != null)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Imagens", visitante.Foto);
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);

                    _context.Visitante.Remove(visitante);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    resultVisitante = false;
                }
            } while (resultVisitante == true);

            //Ao excluir imóvel será excluida todas as imagens de todos os prestadores de serviço
            bool resultPrestador = true;
            do
            {
                var prestador = await _context.Prestador.FirstOrDefaultAsync(obj => obj.ImovelId == id);
                if (prestador != null)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Imagens", prestador.Foto);
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);

                    _context.Prestador.Remove(prestador);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    resultPrestador = false;
                }
            } while (resultPrestador == true);

            //Ao excluir imóvel será excluida todas as imagens de todas as entregas
            bool resultEntregaFoto = true;
            do
            {
                var encEntrega = await _context.EncEntrega.FirstOrDefaultAsync(obj => obj.ImovelId == id);
                if (encEntrega != null)
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, "Imagens", encEntrega.Foto);
                    if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);

                    _context.EncEntrega.Remove(encEntrega);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    resultEntregaFoto = false;
                }
            } while (resultEntregaFoto == true);

            //Excluir o imóvel e restorna para lista de imóveis
            var imovel = await _context.Imovel.FindAsync(id);

            _context.Imovel.Remove(imovel);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ListarImoveis));
        }

        #endregion

        #region Metodos especificos

        public async Task<List<Imovel>> LocalizaTodosImoveis(int codigo)
        {
            var imoveis = from obj in _context.Imovel select obj;
            imoveis = imoveis.Where(x => x.EmpresaId == codigo);

            imoveis = imoveis.Where(i => i.DataSaida == null)
                .OrderByDescending(x => x.DataEntrada);
            return await imoveis.ToListAsync();
        }

        public async Task<List<Imovel>> LocalizaImovel(string BlocoQuadra, string AptoLote, bool Saida, int codigo)
        {
            var imoveis = from obj in _context.Imovel select obj;
            imoveis = imoveis.Where(x => x.BlocoQuadra.Contains(BlocoQuadra)
                && x.AptoLote.Contains(AptoLote) && x.EmpresaId == codigo);

            if (!Saida)
            {
                imoveis = imoveis.Where(i => i.DataSaida == null)
                .OrderByDescending(x => x.DataEntrada);
            }
            else
            {
                imoveis = imoveis.Where(i => i.DataSaida != null)
                .OrderByDescending(x => x.DataEntrada);
            }
            return await imoveis.ToListAsync();
        }

        public async Task<List<Morador>> ListMoradoresImovelIdAsync(int? id)
        {
            var morador = from obj in _context.Morador select obj;
            morador = morador.Where(x => x.ImovelId == id);
            return await morador.ToListAsync();
        }

        #endregion

        #region Metodos padrao

        private bool ImovelExists(int id)
        {
            return _context.Imovel.Any(e => e.Id == id);
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
