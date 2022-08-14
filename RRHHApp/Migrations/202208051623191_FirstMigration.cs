namespace RRHHApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FirstMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Ciudad",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaisId = c.Int(nullable: false),
                        Descripcion = c.String(maxLength: 100),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Pais", t => t.PaisId)
                .Index(t => t.PaisId);
            
            CreateTable(
                "dbo.Usuario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GrupoUsuarioId = c.Int(nullable: false),
                        DepartamentoId = c.Int(nullable: false),
                        PosicionId = c.Int(nullable: false),
                        UsuarioId = c.String(maxLength: 20),
                        Nombre = c.String(maxLength: 100),
                        Apellido = c.String(maxLength: 100),
                        Identificacion = c.String(maxLength: 20),
                        PaisId = c.Int(nullable: false),
                        CiudadId = c.Int(nullable: false),
                        Direccion = c.String(maxLength: 300),
                        Correo = c.String(maxLength: 100),
                        Telefono = c.String(maxLength: 30),
                        FechaIngreso = c.DateTime(nullable: false),
                        Genero = c.Int(nullable: false),
                        RRHHAutorizado = c.Boolean(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Posicion", t => t.PosicionId)
                .ForeignKey("dbo.Departamento", t => t.DepartamentoId, cascadeDelete: true)
                .ForeignKey("dbo.GrupoUsuario", t => t.GrupoUsuarioId)
                .ForeignKey("dbo.Pais", t => t.PaisId)
                .ForeignKey("dbo.Ciudad", t => t.CiudadId)
                .Index(t => t.GrupoUsuarioId)
                .Index(t => t.DepartamentoId)
                .Index(t => t.PosicionId)
                .Index(t => t.PaisId)
                .Index(t => t.CiudadId);
            
            CreateTable(
                "dbo.SolicitudEstado",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsuarioSolicitudId = c.Int(nullable: false),
                        SupervisorId = c.Int(nullable: false),
                        SupervisorEstado = c.Int(nullable: false),
                        GerenteId = c.Int(nullable: false),
                        GerenteEstado = c.Int(nullable: false),
                        RRHHId = c.Int(nullable: false),
                        RRHHEstado = c.Int(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UsuarioSolicitudes", t => t.UsuarioSolicitudId)
                .ForeignKey("dbo.Usuario", t => t.GerenteId)
                .ForeignKey("dbo.Usuario", t => t.RRHHId)
                .ForeignKey("dbo.Usuario", t => t.SupervisorId)
                .Index(t => t.UsuarioSolicitudId)
                .Index(t => t.SupervisorId)
                .Index(t => t.GerenteId)
                .Index(t => t.RRHHId);
            
            CreateTable(
                "dbo.UsuarioSolicitudes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsuarioId = c.Int(nullable: false),
                        LicenciaId = c.Int(nullable: false),
                        FechaDesde = c.DateTime(nullable: false),
                        FechaHasta = c.DateTime(nullable: false),
                        Comentario = c.String(),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Licencia", t => t.LicenciaId)
                .ForeignKey("dbo.Usuario", t => t.UsuarioId)
                .Index(t => t.UsuarioId)
                .Index(t => t.LicenciaId);
            
            CreateTable(
                "dbo.Licencia",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                        Icono = c.String(),
                        Es_Principal = c.Boolean(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.LicenciaConfiguracion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LicenciaId = c.Int(nullable: false),
                        UsuarioTiempo = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                        RequiereSupervisorAprobacion = c.Boolean(nullable: false),
                        RequiereGerenteAprobacion = c.Boolean(nullable: false),
                        RequiereRRHHAprobacion = c.Boolean(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Licencia", t => t.LicenciaId)
                .Index(t => t.LicenciaId);
            
            CreateTable(
                "dbo.UsuarioAsociado",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LiderId = c.Int(nullable: false),
                        UsuarioId = c.Int(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuario", t => t.LiderId)
                .ForeignKey("dbo.Usuario", t => t.UsuarioId)
                .Index(t => t.LiderId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.UsuarioContrasena",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsuarioId = c.Int(nullable: false),
                        Contraseña = c.String(),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Usuario", t => t.UsuarioId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.UsuarioTardanzas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UsuarioId = c.Int(nullable: false),
                        MotivoTardanzaId = c.Int(nullable: false),
                        Tiempo = c.DateTime(nullable: false),
                        FechaTardanza = c.DateTime(nullable: false),
                        Comentario = c.String(),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MotivoTardanzas", t => t.MotivoTardanzaId)
                .ForeignKey("dbo.Usuario", t => t.UsuarioId)
                .Index(t => t.UsuarioId)
                .Index(t => t.MotivoTardanzaId);
            
            CreateTable(
                "dbo.MotivoTardanzas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Departamento",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(maxLength: 100),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Posicion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DepartamentoId = c.Int(nullable: false),
                        Descripcion = c.String(maxLength: 100),
                        EsLider = c.Boolean(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Departamento", t => t.DepartamentoId)
                .Index(t => t.DepartamentoId);
            
            CreateTable(
                "dbo.GrupoUsuario",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(maxLength: 100),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.GrupoPermiso",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GrupoUsuarioId = c.Int(nullable: false),
                        PermisoId = c.Int(nullable: false),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Permiso", t => t.PermisoId)
                .ForeignKey("dbo.GrupoUsuario", t => t.GrupoUsuarioId)
                .Index(t => t.GrupoUsuarioId)
                .Index(t => t.PermisoId);
            
            CreateTable(
                "dbo.Permiso",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Descripcion = c.String(maxLength: 100),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pais",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CodigoArea = c.String(maxLength: 10),
                        Descripcion = c.String(maxLength: 100),
                        CodigoTelefono = c.String(maxLength: 10),
                        FechaCreacion = c.DateTime(nullable: false),
                        FechaModificacion = c.DateTime(nullable: false),
                        Estado = c.Int(nullable: false),
                        Eliminado = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Usuario", "CiudadId", "dbo.Ciudad");
            DropForeignKey("dbo.Usuario", "PaisId", "dbo.Pais");
            DropForeignKey("dbo.Ciudad", "PaisId", "dbo.Pais");
            DropForeignKey("dbo.Usuario", "GrupoUsuarioId", "dbo.GrupoUsuario");
            DropForeignKey("dbo.GrupoPermiso", "GrupoUsuarioId", "dbo.GrupoUsuario");
            DropForeignKey("dbo.GrupoPermiso", "PermisoId", "dbo.Permiso");
            DropForeignKey("dbo.Usuario", "DepartamentoId", "dbo.Departamento");
            DropForeignKey("dbo.Posicion", "DepartamentoId", "dbo.Departamento");
            DropForeignKey("dbo.Usuario", "PosicionId", "dbo.Posicion");
            DropForeignKey("dbo.UsuarioTardanzas", "UsuarioId", "dbo.Usuario");
            DropForeignKey("dbo.UsuarioTardanzas", "MotivoTardanzaId", "dbo.MotivoTardanzas");
            DropForeignKey("dbo.UsuarioSolicitudes", "UsuarioId", "dbo.Usuario");
            DropForeignKey("dbo.UsuarioContrasena", "UsuarioId", "dbo.Usuario");
            DropForeignKey("dbo.UsuarioAsociado", "UsuarioId", "dbo.Usuario");
            DropForeignKey("dbo.UsuarioAsociado", "LiderId", "dbo.Usuario");
            DropForeignKey("dbo.SolicitudEstado", "SupervisorId", "dbo.Usuario");
            DropForeignKey("dbo.SolicitudEstado", "RRHHId", "dbo.Usuario");
            DropForeignKey("dbo.SolicitudEstado", "GerenteId", "dbo.Usuario");
            DropForeignKey("dbo.UsuarioSolicitudes", "LicenciaId", "dbo.Licencia");
            DropForeignKey("dbo.LicenciaConfiguracion", "LicenciaId", "dbo.Licencia");
            DropForeignKey("dbo.SolicitudEstado", "UsuarioSolicitudId", "dbo.UsuarioSolicitudes");
            DropIndex("dbo.GrupoPermiso", new[] { "PermisoId" });
            DropIndex("dbo.GrupoPermiso", new[] { "GrupoUsuarioId" });
            DropIndex("dbo.Posicion", new[] { "DepartamentoId" });
            DropIndex("dbo.UsuarioTardanzas", new[] { "MotivoTardanzaId" });
            DropIndex("dbo.UsuarioTardanzas", new[] { "UsuarioId" });
            DropIndex("dbo.UsuarioContrasena", new[] { "UsuarioId" });
            DropIndex("dbo.UsuarioAsociado", new[] { "UsuarioId" });
            DropIndex("dbo.UsuarioAsociado", new[] { "LiderId" });
            DropIndex("dbo.LicenciaConfiguracion", new[] { "LicenciaId" });
            DropIndex("dbo.UsuarioSolicitudes", new[] { "LicenciaId" });
            DropIndex("dbo.UsuarioSolicitudes", new[] { "UsuarioId" });
            DropIndex("dbo.SolicitudEstado", new[] { "RRHHId" });
            DropIndex("dbo.SolicitudEstado", new[] { "GerenteId" });
            DropIndex("dbo.SolicitudEstado", new[] { "SupervisorId" });
            DropIndex("dbo.SolicitudEstado", new[] { "UsuarioSolicitudId" });
            DropIndex("dbo.Usuario", new[] { "CiudadId" });
            DropIndex("dbo.Usuario", new[] { "PaisId" });
            DropIndex("dbo.Usuario", new[] { "PosicionId" });
            DropIndex("dbo.Usuario", new[] { "DepartamentoId" });
            DropIndex("dbo.Usuario", new[] { "GrupoUsuarioId" });
            DropIndex("dbo.Ciudad", new[] { "PaisId" });
            DropTable("dbo.Pais");
            DropTable("dbo.Permiso");
            DropTable("dbo.GrupoPermiso");
            DropTable("dbo.GrupoUsuario");
            DropTable("dbo.Posicion");
            DropTable("dbo.Departamento");
            DropTable("dbo.MotivoTardanzas");
            DropTable("dbo.UsuarioTardanzas");
            DropTable("dbo.UsuarioContrasena");
            DropTable("dbo.UsuarioAsociado");
            DropTable("dbo.LicenciaConfiguracion");
            DropTable("dbo.Licencia");
            DropTable("dbo.UsuarioSolicitudes");
            DropTable("dbo.SolicitudEstado");
            DropTable("dbo.Usuario");
            DropTable("dbo.Ciudad");
        }
    }
}
