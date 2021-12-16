using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMvc.Condominio.Models
{
    public class Visitante
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public int ImovelId { get; set; }
        public Imovel Imovel { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(50, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 3)]
        public string Nome { get; set; }

        [Display(Name = "Documento")]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        [StringLength(30, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 3)]
        public string Documento { get; set; }

        [Display(Name = "Data entrada")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? DataEntrada { get; set; }

        [Display(Name = "Data saída")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? DataSaida { get; set; }

        [NotMapped]
        [DisplayName("Foto")]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        public IFormFile FotoFile { get; set; }
        public string Foto { get; set; } //varchar(100)
    }
}
