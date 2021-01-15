using IncripcionesCobach.Models;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Net.Http;
using System.Web;
using System.Text;

namespace IncripcionesCobach.Controllers
{
    public class PagoController : Controller
    {
        InscripcionesEntities db = new InscripcionesEntities();
        InscripcionViewModel infoPagos = new InscripcionViewModel();
        // GET: Pago
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult pagoInscripcion(string matricula)
        {
            var data = db.sp_GetCostoPlantel(matricula).FirstOrDefault();
            var grupo = data.CveGrupo;
            int i = Math.Abs(grupo);
            while (i >= 10)
                i /= 10;

            infoPagos.Semestre = i;
            infoPagos.NombreCompleto = data.NombreCompleto;
            infoPagos.CveGrupo = grupo.ToString();
            infoPagos.Matricula = data.Matricula;
            infoPagos.CIE = data.CIE;
            infoPagos.Referencia = data.Referencia;
            infoPagos.Inscripcion = data.Inscripcion;

            infoPagos.Fecha = DateTime.Now.Date;
            return View(infoPagos);
        }
        public ActionResult pagoSociedadAlumnos(string matricula)
        {
            var data = db.sp_GetCostoPlantel(matricula).FirstOrDefault();
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
            infoPagos.sociedadAlumnos = data.SociedadAlumnos;
            infoPagos.Fecha = DateTime.Now.Date;
            return View(infoPagos);
        }

        public ActionResult ComprobantePago()
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Title = "Confirmación de pago";

                return View();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult UploadImg(HttpPostedFileBase file)
        {

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Title = "Confirmación de pago";


                if (file != null && file.ContentLength > 0)
                {

                    string nam = file.FileName;

                    nam = nam.Substring(nam.Length - 4);

                    if (nam != ".png" || nam != ".jpeg" || nam != ".jpg" || nam != ".gif")
                    {
                        //string pic = System.IO.Path.GetFileName(file.FileName);
                        string pic = (string)Session["Matricula"] + ".jpg";
                        string matricula = (string)Session["Matricula"];
                        //string path = System.IO.Path.Combine(Server.MapPath("~/Comprobantes"), pic);
                        //string path = "@C:\inetpub\wwwroot\ModuloExtras\Comprobantes\" + pic;
                        //string path = "D:\\inetpub\\wwwroot\\ModuloExtras\\Comprobantes\\" + pic;
                        string path = "D:\\Inscripciones\\" + pic;
                        // file is uploaded
                        file.SaveAs(path);

                        db.sp_UploadPago(matricula,pic);
                        db.SaveChanges();

                        StringBuilder errorMessages = new StringBuilder();
                        
                    }
                    else
                    {
                        ViewBag.Mensaje = "¡FORMATO NO PERMITIDO!";
                    }

                }
                else
                {
                    ViewBag.Mensaje = "¡ERROR!";
                }
                ViewBag.Mensaje = "Realizado con Éxito";
                return View("ComprobantePago");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

            public ActionResult returnPago()
        {
            var matricula = (string)Session["Matricula"];
            db.sp_UpdatePago(matricula, "0", "0");
            db.SaveChanges();
            return RedirectToAction("Index", "Inscripcion");
        }
    }
}