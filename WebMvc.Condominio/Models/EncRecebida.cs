using System;
using System.ComponentModel.DataAnnotations;

namespace WebMvc.Condominio.Models
{
    public class EncRecebida
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public int ImovelId { get; set; }
        public Imovel Imovel { get; set; }

        [Display(Name = "Morador encomenda")]
        [Required(ErrorMessage = "Morador na encomenda é obrigatório")]
        public int MoradorId { get; set; }

        [Display(Name = "Código Rastreio")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(30, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 3)]
        public string CodigoRastreio { get; set; }

        [Display(Name = "Tipo encomenda")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(30, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 3)]
        public string TipoEncomenda { get; set; }

        [Display(Name = "Data recebida")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime DataRecebimento { get; set; }

        [Display(Name = "Entregue?")]
        public string Entregue_Sim_Nao { get; set; } //varchar(4)
    }
}
