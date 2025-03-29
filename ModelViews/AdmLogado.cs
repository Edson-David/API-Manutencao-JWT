using Manutencao.Entities;

namespace Manutencao.ModelViews;

public record AdmLogado
{
    public string Matricula { get; set; } = default!;
    public string Perfil { get; set; } = default!;
    public string Token { get; set; } = default!;
}