using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using WebMvc.Condominio.Models;

namespace WebMvc.Condominio.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Role e RoleClaim
            builder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("varchar(450)");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("varchar(max)");

                b.Property<string>("Name")
                    .HasMaxLength(256)
                    .HasColumnType("varchar(256)");

                b.Property<string>("NormalizedName")
                    .HasMaxLength(256)
                    .HasColumnType("varchar(256)");

                b.HasKey("Id");

                b.HasIndex("NormalizedName")
                    .IsUnique()
                    .HasDatabaseName("RoleNameIndex")
                    .HasFilter("[NormalizedName] IS NOT NULL");

                b.ToTable("AspNetRoles");
            });

            builder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                b.Property<string>("ClaimType")
                    .HasColumnType("varchar(max)");

                b.Property<string>("ClaimValue")
                    .HasColumnType("varchar(max)");

                b.Property<string>("RoleId")
                    .IsRequired()
                    .HasColumnType("varchar(450)");

                b.HasKey("Id");

                b.HasIndex("RoleId");

                b.ToTable("AspNetRoleClaims");
            });
            #endregion

            #region Empresa
            builder.Entity<Empresa>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.RazaoSocial)
                   .IsRequired()
                   .HasMaxLength(70)
                   .HasColumnType("varchar(70)");

                b.Property(p => p.Fantasia)
                   .IsRequired()
                   .HasMaxLength(50)
                   .HasColumnType("varchar(50)");

                b.Property(p => p.Cnpj)
                    .IsRequired()
                    .HasMaxLength(14)
                    .HasColumnType("varchar(14)");

                b.Property(p => p.Tipo)
                    .HasColumnType("int");

                b.Property(p => p.NomeResposavel)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

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

                b.ToTable("Empresa");
            });
            #endregion

            #region User 
            builder.Entity("WebMvc.Condominio.Data.ApplicationUser", b =>
            {
                b.Property<string>("Id")
                    .HasColumnType("varchar(450)");

                b.Property<int>("AccessFailedCount")
                    .HasColumnType("int");

                b.Property<string>("ConcurrencyStamp")
                    .IsConcurrencyToken()
                    .HasColumnType("varchar(max)");

                b.Property<string>("Email")
                    .HasMaxLength(256)
                    .HasColumnType("varchar(256)");

                b.Property<bool>("EmailConfirmed")
                    .HasColumnType("bit");

                b.Property<int>("EmpresaId")
                    .HasColumnType("int");

                b.Property<bool>("LockoutEnabled")
                    .HasColumnType("bit");

                b.Property<DateTimeOffset?>("LockoutEnd")
                    .HasColumnType("datetimeoffset");

                b.Property<string>("NormalizedEmail")
                    .HasMaxLength(256)
                    .HasColumnType("varchar(256)");

                b.Property<string>("NormalizedUserName")
                    .HasMaxLength(256)
                    .HasColumnType("varchar(256)");

                b.Property<string>("PasswordHash")
                    .HasColumnType("varchar(max)");

                b.Property<string>("PhoneNumber")
                    .HasColumnType("varchar(max)");

                b.Property<bool>("PhoneNumberConfirmed")
                    .HasColumnType("bit");

                b.Property<string>("SecurityStamp")
                    .HasColumnType("varchar(max)");

                b.Property<bool>("TwoFactorEnabled")
                    .HasColumnType("bit");

                b.Property<string>("UserName")
                    .HasMaxLength(256)
                    .HasColumnType("varchar(256)");

                b.HasKey("Id");

                b.HasIndex("EmpresaId");

                b.HasIndex("NormalizedEmail")
                    .HasDatabaseName("EmailIndex");

                b.HasIndex("NormalizedUserName")
                    .IsUnique()
                    .HasDatabaseName("UserNameIndex")
                    .HasFilter("[NormalizedUserName] IS NOT NULL");

                b.ToTable("AspNetUsers");
            });
            #endregion

            #region MyRegion UserClaim - UserLogin - UserRole - UserToken
            builder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("int")
                    .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                b.Property<string>("ClaimType")
                    .HasColumnType("varchar(max)");

                b.Property<string>("ClaimValue")
                    .HasColumnType("varchar(max)");

                b.Property<string>("UserId")
                    .IsRequired()
                    .HasColumnType("varchar(450)");

                b.HasKey("Id");

                b.HasIndex("UserId");

                b.ToTable("AspNetUserClaims");
            });

            builder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
            {
                b.Property<string>("LoginProvider")
                    .HasColumnType("varchar(450)");

                b.Property<string>("ProviderKey")
                    .HasColumnType("varchar(450)");

                b.Property<string>("ProviderDisplayName")
                    .HasColumnType("varchar(max)");

                b.Property<string>("UserId")
                    .IsRequired()
                    .HasColumnType("varchar(450)");

                b.HasKey("LoginProvider", "ProviderKey");

                b.HasIndex("UserId");

                b.ToTable("AspNetUserLogins");
            });

            builder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
            {
                b.Property<string>("UserId")
                    .HasColumnType("varchar(450)");

                b.Property<string>("RoleId")
                    .HasColumnType("varchar(450)");

                b.HasKey("UserId", "RoleId");

                b.HasIndex("RoleId");

                b.ToTable("AspNetUserRoles");
            });

            builder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
            {
                b.Property<string>("UserId")
                    .HasColumnType("varchar(450)");

                b.Property<string>("LoginProvider")
                    .HasColumnType("varchar(450)");

                b.Property<string>("Name")
                    .HasColumnType("varchar(450)");

                b.Property<string>("Value")
                    .HasColumnType("varchar(max)");

                b.HasKey("UserId", "LoginProvider", "Name");

                b.ToTable("AspNetUserTokens");
            });
            #endregion

            #region Imovel
            builder.Entity<Imovel>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.EmpresaId)
                    .HasColumnType("int");

                b.Property(p => p.AptoLote)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnType("varchar(10)");

                b.Property(p => p.BlocoQuadra)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnType("varchar(10)");

                b.Property(p => p.Box)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnType("varchar(10)");

                b.Property(p => p.Tipo)
                    .HasColumnType("int");

                b.Property(p => p.DataEntrada)
                    .HasColumnType("datetime2");

                b.Property(p => p.DataSaida)
                    .HasColumnType("datetime2");

                b.Property(p => p.Marca)
                    .HasMaxLength(20)
                    .HasColumnType("varchar(20)");

                b.Property(p => p.Modelo)
                    .HasMaxLength(20)
                    .HasColumnType("varchar(20)");

                b.Property(p => p.Cor)
                    .HasMaxLength(20)
                    .HasColumnType("varchar(20)");

                b.Property(p => p.Placa)
                    .HasMaxLength(20)
                    .HasColumnType("varchar(20)");

                b.Property(p => p.ParenteNome)
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                b.Property(p => p.ParenteCelular)
                    .HasMaxLength(16)
                    .HasColumnType("varchar(16)");

                b.Property(p => p.ParenteTelefone)
                    .HasMaxLength(16)
                    .HasColumnType("varchar(16)");

                b.ToTable("Imovel");
            });
            #endregion

            #region Morador
            builder.Entity<Morador>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.EmpresaId)
                    .HasColumnType("int");

                b.Property(p => p.ImovelId)
                    .HasColumnType("int");

                b.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                b.Property(p => p.Celular)
                    .IsRequired()
                    .HasMaxLength(16)
                    .HasColumnType("varchar(16)");

                b.Property(p => p.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                b.Property(p => p.Telefone)
                    .HasMaxLength(16)
                    .HasColumnType("varchar(16)");

                b.Property(p => p.Foto)
                    .HasColumnType("varchar(100)");

                b.HasIndex("ImovelId");

                b.ToTable("Morador");
            });
            #endregion

            #region Prestador
            builder.Entity<Prestador>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.EmpresaId)
                    .HasColumnType("int");

                b.Property(p => p.ImovelId)
                    .HasColumnType("int");

                b.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                b.Property(p => p.Documento)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnType("varchar(30)");

                b.Property(p => p.EmpresaPrestadora)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnType("varchar(30)");

                b.Property(p => p.DataHoraEntrada)
                    .HasColumnType("datetime2");

                b.Property(p => p.DataHoraSaida)
                    .HasColumnType("datetime2");

                b.Property(p => p.Foto)
                    .HasColumnType("varchar(100)");

                b.HasIndex("ImovelId");

                b.ToTable("Prestador");
            });
            #endregion

            #region Visitante
            builder.Entity<Visitante>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.EmpresaId)
                    .HasColumnType("int");

                b.Property(p => p.ImovelId)
                    .HasColumnType("int");

                b.Property(p => p.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnType("varchar(50)");

                b.Property(p => p.Documento)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnType("varchar(30)");

                b.Property(p => p.DataEntrada)
                    .HasColumnType("datetime2");

                b.Property(p => p.DataSaida)
                    .HasColumnType("datetime2");

                b.Property(p => p.Foto)
                    .HasColumnType("varchar(100)");

                b.HasIndex("ImovelId");

                b.ToTable("Visitante");
            });
            #endregion

            #region EncRecebida
            builder.Entity<EncRecebida>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.EmpresaId)
                    .HasColumnType("int");

                b.Property(p => p.ImovelId)
                    .HasColumnType("int");

                b.Property(p => p.MoradorId)
                    .HasColumnType("int");

                b.Property(p => p.CodigoRastreio)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnType("varchar(30)");

                b.Property(p => p.TipoEncomenda)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnType("varchar(30)");

                b.Property(p => p.DataRecebimento)
                    .HasColumnType("datetime2");

                b.Property(p => p.Entregue_Sim_Nao)
                    .HasColumnType("varchar(4)");

                b.HasIndex("ImovelId");

                b.ToTable("EncRecebida");
            });
            #endregion

            #region EncEntrega
            builder.Entity<EncEntrega>(b =>
            {
                b.HasKey(p => p.Id);

                b.Property(p => p.EmpresaId)
                    .HasColumnType("int");

                b.Property(p => p.ImovelId)
                    .HasColumnType("int");

                b.Property(p => p.EncRecebidaId)
                    .HasColumnType("int");

                b.Property(p => p.MoradorId)
                    .HasColumnType("int");

                b.Property(p => p.DocumentoDoMorador)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasColumnType("varchar(30)");

                b.Property(p => p.DataEntrega)
                    .HasColumnType("datetime2");

                b.Property(p => p.Foto)
                    .HasColumnType("varchar(100)");

                b.HasIndex("ImovelId");

                b.ToTable("EncEntrega");
            });
            #endregion
        }

        #region DbSet_Models
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Imovel> Imovel { get; set; }
        public DbSet<Morador> Morador { get; set; }
        public DbSet<Prestador> Prestador { get; set; }
        public DbSet<Visitante> Visitante { get; set; }
        public DbSet<EncRecebida> EncRecebida { get; set; }
        public DbSet<EncEntrega> EncEntrega { get; set; }        
        #endregion
    }
}