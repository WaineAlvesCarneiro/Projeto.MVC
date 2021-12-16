using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMvc.Vendas.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        [StringLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string Nome { get; set; }

        [Display(Name = "Preço")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Preco { get; set; }

        public int FornecedorId { get; set; }
        public Fornecedor Fornecedores { get; set; }
    }
}
