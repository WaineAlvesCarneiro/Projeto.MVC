using System.Collections.Generic;

namespace WebMvc.Vendas.Models
{
    public class PedidoViewModel
    {
        public Pedido Pedido { get; set; }

        public Item Item { get; set; }
        public ICollection<Item> Itemes { get; set; }

        public Status StatusList { get; set; }
        public ICollection<Status> Statuses { get; set; }

        public Cliente ClienteList { get; set; }
        public ICollection<Cliente> Clientees { get; set; }

        public Rca RcaList { get; set; }
        public ICollection<Rca> Rcaes { get; set; }

        public ICollection<Produto> Produtoes { get; set; }
    }
}
