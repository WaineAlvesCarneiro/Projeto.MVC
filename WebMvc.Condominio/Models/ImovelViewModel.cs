using System.Collections.Generic;

namespace WebMvc.Condominio.Models
{
    public class ImovelViewModel
    {
        public Imovel Imovel { get; set; }

        public Morador Morador { get; set; }
        public ICollection<Morador> Moradores { get; set; }

        public Prestador Prestador { get; set; }
        public Visitante Visitante { get; set; }

        public EncRecebida EncRecebida { get; set; }

        public EncEntrega EncEntrega { get; set; }
        public Morador MoradorRecebeuEntrega { get; set; }

    }
}
