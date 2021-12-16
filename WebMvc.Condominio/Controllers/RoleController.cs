using WebMvc.Condominio.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace WebMvc.Condominio.Controllers
{
    [Authorize(Roles = "Suporte")]
    public class RoleController : Controller
    {
        readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Suporte")]
        public IActionResult ListarRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return View(roles);
        }

        [Authorize(Roles = "Suporte")]
        public IActionResult CriarRole()
        {
            return View(new IdentityRole());
        }

        [Authorize(Roles = "Suporte")]
        [HttpPost]
        public async Task<IActionResult> CriarRole(IdentityRole role)
        {
            await _roleManager.CreateAsync(role);
            return RedirectToAction("ListarRoles");
        }

        //private bool RoleExists(int id)
        //{
        //    return _context.Role.Any(e => e.Id == id);
        //}

        public IActionResult Error(string message)
        {
            var viewModel = new ErrorViewModel
            {
                Message = message,
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            };
            return View(viewModel);
        }
    }
}
