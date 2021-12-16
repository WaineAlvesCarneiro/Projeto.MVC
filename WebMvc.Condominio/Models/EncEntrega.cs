using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMvc.Condominio.Models
{
    public class EncEntrega
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public int ImovelId { get; set; }
        public Imovel Imovel { get; set; }
        public int EncRecebidaId { get; set; }

        [Display(Name = "Morador recebeu")]
        [Required(ErrorMessage = "Morador que recebeu é obrigatório")]
        public int MoradorId { get; set; }

        [Display(Name = "Doc. morador")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(30, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 3)]
        public string DocumentoDoMorador { get; set; }

        [Display(Name = "Data entrega")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime DataEntrega { get; set; }

        [NotMapped]
        [DisplayName("Foto")]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        public IFormFile FotoFile { get; set; }
        public string Foto { get; set; } //varchar(100)
    }
}
