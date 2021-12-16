using System;
using System.ComponentModel.DataAnnotations;
using static WebMvc.CadastroPessoa.Validacao.CustomValidation;

namespace WebMvc.CadastroPessoa.Models
{
	public class Pessoa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Nome { get; set; }

        [Display(Name = "CPF/CNPJ")]
        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(18, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 11)]
        [CpfCnpjValido]
        public string CpfCnpj { get; set; }

        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(16, ErrorMessage = "O campo {0} precisa ter {1} caracteres", MinimumLength = 11)]
        public string Celular { get; set; }

        [StringLength(16, ErrorMessage = "O campo {0} precisa ter {1} caracteres", MinimumLength = 11)]
        public string Telefone { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string Email { get; set; }

        [Display(Name = "Data aniversário")]
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        [Min18Anos]
        public DateTime DtAniversario { get; set; }

        public bool Ativo { get; set; }

        [Display(Name = "Tipo")]
        public int TipoPessoa { get; set; }

        public int EnderecoId { get; set; }
        public Endereco Endereco { get; set; }
    }
}
