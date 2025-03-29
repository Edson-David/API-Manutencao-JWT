using Manutencao.Entities;
using Manutencao.DTOs;

namespace Manutencao.Interfaces
{
    public interface iAdministradorServico
    {
        Administrador Login(LoginDTO loginDTO);
        //Administrador Incluir(AdministradorDTO administradorDTO);
        Administrador Incluir(Administrador administrador);
        Administrador BuscaPorId(int id);
        List<Administrador> Todos(int? pagina);
    }
}