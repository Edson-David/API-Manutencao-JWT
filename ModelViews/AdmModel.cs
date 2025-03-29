using Manutencao.Entities;

namespace Manutencao.ModelViews
{
    public class AdministradorModel
    {
        public int Id { get; set; }
        public string Matricula { get; set; } = default!;
        public string Perfil { get; set; } = default!;
    }
}