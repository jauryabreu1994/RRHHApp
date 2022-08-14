using RRHHApp.Context;
using RRHHApp.Models.Enums;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Threading.Tasks;

namespace RRHHApp.Controllers
{
    public class GenericController :  Controller
    {
        MainDbContext db = new MainDbContext();
        Encriptar_DesEncriptar encriptar_DesEncriptar = new Encriptar_DesEncriptar();
        public string SetFormatVatNumber(string VatNumber)
        {

            if (new String(VatNumber.Where(Char.IsLetter).ToArray()).Length == 0)
            {
                if (VatNumber.Length == 9)
                {
                    VatNumber = Convert.ToInt64(new String(VatNumber.Where(Char.IsDigit).ToArray())).ToString("###-#####-#");
                }
                else if (VatNumber.Length == 11)
                {
                    VatNumber = Convert.ToInt64(new String(VatNumber.Where(Char.IsDigit).ToArray())).ToString("###-#######-#");
                }
            }

            return VatNumber;
        }

        public bool hasAccess(PermisoEnum permiso, HttpSessionStateBase session) 
        {
            try
            {
                if (session["UserID"] != null)
                {
                    int UserID = Convert.ToInt32(encriptar_DesEncriptar.DesEncriptar(session["UserID"].ToString()));
                    int GrupoUsuarioId = Convert.ToInt32(encriptar_DesEncriptar.DesEncriptar(session["GrupoUsuarioId"].ToString()));

                    var grupoPermisos = db.GrupoPermiso.Include(e => e._GrupoUsuario).Include(e => e._Permiso).Where(a => a.GrupoUsuarioId == GrupoUsuarioId).ToList();

                    return grupoPermisos.Where(a => a.PermisoId == (int)permiso).Count() == 0 ? false : true;
                }
                return false;
            }
            catch (Exception) 
            {
                return false;
            }
        }

        public Tuple<int, int, int> getChief(HttpSessionStateBase session) 
        {
            try
            {
                int[] chief = { 0, 0, 0 };
                int UserID = Convert.ToInt32(encriptar_DesEncriptar.DesEncriptar(session["UserID"].ToString()));

                var rrhh = db.Usuario.Where(a => a.RRHHAutorizado).ToList();
                var rrhhResults = (from r in rrhh orderby Guid.NewGuid() ascending select r).ToList();
                chief[2] = rrhhResults.FirstOrDefault().Id;

                var supervisores = db.UsuarioAsociado.Where(a => a.UsuarioId == UserID).ToList();
                if (supervisores.Count() > 0)
                {
                    var supervisor = supervisores.First();
                    chief[0] = supervisor.LiderId;
                    supervisores = db.UsuarioAsociado.Where(a => a.UsuarioId == supervisor.LiderId).ToList();
                    
                    if (supervisores.Count() > 0)
                        chief[1] = supervisores.First().LiderId;
                    else
                        return Tuple.Create(chief[0], chief[2], chief[2]);
                }
                else 
                    return Tuple.Create(chief[2], chief[2], chief[2]);                

                return Tuple.Create(chief[0], chief[1], chief[2]);
            }
            catch (Exception)
            {
                return Tuple.Create(1, 1, 1);
            }
        }

        public int BusinessDaysUntil(DateTime firstDay, DateTime lastDay)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            if (businessDays > fullWeekCount * 7)
            {
                int firstDayOfWeek = (int)firstDay.DayOfWeek;
                int lastDayOfWeek = (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)
                    businessDays -= 1;
            }
            businessDays -= fullWeekCount + fullWeekCount;

            return businessDays;
        }


        public async Task<Tuple<int, int, int>> getDateAvailable(int usuarioId, int licenciaId, DateTime usuarioFechaDesde, DateTime usuarioFechaHasta, DateTime usuarioIngreso)
        {
            try
            {
                #region Estado de Solicitud
                var dias = (usuarioFechaDesde > usuarioFechaHasta) ? 0 : BusinessDaysUntil(usuarioFechaDesde, usuarioFechaHasta);
                var licencias = db.LicenciaConfiguracion.Where(a => a.LicenciaId == licenciaId).ToListAsync().Result;


                var fecha = DateTime.Now.Date - usuarioIngreso.Date;

                var licencia = licencias.Where(a => a.UsuarioTiempo <= fecha.TotalDays).OrderBy(a => a.UsuarioTiempo).LastOrDefault();
                if (licencia != null)
                {
                    int tiempo = (int)Math.Floor(fecha.TotalDays / 365);
                    DateTime periodoInicio = usuarioIngreso.AddYears(tiempo);
                    DateTime periodoFinal = periodoInicio.AddYears(1);



                    var solicitudes = db.UsuarioSolicitudes.Where(a => a.UsuarioId == usuarioId && !a.Eliminado &&
                                                                        a.Estado == Models.Enums.EstadoEnum.Activo &&
                                                                        a.LicenciaId == licenciaId &&
                                                                        a.FechaDesde >= periodoInicio &&
                                                                        db.SolicitudEstado.Any(p2 => p2.UsuarioSolicitudId == a.Id &&
                                                                                                        p2.GerenteEstado == Models.Enums.SolicitudEstadoEnum.Aprobado &&
                                                                                                        p2.SupervisorEstado == Models.Enums.SolicitudEstadoEnum.Aprobado &&
                                                                                                        p2.RRHHEstado == Models.Enums.SolicitudEstadoEnum.Aprobado))
                                                                    .Include(u => u._Licencia)
                                                                    .Include(u => u._Usuario).ToListAsync().Result;
                    int sumFecha = 0;
                    foreach (var linea in solicitudes)
                        sumFecha += BusinessDaysUntil(linea.FechaDesde, linea.FechaHasta);

                    int diasRestantes = (licencia.Cantidad - sumFecha - dias);
                    #endregion

                    return Tuple.Create(diasRestantes, licencia.Cantidad, sumFecha);
                }
                return Tuple.Create(0, 0, 0);
            }
            catch
            {
                return Tuple.Create(0, 0, 0);
            }
        }
    }
}