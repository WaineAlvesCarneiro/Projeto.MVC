using System.ComponentModel.DataAnnotations;

namespace WebMvc.Vendas.Models
{
    public class Fornecedor
    {
        public int Id { get; set; }

        [Display(Name = "Razão social")]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        [StringLength(50, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 3)]
        public string Nome { get; set; }

        [Display(Name = "CNPJ")]
        [StringLength(14, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 14)]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        public string Cnpj { get; set; }

        [Required(ErrorMessage = "{0} é obrigatorio")]
        [StringLength(16, ErrorMessage = "O campo {0} precisa ter {1} caracteres", MinimumLength = 11)]
        public string Celular { get; set; }

        [StringLength(16, ErrorMessage = "O campo {0} precisa ter {1} caracteres", MinimumLength = 11)]
        public string Telefone { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string Email { get; set; }

        [Display(Name = "CEP")]
        [StringLength(8, ErrorMessage = "O campo {0} precisa ter {1} caracteres", MinimumLength = 8)]
        public string Cep { get; set; }

        [Display(Name = "Estado-UF")]
        [StringLength(30, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Uf { get; set; }

        [StringLength(50, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Cidade { get; set; }

        [Display(Name = "Endereço")]
        [StringLength(50, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 2)]
        public string Endereco { get; set; }

        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string Complemento { get; set; }
    }
}
