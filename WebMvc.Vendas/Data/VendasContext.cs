using Microsoft.EntityFrameworkCore;
using System;
using WebMvc.Vendas.Models;

namespace WebMvc.Vendas.Data
{
    public class VendasContext : DbContext
    {
        public VendasContext(DbContextOptions<VendasContext> options)
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

            #region Fornecedor

            builder.Entity<Fornecedor>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.Nome)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasColumnType("varchar(50)");

                b.Property(p => p.Cnpj)
                    .IsRequired()
                    .HasMaxLength(14)
                    .HasColumnType("varchar(14)");

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

                b.ToTable("Fornecedor");
            });

            #endregion

            #region Item

            builder.Entity<Item>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.Sequencia)
                    .IsRequired()
                    .HasColumnType("int");

                b.Property(p => p.PedidoId)
                    .HasColumnType("int");

                b.Property(p => p.ProdutoId)
                    .HasColumnType("int");

                b.Property(p => p.QuantidadeItem)
                    .IsRequired()
                    .HasColumnType("int");

                b.Property(p => p.PrecoItem)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.Property(p => p.TotalItem)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.HasIndex("PedidoId");

                b.HasIndex("ProdutoId");

                b.ToTable("Item");
            });

            #endregion

            #region Pedido

            builder.Entity<Pedido>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.DataPedido)
                    .HasColumnType("datetime2");

                b.Property(p => p.StatusId)
                    .HasColumnType("int");

                b.Property(p => p.ClienteId)
                    .HasColumnType("int");

                b.Property(p => p.RcaId)
                    .HasColumnType("int");

                b.Property(p => p.TotalPedido)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.Property(p => p.Cont)
                    .IsRequired()
                    .HasColumnType("int");

                b.HasIndex("ClienteId");

                b.HasIndex("RcaId");

                b.HasIndex("StatusId");

                b.ToTable("Pedido");
            });

            #endregion

            #region Produto

            builder.Entity<Produto>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.FornecedorId)
                    .HasColumnType("int");

                b.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                b.Property(p => p.Preco)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.HasIndex("FornecedorId");

                b.ToTable("Produto");
            });

            #endregion

            #region Rca

            builder.Entity<Rca>(b =>
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

                b.ToTable("Rca");
            });

            #endregion

            #region Status

            builder.Entity<Status>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.NomeStatus)
                    .IsRequired()
                    .HasColumnType("varchar(15)");

                b.ToTable("Status");
            });

            #endregion
        }

        #region DbSet_Models

        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<Fornecedor> Fornecedor { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<Rca> Rca { get; set; }
        public DbSet<Produto> Produto { get; set; }
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Item> Item { get; set; }

        #endregion
    }
}
