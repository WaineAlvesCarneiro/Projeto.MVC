using System.ComponentModel.DataAnnotations;

namespace WebMvc.Vendas.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} é obrigatorio")]
        [StringLength(50, ErrorMessage = "O campo {0} deve ter entre {2} e {1} numeros", MinimumLength = 3)]
        public string Nome { get; set; }

        [Display(Name = "CPF")]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        [StringLength(11, ErrorMessage = "O campo {0} deve ter entre {2} e {1} numeros", MinimumLength = 11)]
        public string Cpf { get; set; }

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
