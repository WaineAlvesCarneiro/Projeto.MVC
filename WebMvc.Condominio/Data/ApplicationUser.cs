using Microsoft.AspNetCore.Identity;
using WebMvc.Condominio.Models;

namespace WebMvc.Condominio.Data
{
    public class ApplicationUser : IdentityUser
    {
        public Empresa Empresa { get; set; }
        public int EmpresaId { get; set; }
    }
}