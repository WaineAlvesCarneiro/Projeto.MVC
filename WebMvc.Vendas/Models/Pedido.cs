using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMvc.Vendas.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Display(Name = "Data pedido")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataPedido { get; set; }

        public int StatusId { get; set; }
        public Status Status { get; set; }

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        public int RcaId { get; set; }
        public Rca Rca { get; set; }

        [Display(Name = "Total")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalPedido { get; set; }

        public int Cont { get; set; }
    }
}
