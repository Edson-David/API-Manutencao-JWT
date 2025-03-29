/*using Manutencao;
using Operacao.Context;
using Manutencao.DTOs;
using Manutencao.Entities;
using Manutencao.Interfaces;
using Manutencao.ModelViews;

namespace Manutencao.Servicos;

public class AdministradorServico : iAdministradorServico
{
    private readonly OperacaoContext _context;

    public AdministradorServico(OperacaoContext context)
    {
        _context = context;
    }

    public Administrador Login(LoginDTO loginDTO)
    {
        return _context.Administradores.FirstOrDefault(a => a.Matricula == loginDTO.Matricula && a.Senha == loginDTO.Senha);
    }

    public Administrador Incluir(AdministradorDTO administradorDTO)
    {
        var administrador = new Administrador
        {
            Matricula = administradorDTO.Matricula,
            Senha = administradorDTO.Senha,
            Perfil = administradorDTO.Perfil
        };

        _context.Administradores.Add(administrador);
        _context.SaveChanges();

        return administrador;
    }

    /*public Administrador? Login(LoginDTO loginDTO)
    {
        var administrador = _context.Administradores.FirstOrDefault(a => a.Matricula == loginDTO.Matricula && a.Senha == loginDTO.Senha);
        return administrador;
    }/

    public Administrador BuscaPorId(int id)
    {
        return _context.Administradores.Find(id);
    }

    public List<Administrador> Todos(int? pagina)
    {
        return _context.Administradores.Skip((pagina ?? 0) * 10).Take(10).ToList();
    }
}*/

using Manutencao;
using Operacao.Context;
using Manutencao.DTOs;
using Manutencao.Entities;
using Manutencao.Interfaces;
using Manutencao.ModelViews;

namespace Manutencao.Servicos;

public class AdministradorServico : iAdministradorServico
{
    private readonly OperacaoContext _context;

    public AdministradorServico(OperacaoContext context)
    {
        _context = context;
    }

    public Administrador Login(LoginDTO loginDTO)
    {
        return _context.Administradores.FirstOrDefault(a => a.Matricula == loginDTO.Matricula && a.Senha == loginDTO.Senha);
    }

    public Administrador Incluir(Administrador administrador)
    {
        // Validation
        var validacao = new ErroDeValidacao
        {
            Mensagens = new List<string>()
        };

        if (string.IsNullOrEmpty(administrador.Matricula))
            validacao.Mensagens.Add("Matricula não pode ser vazio");
        if (string.IsNullOrEmpty(administrador.Senha))
            validacao.Mensagens.Add("Senha não pode ser vazia");
        if (administrador.Perfil == null)
            validacao.Mensagens.Add("Perfil não pode ser vazio");

        if (validacao.Mensagens.Count > 0)
            throw new Exception(string.Join(", ", validacao.Mensagens)); // Or a custom exception

        _context.Administradores.Add(administrador);
        _context.SaveChanges();

        return administrador;
    }

    /*public Administrador Login(LoginDTO loginDTO)
    {
        var administrador = _context.Administradores.FirstOrDefault(a => a.Matricula == loginDTO.Matricula && a.Senha == loginDTO.Senha);
        return administrador;
    }*/

    public Administrador BuscaPorId(int id)
    {
        return _context.Administradores.Find(id);
    }

    public List<Administrador> Todos(int? pagina)
    {
        return _context.Administradores.Skip((pagina ?? 0) * 10).Take(10).ToList();
    }
}
