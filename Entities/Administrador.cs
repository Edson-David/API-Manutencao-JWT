using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Manutencao.Entities;

public class Administrador
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;

    [Required]
    [StringLength(255)]
    public string Matricula { get; set; } = default!;

    [Required]
    [StringLength(50)]
    public string Senha { get; set; } = default!;

    [Required]
    [StringLength(20)]
    public string Perfil { get; set; } = default!;
}