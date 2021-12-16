using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMvc.Vendas.Models
{
    public class Item
    {
        public int Id { get; set; }

        public int Sequencia { get; set; }
        
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; }

        [Display(Name = "Qtde.")]
        public int QuantidadeItem { get; set; }

        [Display(Name = "Preço item")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal PrecoItem { get; set; }

        [Display(Name = "Preço total")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalItem { get; set; }
    }
}
