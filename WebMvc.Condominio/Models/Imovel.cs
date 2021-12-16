using System;
using System.ComponentModel.DataAnnotations;

namespace WebMvc.Condominio.Models
{
    public class Imovel
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }

        [Display(Name = "Bloco/Quadra")]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        [StringLength(10, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 1)]
        public string BlocoQuadra { get; set; }

        [Display(Name = "Apto/Lote")]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        [StringLength(10, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 1)]
        public string AptoLote { get; set; }

        [Required(ErrorMessage = "{0} é obrigatorio")]
        [StringLength(10, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 1)]
        public string Box { get; set; }

        [Display(Name = "Tipo")]
        public int Tipo { get; set; }

        [Display(Name = "Data entrada")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DataEntrada { get; set; }

        [Display(Name = "Data saída")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? DataSaida { get; set; }

        [MaxLength(20, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string Marca { get; set; }

        [MaxLength(20, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string Modelo { get; set; }

        [MaxLength(20, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string Cor { get; set; }

        [MaxLength(20, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string Placa { get; set; }

        [Display(Name = "Nome parente")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string ParenteNome { get; set; }

        [Display(Name = "Celular parente")]
        [MaxLength(16, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string ParenteCelular { get; set; }

        [Display(Name = "Telefone parente")]
        [MaxLength(16, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string ParenteTelefone { get; set; }
    }
}
