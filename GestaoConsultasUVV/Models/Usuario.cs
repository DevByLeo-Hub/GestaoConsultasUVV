using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestaoConsultasUVV.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "E-mail em formato inválido.")]
        [StringLength(150, ErrorMessage = "O e-mail deve ter no máximo 150 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public string Senha { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.Now;

        public ICollection<Consulta> Consultas { get; set; }
    }
}