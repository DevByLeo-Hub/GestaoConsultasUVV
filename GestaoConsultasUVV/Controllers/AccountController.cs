using System.Security.Claims;
using GestaoConsultasUVV.Data;
using GestaoConsultasUVV.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoConsultasUVV.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Cadastro()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastro(Usuario usuario)
        {
            // MODO À PROVA DE FALHAS: Fazemos a checagem manual e direta do que realmente importa!
            if (!string.IsNullOrEmpty(usuario.Nome) && 
                !string.IsNullOrEmpty(usuario.Email) && 
                !string.IsNullOrEmpty(usuario.Senha) && 
                usuario.Senha.Length >= 6)
            {
                usuario.DataCadastro = DateTime.Now;
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();
                
                // Sucesso absoluto: manda direto pra tela de Login!
                return RedirectToAction("Login");
            }

            // Se a senha tiver menos de 6 caracteres, ele volta pra tela de cadastro
            return View(usuario);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string senha)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Senha == senha);

            if (usuario != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, usuario.Nome),
                    new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("Index", "Home"); 
            }

            ViewBag.Erro = "E-mail ou senha inválidos.";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}