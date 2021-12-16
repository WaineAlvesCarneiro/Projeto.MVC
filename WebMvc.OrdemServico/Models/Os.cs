using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMvc.OrdemServico.Models
{
    public class Os
    {
        public int Id { get; set; }

        [Display(Name = "Data abertura")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DtAbertura { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [Display(Name = "Total")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalOs { get; set; }

        [Display(Name = "Data conclusão")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DtConcluido { get; set; }
    }
}
