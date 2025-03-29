using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;

namespace Operacao.Context
{
    public class OperacaoContextFactory : IDesignTimeDbContextFactory<OperacaoContext>
    {
        public OperacaoContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<OperacaoContext>();
            optionsBuilder.UseSqlServer("Server=localhost\\sqlexpress; Initial Catalog=Manutencao; Integrated Security=True; TrustServerCertificate=True");
            return new OperacaoContext(optionsBuilder.Options);
        }
    }
}
