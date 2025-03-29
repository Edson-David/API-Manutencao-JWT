using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Operacao.Context;
using Manutencao.Entities;
using Microsoft.AspNetCore.Authorization;
using Manutencao.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Manutencao.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ManutencaoController : ControllerBase
    {
        private readonly OperacaoContext _context;
        private readonly IConfiguration _configuration;

        public ManutencaoController(OperacaoContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Endpoint de Login
        /*[HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginDTO loginDTO)
        {
            var administrador = _context.Administradores.FirstOrDefault(adm => 
                adm.Matricula == loginDTO.Matricula && adm.Senha == loginDTO.Senha);

            if (administrador == null)
                return Unauthorized("Credenciais inválidas.");

            var token = GerarTokenJwt(administrador);

            return Ok(new
            {
                administrador.Matricula,
                administrador.Perfil,
                Token = token
            });
        }

        string GerarTokenJwt(Administrador administrador)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>()
             {
                new Claim(JwtRegisteredClaimNames.Sub, administrador.Matricula),
                new Claim("Perfil", administrador.Perfil.ToString()), // Convert to string
                new Claim(ClaimTypes.Role, administrador.Perfil.ToString()), // Convert to string
                new Claim(ClaimTypes.Role, administrador.Perfil.ToString() == "1" ? "Administrador" : "Usuario" ), 
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
            signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }*/

        [HttpGet("{id}")]
        [Authorize (Roles = "Administrador, Tecnico")]
        public IActionResult ObterPorId(int id)
        {
            var reparo = _context.Reparos.Find(id);

            if (reparo == null)
                return NotFound();

            return Ok(reparo);
        }

        [HttpGet("ObterTodos")]
        [Authorize (Roles = "Administrador, Tecnico")]
        public async Task<IActionResult> ObterTodosAsync(int? pagina)
        {
            int TamanhoPagina = 10;
            int numeroPagina = pagina ?? 1;
            int pular = (numeroPagina - 1) * TamanhoPagina;

            List<Reparo> reparos = await _context.Reparos.Skip(pular)
                                                         .Take(TamanhoPagina)
                                                         .ToListAsync();
            return Ok(reparos);
        }

        [HttpGet("ObterPorStatus")]
        [Authorize (Roles = "Administrador, Tecnico")]
        public IActionResult ObterPorStatus(EnumStatus status)
        {
            var reparo = _context.Reparos.Where(x => x.Status == status);
            return Ok(reparo);
        }

        [HttpPost]
        [Authorize (Roles = "Administrador, Tecnico, Usuario")]
        public IActionResult Maquina(Reparo reparo)
        {
            _context.Add(reparo);
            _context.SaveChanges();
            return CreatedAtAction(nameof(ObterPorId), new { id = reparo.Id }, reparo);
        }

        [HttpPut("{id}")]
        [Authorize (Roles = "Administrador, Tecnico")]
        public IActionResult Atualizar(int id, Reparo reparo)
        {
            var reparoBanco = _context.Reparos.Find(id);

            if (reparoBanco == null)
                return NotFound();

            reparoBanco.Maquina = reparo.Maquina;
            reparoBanco.Local = reparo.Local;
            reparoBanco.Problema = reparo.Problema;
            reparoBanco.Status = reparo.Status;

            _context.SaveChanges();
            return Ok();
        }


        // Endpoint para atualizar o status do reparo
        // Apenas o mecanico e o administrador podem atualizar o status

        /* O status pode ser: "Pendente = 0", "Em Andamento = 1", "Concluido = 2"
           Essa parte é apenas a titulo de curiosidade de como mostra no bando de dados
           Favor usar a escrita na hora de usar a API */
        [HttpPut("status")]
        [Authorize (Roles = "Administrador, Tecnico")]
        public IActionResult AtualizarStatus(int id, Reparo reparo)
        {
            var reparoBanco = _context.Reparos.Find(id);

            if (reparoBanco == null)
                return NotFound();

            reparoBanco.Status = reparo.Status;

            _context.SaveChanges();
            return Ok();
        }

        // Endpoint para deletar o reparo
        // Apenas o administrador pode deletar o reparo
        [HttpDelete("{id}")]
        [Authorize (Roles = "Administrador")]
        public IActionResult Deletar(int id)
        {
            var reparoBanco = _context.Reparos.Find(id);

            if (reparoBanco == null)
                return NotFound();

            _context.Reparos.Remove(reparoBanco);
            _context.SaveChanges();
            return NoContent();
        }
    }
}