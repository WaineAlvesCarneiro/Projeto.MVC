using System.ComponentModel.DataAnnotations;

namespace WebMvc.Vendas.Models
{
    public class Status
    {
        public int Id { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(15, ErrorMessage = "O campo {0} precisa ter {1} caracteres", MinimumLength = 3)]
        public string NomeStatus { get; set; }
    }
}
