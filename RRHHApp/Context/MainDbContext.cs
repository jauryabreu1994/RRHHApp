
using RRHHApp.Models.Licencias;
using RRHHApp.Models.Solicitudes;
using RRHHApp.Models.Tardanza;
using RRHHApp.Models.Ubicaciones;
using RRHHApp.Models.Usuarios;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace RRHHApp.Context
{
    public class MainDbContext : DbContext
    {

        public MainDbContext() : base("name=mssql")
        { }


        public DbSet<Ciudad> Ciudad { get; set; }
        public DbSet<Pais> Pais { get; set; }

        public DbSet<Posicion> Posicion { get; set; }
        public DbSet<Departamento> Departamento { get; set; }
        public DbSet<GrupoPermiso> GrupoPermiso { get; set; }
        public DbSet<GrupoUsuario> GrupoUsuario { get; set; }
        public DbSet<Permiso> Permiso { get; set; }
        public DbSet<UsuarioAsociado> UsuarioAsociado { get; set; }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<UsuarioContrasena> UsuarioContrasena { get; set; }


        public DbSet<Licencia> Licencia { get; set; }
        public DbSet<LicenciaConfiguracion> LicenciaConfiguracion { get; set; }


        public DbSet<MotivoTardanza> MotivoTardanza { get; set; }
        public DbSet<UsuarioTardanza> UsuarioTardanza { get; set; }
        public DbSet<UsuarioSolicitudes> UsuarioSolicitudes { get; set; }
        public DbSet<SolicitudEstado> SolicitudEstado { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Common Entities
            modelBuilder.Properties<int>()
                .Where(a => a.Name == "Id")
                .Configure(c => c.HasColumnType("int").HasColumnName("Id")
                .IsKey()
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity));
            #endregion

            modelBuilder.Entity<Ciudad>().ToTable("Ciudad", "dbo").HasKey(a => a.Id);
            modelBuilder.Entity<Pais>().ToTable("Pais", "dbo").HasKey(a => a.Id);

            modelBuilder.Entity<Posicion>().ToTable("Posicion", "dbo").HasKey(a => a.Id);
            modelBuilder.Entity<Departamento>().ToTable("Departamento", "dbo").HasKey(a => a.Id);
            modelBuilder.Entity<GrupoPermiso>().ToTable("GrupoPermiso", "dbo").HasKey(a => a.Id);
            modelBuilder.Entity<GrupoUsuario>().ToTable("GrupoUsuario", "dbo").HasKey(a => a.Id);
            modelBuilder.Entity<Permiso>().ToTable("Permiso", "dbo").HasKey(a => a.Id);
            modelBuilder.Entity<UsuarioAsociado>().ToTable("UsuarioAsociado", "dbo").HasKey(a => a.Id);
            modelBuilder.Entity<Usuario>().ToTable("Usuario", "dbo").HasKey(a => a.Id);
            modelBuilder.Entity<UsuarioContrasena>().ToTable("UsuarioContrasena", "dbo").HasKey(a => a.Id);

            modelBuilder.Entity<Licencia>().ToTable("Licencia", "dbo").HasKey(a => a.Id);
            modelBuilder.Entity<LicenciaConfiguracion>().ToTable("LicenciaConfiguracion", "dbo").HasKey(a => a.Id);
            modelBuilder.Entity<UsuarioSolicitudes>().ToTable("UsuarioSolicitudes", "dbo").HasKey(a => a.Id); 
            modelBuilder.Entity<SolicitudEstado>().ToTable("SolicitudEstado", "dbo").HasKey(a => a.Id);

            #region Pais
            modelBuilder.Entity<Pais>()
                .HasMany(a => a.__Ciudades)
                .WithRequired(b => b._Pais)
                .HasForeignKey(c => c.PaisId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Pais>()
                .HasMany(a => a.__Usuarios)
                .WithRequired(b => b._Pais)
                .HasForeignKey(c => c.PaisId).WillCascadeOnDelete(false); 
            #endregion

            #region Ciudad
            modelBuilder.Entity<Ciudad>()
                .HasMany(a => a.__Usuarios)
                .WithRequired(b => b._Ciudad)
                .HasForeignKey(c => c.CiudadId).WillCascadeOnDelete(false);
            #endregion

            #region Usuarios
            modelBuilder.Entity<Usuario>()
                .HasMany(a => a.__UsuarioContrasenas)
                .WithRequired(b => b._Usuario)
                .HasForeignKey(c => c.UsuarioId).WillCascadeOnDelete(false);


            modelBuilder.Entity<Usuario>()
               .HasMany(a => a.__UsuarioAsociadosLider)
               .WithRequired(b => b._Lider)
               .HasForeignKey(c => c.LiderId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
               .HasMany(a => a.__UsuarioAsociadosRecurso)
               .WithRequired(b => b._Usuario)
               .HasForeignKey(c => c.UsuarioId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
               .HasMany(a => a.__UsuarioSolicitudes)
               .WithRequired(b => b._Usuario)
               .HasForeignKey(c => c.UsuarioId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
               .HasMany(a => a.__SolicitudesEstadoGerente)
               .WithRequired(b => b._Gerente)
               .HasForeignKey(c => c.GerenteId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
               .HasMany(a => a.__SolicitudesEstadoRRHH)
               .WithRequired(b => b._RRHH)
               .HasForeignKey(c => c.RRHHId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
               .HasMany(a => a.__SolicitudesEstadoSupervidor)
               .WithRequired(b => b._Supervisor)
               .HasForeignKey(c => c.SupervisorId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Usuario>()
                .HasMany(a => a.__UsuarioTardanza)
                .WithRequired(b => b._Usuario)
                .HasForeignKey(c => c.UsuarioId).WillCascadeOnDelete(false);
            #endregion

            #region Departamento
            modelBuilder.Entity<Departamento>()
                .HasMany(a => a.__Posiciones)
                .WithRequired(b => b._Departamento)
                .HasForeignKey(c => c.DepartamentoId).WillCascadeOnDelete(false);
            #endregion

            #region Posicion
            modelBuilder.Entity<Posicion>()
                 .HasMany(a => a.__Usuarios)
                 .WithRequired(b => b._Posicion)
                 .HasForeignKey(c => c.PosicionId).WillCascadeOnDelete(false);
            #endregion

            #region Grupo Usuario
            modelBuilder.Entity<GrupoUsuario>()
                .HasMany(a => a.__GrupoPermisos)
                .WithRequired(b => b._GrupoUsuario)
                .HasForeignKey(c => c.GrupoUsuarioId).WillCascadeOnDelete(false);

            modelBuilder.Entity<GrupoUsuario>()
                .HasMany(a => a.__Usuarios)
                .WithRequired(b => b._GrupoUsuario)
                .HasForeignKey(c => c.GrupoUsuarioId).WillCascadeOnDelete(false);
            #endregion

            #region Permiso
            modelBuilder.Entity<Permiso>()
                .HasMany(a => a.__GrupoPermisos)
                .WithRequired(b => b._Permiso)
                .HasForeignKey(c => c.PermisoId).WillCascadeOnDelete(false);
            #endregion

            #region Licencia
            modelBuilder.Entity<Licencia>()
                .HasMany(a => a.__LicenciaConfiguraciones)
                .WithRequired(b => b._Licencia)
                .HasForeignKey(c => c.LicenciaId).WillCascadeOnDelete(false);

            modelBuilder.Entity<Licencia>()
               .HasMany(a => a.__UsuarioSolicitudes)
               .WithRequired(b => b._Licencia)
               .HasForeignKey(c => c.LicenciaId).WillCascadeOnDelete(false);
            #endregion

            #region Usuario Solicitudes
            modelBuilder.Entity<UsuarioSolicitudes>()
                .HasMany(a => a.__SolicitudesEstado)
                .WithRequired(b => b._UsuarioSolicitudes)
                .HasForeignKey(c => c.UsuarioSolicitudId).WillCascadeOnDelete(false);
            #endregion

            #region Tardanza
            modelBuilder.Entity<MotivoTardanza>()
                .HasMany(a => a.__UsuarioTardanza)
                .WithRequired(b => b._MotivoTardanza)
                .HasForeignKey(c => c.MotivoTardanzaId).WillCascadeOnDelete(false);
            #endregion

        }
    }
}