using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMvc.OrdemServico.Models
{
    public class Prestado
    {
        public int Id { get; set; }
        public int OsId { get; set; }
        public Os Os { get; set; }
        public int ServicoId { get; set; }
        public Servico Servico { get; set; }

        [Display(Name = "Qtde.")]
        public int QtdeServico { get; set; }

        [Display(Name = "Valor item")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal ValorItem { get; set; }

        [Display(Name = "Total")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalItem { get; set; }
    }
}
