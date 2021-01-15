using IncripcionesCobach.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace IncripcionesCobach.Controllers
{
    public class InscripcionController : Controller
    {
        InscripcionesEntities db = new InscripcionesEntities();
        // GET: Inscripcion

        InscripcionViewModel infoPagos = new InscripcionViewModel();

        public ActionResult Index(string email)
        {

            var matricula = email.Split('@');
            var idMatricula = matricula[0];

          Session["Matricula"] = idMatricula;

            var data = db.sp_GetCostoPlantel(idMatricula).FirstOrDefault();

            ViewBag.data = infoPagos;

            if (data != null)
            {

                var grupo = data.CveGrupo;
                int i = Math.Abs(grupo);
                while (i >= 10)
                    i /= 10;

                infoPagos.Semestre = i;
                infoPagos.NombreCompleto = data.NombreCompleto;
                infoPagos.CveGrupo = grupo.ToString();
                infoPagos.Matricula = data.Matricula;
                infoPagos.Referencia = data.Referencia;
                infoPagos.CIE = data.CIE;
                infoPagos.Inscripcion = data.Inscripcion;
                infoPagos.sociedadAlumnos = data.SociedadAlumnos;
                infoPagos.pagoInscripcion = data.pagoInscripcion;
                infoPagos.pagoSA = data.pagoSA;
                infoPagos.Plantel = data.Plantel;
                infoPagos.TurnoM = data.TM;
                infoPagos.TurnoV = data.TV;




                return View(infoPagos);



            }
            else
            {

                infoPagos.Nodata = "No existe información con esa Matricula " + " " + idMatricula;
                return View(infoPagos);
            }

        }

    }
}