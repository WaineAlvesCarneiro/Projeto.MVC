using Microsoft.EntityFrameworkCore;
using System;
using WebMvc.OrdemServico.Models;

namespace WebMvc.OrdemServico.Data
{
    public class OrdemServicoContext : DbContext
    {
        public OrdemServicoContext(DbContextOptions<OrdemServicoContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Cliente

            builder.Entity<Cliente>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                b.Property(p => p.Cpf)
                    .IsRequired()
                    .HasMaxLength(11)
                    .HasColumnType("varchar(11)");

                b.Property(p => p.Celular)
                    .IsRequired()
                    .HasMaxLength(16)
                    .HasColumnType("varchar(16)");

                b.Property(p => p.Telefone)
                    .HasMaxLength(16)
                    .HasColumnType("varchar(16)");

                b.Property(p => p.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                b.Property(p => p.Cep)
                    .HasMaxLength(12)
                    .HasColumnType("varchar(12)");

                b.Property(p => p.Uf)
                    .HasMaxLength(30)
                    .HasColumnType("varchar(30)");

                b.Property(p => p.Cidade)
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                b.Property(p => p.Endereco)
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                b.Property(p => p.Complemento)
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                b.ToTable("Cliente");
            });

            #endregion

            #region Os

            builder.Entity<Os>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.DtAbertura)
                    .HasColumnType("datetime2");

                b.Property(p => p.ClienteId)
                    .HasColumnType("int");

                b.Property(p => p.TotalOs)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.Property(p => p.DtConcluido)
                    .HasColumnType("datetime2");

                b.HasIndex("ClienteId");

                b.ToTable("Os");
            });

            #endregion

            #region Prestado

            builder.Entity<Prestado>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.OsId)
                    .HasColumnType("int");

                b.Property(p => p.QtdeServico)
                    .IsRequired()
                    .HasColumnType("int");

                b.Property(p => p.ServicoId)
                    .HasColumnType("int");

                b.Property(p => p.TotalItem)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.Property(p => p.ValorItem)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.HasIndex("OsId");

                b.HasIndex("ServicoId");

                b.ToTable("Prestado");
            });

            #endregion

            #region Servico

            builder.Entity<Servico>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.NomeServico)
                    .IsRequired()
                    .HasColumnType("varchar(30)");

                b.Property(p => p.Preco)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.ToTable("Servico");
            });

            #endregion
        }

        #region DbSet_Models

        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Servico> Servico { get; set; }
        public DbSet<Os> Os { get; set; }
        public DbSet<Prestado> Prestado { get; set; }
        #endregion
    }
}
