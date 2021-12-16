using System.Collections.Generic;

namespace WebMvc.Vendas.Models.ViewModels
{
    public class ProdutoViewModel
    {
        public Produto Produto { get; set; }
        public ICollection<Fornecedor> FornecedorProduto { get; set; }
    }
}
