using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebMvc.Condominio.Models
{
    public class Morador
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public int ImovelId { get; set; }
        public Imovel Imovel { get; set; }

        [Display(Name = "Morador")]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(50, ErrorMessage = "O campo {0} precisa ter entre {2} e {1} caracteres", MinimumLength = 3)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "{0} é obrigatório")]
        [StringLength(16, ErrorMessage = "O campo {0} precisa ter {1} caracteres", MinimumLength = 11)]
        public string Celular { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "{0} é obrigatório")]
        [MaxLength(50, ErrorMessage = "O campo {0} pode ter no máximo {1} caracteres")]
        public string Email { get; set; }

        [StringLength(16, ErrorMessage = "O campo {0} precisa ter {1} caracteres", MinimumLength = 11)]
        public string Telefone { get; set; }

        [NotMapped]
        [DisplayName("Foto")]
        [Required(ErrorMessage = "{0} é obrigatorio")]
        public IFormFile FotoFile { get; set; }
        public string Foto { get; set; } //varchar(100)
    }
}
