using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using GestaoConsultasUVV.Data;
using GestaoConsultasUVV.Models;
using System.Security.Claims;

namespace GestaoConsultasUVV.Controllers
{
    [Authorize]
    public class ConsultasController : Controller
    {
        private readonly AppDbContext _context;

        public ConsultasController(AppDbContext context)
        {
            _context = context;
        }

        private int GetUsuarioLogadoId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userId);
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = GetUsuarioLogadoId();
            var consultas = await _context.Consultas
                .Where(c => c.UsuarioId == usuarioId)
                .OrderBy(c => c.DataHora)
                .ToListAsync();
                
            return View(consultas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Consulta consulta)
        {
            consulta.UsuarioId = GetUsuarioLogadoId();
            ModelState.Remove("Usuario");

            // O TRUQUE DE MESTRE AQUI:
            // Se a descrição estiver nula, transformamos em uma string vazia "".
            // Assim, o banco de dados SQL Server aceita sem reclamar!
            if (string.IsNullOrEmpty(consulta.Descricao))
            {
                consulta.Descricao = "";
            }

            if (ModelState.IsValid)
            {
                _context.Consultas.Add(consulta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(consulta);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta == null || consulta.UsuarioId != GetUsuarioLogadoId()) return NotFound();

            return View(consulta);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta != null && consulta.UsuarioId == GetUsuarioLogadoId())
            {
                _context.Consultas.Remove(consulta);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 4. UPDATE: Abre a tela de edição com os dados preenchidos
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var consulta = await _context.Consultas.FindAsync(id);
            if (consulta == null || consulta.UsuarioId != GetUsuarioLogadoId()) return NotFound();

            return View(consulta);
        }

        // 4. UPDATE: Recebe os dados alterados e salva no banco
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Consulta consulta)
        {
            if (id != consulta.Id) return NotFound();

            consulta.UsuarioId = GetUsuarioLogadoId();
            ModelState.Remove("Usuario");

            if (string.IsNullOrEmpty(consulta.Descricao))
            {
                consulta.Descricao = "";
            }

            if (ModelState.IsValid)
            {
                _context.Update(consulta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(consulta);
        }
    }
}