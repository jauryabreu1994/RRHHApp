using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RRHHApp.Models.Licencias
{
    public class LicenciaConfiguracion : MainModel
    {
        public int LicenciaId { get; set; }
        public int UsuarioTiempo { get; set; }
        public int Cantidad { get; set; }

        public bool RequiereSupervisorAprobacion { get; set; }
        public bool RequiereGerenteAprobacion { get; set; }
        public bool RequiereRRHHAprobacion { get; set; }

        public virtual Licencia _Licencia { get; set; }

    }
}