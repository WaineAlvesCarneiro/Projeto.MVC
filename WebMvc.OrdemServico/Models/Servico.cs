using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMvc.OrdemServico.Models
{
    public class Servico
    {
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        [StringLength(30, ErrorMessage = "O campo {0} deve ter entre {2} e {1} numeros", MinimumLength = 3)]
        public string NomeServico { get; set; }

        [Display(Name = "Preço")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Preco { get; set; }
    }
}
