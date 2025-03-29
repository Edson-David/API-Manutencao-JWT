using Manutencao.Entities;

namespace Manutencao.DTOs
{
    public class AdministradorDTO
    {
        public string Matricula { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public string Perfil { get; set; } = default!;
    }
}