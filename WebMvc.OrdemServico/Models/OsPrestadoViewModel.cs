using System.Collections.Generic;

namespace WebMvc.OrdemServico.Models
{
    public class OsPrestadoViewModel
    {
        public Os Os { get; set; }

        public Prestado Prestado { get; set; }
        public ICollection<Prestado> Prestadoes { get; set; }

        public ICollection<Servico> Servicoes { get; set; }

        public Cliente ClienteList { get; set; }
        public ICollection<Cliente> Clientees { get; set; }
    }
}
