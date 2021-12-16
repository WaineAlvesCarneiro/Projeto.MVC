using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WebMvc.Vendas.Data;
using WebMvc.Vendas.Models;

namespace WebMvc.Vendas.Controllers
{
    public class PedidoesController : Controller
    {
        private readonly VendasContext _context;

        public PedidoesController(VendasContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vendasContext = _context.Pedido.Include(p => p.Cliente).Include(p => p.Rca).Include(p => p.Status);
            return View(await vendasContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.Rca)
                .Include(p => p.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }
            var listProdutos = await ListItensIdAsync(pedido.Id);
            var viewModel = new PedidoViewModel
            {
                Pedido = pedido,
                Itemes = listProdutos
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var listCliente = await _context.Cliente.OrderBy(x => x.Nome).ToListAsync();
            var listRca = await _context.Rca.OrderBy(x => x.Nome).ToListAsync();
            var listStatus = await _context.Status.OrderBy(x => x.Id).ToListAsync();
            var viewModel = new PedidoViewModel
            {
                Clientees = listCliente,
                Rcaes = listRca,
                Statuses = listStatus
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pedido pedido)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pedido);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IncluirCreate), pedido);
            }
            return View();
        }

        public async Task<IActionResult> IncluirCreate(Pedido pedido)
        {
            var ClientePedido = await _context.Cliente.FirstOrDefaultAsync(obj => obj.Id == pedido.ClienteId);
            var rcaPedido = await _context.Rca.FirstOrDefaultAsync(obj => obj.Id == pedido.RcaId);
            var statusPedido = await _context.Status.FirstOrDefaultAsync(obj => obj.Id == pedido.StatusId);
            var listProdutos = await _context.Produto.OrderBy(x => x.Nome).ToListAsync();
            var listItems = await ListItemPedidoIdAsync(pedido.Id);
            var viewModel = new PedidoViewModel
            {
                Produtoes = listProdutos,
                ClienteList = ClientePedido,
                RcaList = rcaPedido,
                StatusList = statusPedido,
                Pedido = pedido,
                Itemes = listItems
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IncluirCreate(Pedido pedido, Item item, string responda)
        {
            if (ModelState.IsValid)
            {
                switch (responda)
                {
                    case "Adicionar":
                        item.PedidoId = pedido.Id;
                        pedido.Cont += 1;
                        item.Sequencia = pedido.Cont;
                        var valorProduto = await _context.Produto.FirstOrDefaultAsync(obj => obj.Id == item.ProdutoId);
                        item.PrecoItem = valorProduto.Preco;
                        item.TotalItem = item.QuantidadeItem* item.PrecoItem;

                        _context.Add(item);
                        await _context.SaveChangesAsync();

                        pedido.TotalPedido = TotalValorPedido(pedido);
                        _context.Update(pedido);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(IncluirCreate), pedido);

                    default:
                        return RedirectToAction(nameof(Index));
                }
            }
            return View();
        }

        public async Task<IActionResult> DeleteItemCreate(int? id)
        {
            var itemExcluir = await _context.Item.FirstOrDefaultAsync(obj => obj.Id == id.Value);
            Pedido pedido = await _context.Pedido.FirstOrDefaultAsync(obj => obj.Id == itemExcluir.PedidoId);

            _context.Item.Remove(itemExcluir);
            await _context.SaveChangesAsync();

            pedido.TotalPedido = TotalValorPedido(pedido);
            pedido.Cont -= 1;
            _context.Update(pedido);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IncluirCreate), pedido);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var pedido = await _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.Rca)
                .Include(p => p.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }
            var listItens = await ListItensIdAsync(pedido.Id);
            var listCliente = await _context.Cliente.OrderBy(x => x.Nome).ToListAsync();
            var listStatusOs = await _context.Status.OrderBy(x => x.NomeStatus).ToListAsync();
            var listRcaOs = await _context.Rca.OrderBy(x => x.Nome).ToListAsync();
            var listProdutos = await _context.Produto.OrderBy(x => x.Nome).ToListAsync();
            var viewModel = new PedidoViewModel
            {
                Pedido = pedido,
                Itemes = listItens,
                Clientees = listCliente,
                Statuses = listStatusOs,
                Rcaes = listRcaOs,
                Produtoes = listProdutos,
            };
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "Nome", pedido.ClienteId);
            ViewData["RcaId"] = new SelectList(_context.Rca, "Id", "Nome", pedido.RcaId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "NomeStatus", pedido.StatusId);
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pedido pedido)
        {
            if (id != pedido.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    pedido.TotalPedido = TotalValorPedido(pedido);
                    _context.Update(pedido);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PedidoExists(pedido.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Edit));
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "Id", "Nome", pedido.ClienteId);
            ViewData["RcaId"] = new SelectList(_context.Rca, "Id", "Nome", pedido.RcaId);
            ViewData["StatusId"] = new SelectList(_context.Status, "Id", "NomeStatus", pedido.StatusId);
            return View(pedido);
        }

        public async Task<IActionResult> DeleteItemEdit(int? id)
        {
            var itemExcluir = await _context.Item.FirstOrDefaultAsync(obj => obj.Id == id.Value);
            Pedido pedido = await _context.Pedido.FirstOrDefaultAsync(obj => obj.Id == itemExcluir.PedidoId);

            _context.Item.Remove(itemExcluir);
            await _context.SaveChangesAsync();

            pedido.TotalPedido = TotalValorPedido(pedido);
            pedido.Cont -= 1;
            _context.Update(pedido);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), pedido);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IncluirEdit(Pedido pedido, Item item, string responda)
        {
            if (ModelState.IsValid)
            {
                switch (responda)
                {
                    case "Adicionar":
                        item.PedidoId = pedido.Id;
                        pedido.Cont += 1;
                        item.Sequencia = pedido.Cont;
                        var valorProduto = await _context.Produto.FirstOrDefaultAsync(obj => obj.Id == item.ProdutoId);
                        item.PrecoItem = valorProduto.Preco;
                        item.TotalItem = item.QuantidadeItem * item.PrecoItem;

                        _context.Add(item);
                        await _context.SaveChangesAsync();

                        pedido.TotalPedido = TotalValorPedido(pedido);
                        _context.Update(pedido);
                        await _context.SaveChangesAsync();

                        return RedirectToAction(nameof(Edit), pedido);

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
                return NotFound();
            }

            var pedido = await _context.Pedido
                .Include(p => p.Cliente)
                .Include(p => p.Rca)
                .Include(p => p.Status)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pedido == null)
            {
                return NotFound();
            }
            var listProdutos = await ListItensIdAsync(pedido.Id);
            var viewModel = new PedidoViewModel
            {
                Pedido = pedido,
                Itemes = listProdutos
            };
            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pedido = await _context.Pedido.FindAsync(id);
            _context.Pedido.Remove(pedido);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PedidoExists(int id)
        {
            return _context.Pedido.Any(e => e.Id == id);
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

        public async Task<List<Item>> ListItemPedidoIdAsync(int? id)
        {
            var result = from obj in _context.Item select obj;
            result = result.Where(x => x.PedidoId == id);
            return await result.ToListAsync();
        }

        public decimal TotalValorPedido(Pedido pedido)
        {
            var result = from obj in _context.Item select obj;
            return pedido.TotalPedido = result.Where(x => x.PedidoId == pedido.Id).Sum(selector: x => x.TotalItem);
        }

        public async Task<List<Item>> ListItensIdAsync(int? id)
        {
            var result = from obj in _context.Item select obj;
            result = result.Where(x => x.PedidoId == id).Include(o => o.Produto);
            return await result.ToListAsync();
        }
    }
}