using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Manutencao.Entities;
using Microsoft.EntityFrameworkCore;

namespace Operacao.Context
{
    public class OperacaoContext : DbContext
    {
        public OperacaoContext(DbContextOptions<OperacaoContext> options) : base(options)
        {
        }
        public DbSet<Reparo> Reparos { get; set; }

        public DbSet<Administrador> Administradores { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrador>().HasData(
                new Administrador
                {
                    Id = 1,
                    Matricula = "2210",
                    Perfil = "1",
                    Senha = "0122"
                }
            );
        }
    }
}