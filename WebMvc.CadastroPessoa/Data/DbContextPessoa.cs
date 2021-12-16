using Microsoft.EntityFrameworkCore;
using WebMvc.CadastroPessoa.Models;

namespace WebMvc.CadastroPessoa.Data
{
    public class DbContextPessoa : DbContext
    {
        public DbContextPessoa(DbContextOptions<DbContextPessoa> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Pessoa>(builder =>
            {
                builder.HasKey(p => p.Id);

                builder.Property(p => p.Nome)
                .IsRequired()
                .HasColumnType("varchar(100)");

                builder.Property(p => p.CpfCnpj)
                    .IsRequired()
                    .HasColumnType("varchar(14)");

                builder.Property(p => p.Celular)
                    .IsRequired()
                    .HasMaxLength(16)
                    .HasColumnType("varchar(16)");

                builder.Property(p => p.Telefone)
                    .HasMaxLength(16)
                    .HasColumnType("varchar(16)");

                builder.Property(p => p.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                builder.Property(p => p.DtAniversario)
                    .HasColumnType("datetime2");

                builder.Property(p => p.TipoPessoa)
                    .HasColumnType("int");

                builder.HasIndex("EnderecoId");

                builder.ToTable("Pessoa");
            });

            builder.Entity<Endereco>(builder =>
            {
                builder.HasKey(p => p.Id);

                builder.Property(c => c.Cep)
                    .IsRequired()
                    .HasColumnType("varchar(8)");

                builder.Property(c => c.Logradouro)
                .IsRequired()
                .HasColumnType("varchar(200)");

                builder.Property(c => c.Numero)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                builder.Property(c => c.Bairro)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                builder.Property(c => c.Cidade)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                builder.Property(c => c.Estado)
                    .IsRequired()
                    .HasColumnType("varchar(50)");

                builder.Property(c => c.Complemento)
                    .HasColumnType("varchar(100)");

                builder.ToTable("Endereco");
            });
        }

        public DbSet<Endereco> Endereco { get; set; }
        public DbSet<Pessoa> Pessoa { get; set; }
    }
}
