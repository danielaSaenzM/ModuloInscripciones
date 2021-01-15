using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using IncripcionesCobach.Models;
using Microsoft.AspNet.Identity;
using Microsoft.VisualBasic;
using System.IO;

namespace IncripcionesCobach.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index() {
            return View();
        }
        [HttpPost]
        public ActionResult Logg(string v_usu, string v_pass)
        {
            string usu = v_usu;
            string pas = v_pass;
            DataTable dt2 = new DataTable();
            StringBuilder errorMessages = new StringBuilder();
            using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                try
                {
                    using (SqlCommand cmd2 = new SqlCommand("EXT_P_CallusuariosPlanteles", conexion))
                    {
                        if (cmd2.Connection.State == ConnectionState.Closed)
                        {
                            cmd2.Connection.Open();
                        }

                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@usu", usu);
                        cmd2.Parameters.AddWithValue("@pas", pas);

                        using (SqlDataAdapter da2 = new SqlDataAdapter(cmd2))
                        {
                            da2.SelectCommand = cmd2;
                            da2.Fill(dt2);
                            try
                            {
                                if (dt2.Rows.Count > 0)
                                {
                                    foreach (DataRow dbRow in dt2.Rows)
                                    {
                                        Session["idplantel"] = dbRow["idplantel"].ToString();
                                        Session["usu"] = usu;
                                        Session["pas"] = pas;
                                    }
                                    return RedirectToAction("PanelPlantel", "Home");
                                }
                                else
                                {
                                    TempData["logError"] = "Credenciales Incorrectas";
                                    return RedirectToAction("Login", "Account");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    Console.WriteLine("Error SQL:" + errorMessages.ToString());

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Conexion:" + ex.Message);
                }
            }
            return RedirectToAction("Login", "Account");
        }

        public ActionResult IndexAlumno(string mmatricula)
        {
            if (User.Identity.IsAuthenticated)
            {
                string nombre = User.Identity.Name.Split('_')[0];
                string matricula = mmatricula;
                Session["Matricula"] = mmatricula;

                ViewBag.Title = "Consulta tus materias pendientes";
                int countrows = 0;
                DataTable dt = new DataTable();
                StringBuilder errorMessages = new StringBuilder();
                List<MateriasAlumno> lst_MateriasAdeudo = new List<MateriasAlumno>();
                using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("EXT_P_CallAdeudos", conexion))
                        {
                            if (cmd.Connection.State == ConnectionState.Closed)
                            {   cmd.Connection.Open();  }

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@matricula", matricula);

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd)){
                                da.SelectCommand = cmd;
                                if (da.Equals(null)){
                                    TempData["empty"] = "No hay rows";
                                    countrows = 0;
                                }else{
                                    da.Fill(dt);
                                    try{
                                        if (dt.Rows.Count > 0){
                                            foreach (DataRow dbRow in dt.Rows){
                                                countrows = countrows + 1;
                                                lst_MateriasAdeudo.Add(new MateriasAlumno
                                                {
                                                    idAlumno = dbRow["idAlumno"].ToString(),
                                                    Alumno = dbRow["NombreCompleto"].ToString(),
                                                    idPlantel = dbRow["idPlantel"].ToString(),
                                                    idPlantel_ID = dbRow["idPlantel_ID"].ToString(),
                                                    Materia = dbRow["Materia"].ToString(),
                                                    Matricula = dbRow["Matricula"].ToString(),
                                                    CostoMateria = dbRow["CostoMateria"].ToString(),
                                                    CIE = dbRow["CIE"].ToString(),
                                                    idSemestre = dbRow["idSemestre"].ToString(),
                                                    idMateria = dbRow["idMateria"].ToString(),
                                                    Docente = dbRow["Docente"].ToString(),
                                                    Grupo = dbRow["CveGrupo"].ToString(),
                                                    Referencia = dbRow["Referencia"].ToString()
                                                });
                                            }
                                        }
                                        else{
                                            TempData["empty"] = "EMPTY";
                                            countrows = 0;
                                        }
                                    }
                                    catch (SqlException sex)
                                    {   for (int i = 0; i < sex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + sex.Errors[i].Message + "\n" +
                                                "LineNumber: " + sex.Errors[i].LineNumber + "\n" +
                                                "Source: " + sex.Errors[i].Source + "\n" +
                                                "Procedure: " + sex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                    }
                                    catch (Exception ex)
                                    {   Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    catch (SqlException ex){
                        for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append("Index #" + i + "\n" +
                                "Message: " + ex.Errors[i].Message + "\n" +
                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                "Source: " + ex.Errors[i].Source + "\n" +
                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                        }
                        Console.WriteLine("Error SQL:" + errorMessages.ToString());

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Conexion:" + ex.Message);
                    }

                }
                ViewBag.Countrows = countrows;
                return View(lst_MateriasAdeudo);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Index4Admin(string Matricula)
        {
            string usu = (string)Session["usu"];
            string pas = (string)Session["pas"];
            string plantel = (string)Session["idplantel"];

            if (usu != null && pas != null && plantel != null)
            {
                string matricula = Matricula;
                Session["Matricula"] = Matricula;

                ViewBag.Title = "Generar Papeleta para un Estudiante";
                int countrows = 0;
                DataTable dt = new DataTable();
                StringBuilder errorMessages = new StringBuilder();
                List<MateriasAlumno> lst_MateriasAdeudo = new List<MateriasAlumno>();
                using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("EXT_P_CallAdeudos", conexion))
                        {
                            if (cmd.Connection.State == ConnectionState.Closed)
                            { cmd.Connection.Open(); }

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@matricula", matricula);

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.SelectCommand = cmd;
                                if (da.Equals(null))
                                {
                                    TempData["empty"] = "No hay rows";
                                    countrows = 0;
                                }
                                else
                                {
                                    da.Fill(dt);
                                    try
                                    {
                                        if (dt.Rows.Count > 0)
                                        {
                                            foreach (DataRow dbRow in dt.Rows)
                                            {
                                                countrows = countrows + 1;
                                                lst_MateriasAdeudo.Add(new MateriasAlumno
                                                {
                                                    idAlumno = dbRow["idAlumno"].ToString(),
                                                    Alumno = dbRow["NombreCompleto"].ToString(),
                                                    idPlantel = dbRow["idPlantel"].ToString(),
                                                    idPlantel_ID = dbRow["idPlantel_ID"].ToString(),
                                                    Materia = dbRow["Materia"].ToString(),
                                                    Matricula = dbRow["Matricula"].ToString(),
                                                    CostoMateria = dbRow["CostoMateria"].ToString(),
                                                    CIE = dbRow["CIE"].ToString(),
                                                    idSemestre = dbRow["idSemestre"].ToString(),
                                                    idMateria = dbRow["idMateria"].ToString(),
                                                    Docente = dbRow["Docente"].ToString(),
                                                    Grupo = dbRow["CveGrupo"].ToString(),
                                                    Referencia = dbRow["Referencia"].ToString()
                                                });
                                            }
                                        }
                                        else
                                        {
                                            TempData["empty"] = "EMPTY";
                                            countrows = 0;
                                        }
                                    }
                                    catch (SqlException sex)
                                    {
                                        for (int i = 0; i < sex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + sex.Errors[i].Message + "\n" +
                                                "LineNumber: " + sex.Errors[i].LineNumber + "\n" +
                                                "Source: " + sex.Errors[i].Source + "\n" +
                                                "Procedure: " + sex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append("Index #" + i + "\n" +
                                "Message: " + ex.Errors[i].Message + "\n" +
                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                "Source: " + ex.Errors[i].Source + "\n" +
                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                        }
                        Console.WriteLine("Error SQL:" + errorMessages.ToString());

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Conexion:" + ex.Message);
                    }

                }
                ViewBag.Countrows = countrows;
                return View(lst_MateriasAdeudo);
            }
            else
            {
                return RedirectToAction("LoginAdmin", "Account");
            }
        }

        public ActionResult Upload()
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
                            string path = "D:\\Comprobantes\\" + pic;
                            // file is uploaded
                            file.SaveAs(path);

                            DataTable dt = new DataTable();
                            StringBuilder errorMessages = new StringBuilder();
                            using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                            {
                                try{
                                    using (SqlCommand cmd = new SqlCommand("EXT_P_Update_PapeletasAdeudo", conexion)){
                                        try{
                                            if (cmd.Connection.State == ConnectionState.Closed)
                                            {
                                                cmd.Connection.Open();
                                            }

                                            cmd.CommandType = CommandType.StoredProcedure;
                                            cmd.Parameters.AddWithValue("@Matricula", matricula);
                                            cmd.Parameters.AddWithValue("@Comprobante", pic);

                                            SqlDataReader reader3 = cmd.ExecuteReader();
                                            while (reader3.Read())
                                            {
                                                Console.WriteLine("Good");
                                            }
                                            ViewBag.Mensaje = "¡Comprobante actualizado, ya puedes cerrar ésta ventana!";

                                            reader3.Close();
                                            conexion.Close();
                                        }
                                        catch (SqlException ex)
                                        {
                                            for (int i = 0; i < ex.Errors.Count; i++)
                                            {
                                                errorMessages.Append("Index #" + i + "\n" +
                                                    "Message: " + ex.Errors[i].Message + "\n" +
                                                    "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                    "Source: " + ex.Errors[i].Source + "\n" +
                                                    "Procedure: " + ex.Errors[i].Procedure + "\n");
                                            }
                                            Console.WriteLine(errorMessages.ToString());
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine("Error Conexion:" + ex.Message);
                                        }
                                    }
                                }
                                catch (SqlException ex){
                                    for (int i = 0; i < ex.Errors.Count; i++)
                                    {
                                        errorMessages.Append("Index #" + i + "\n" +
                                            "Message: " + ex.Errors[i].Message + "\n" +
                                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                            "Source: " + ex.Errors[i].Source + "\n" +
                                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                                    }
                                    Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                }
                                catch (Exception ex){
                                    Console.WriteLine("Error Conexion:" + ex.Message);
                                }
                            }
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

                return View("Upload");
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "Ayuda y Soporte";
            return View();
        }

        [HttpPost]
        public ActionResult PickDocente(string Materia1, string Materia2, string Materia3, string Materia4, string idPlantel_tr, string idPlantel_ID_tr, string Referencia,
                                     string Alumno_tr, string Matricula_tr, string CostoMateria_tr, string CIE_tr, string idAlumno_tr,
                                     string idSemestre1, string idSemestre2, string idSemestre3, string idSemestre4,
                                     string idMateria1, string idMateria2, string idMateria3, string idMateria4,
                                     string Docente1, string Docente2, string Docente3, string Docente4,
                                     string Grupo1, string Grupo2, string Grupo3, string Grupo4,
                                     string DatExam1, string DatExam2, string DatExam3, string DatExam4)
        {
            if (User.Identity.IsAuthenticated)
            {
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                DataTable dt4 = new DataTable();
                List<PapeletaModel> lst_Papeleta = new List<PapeletaModel>();
                List<DocentesModel> lst_Docentes1 = new List<DocentesModel>();
                List<DocentesModel> lst_Docentes2 = new List<DocentesModel>();
                List<DocentesModel> lst_Docentes3 = new List<DocentesModel>();
                List<DocentesModel> lst_Docentes4 = new List<DocentesModel>();
                StringBuilder errorMessages = new StringBuilder();

                if (Materia1 == "" || Materia1 == null)
                {
                    Materia1 = "";
                }
                if (Materia2 == "" || Materia2 == null)
                {
                    Materia2 = "";
                }
                if (Materia3 == "" || Materia3 == null)
                {
                    Materia3 = "";
                }
                if (Materia4 == "" || Materia4 == null)
                {
                    Materia4 = "";
                }
                if (Docente1 == "" || Docente1 == " " || Docente1 == null)
                {
                    Docente1 = "";
                }
                if (Docente2 == "" || Docente2 == " " || Docente2 == null)
                {
                    Docente2 = "";
                }
                if (Docente3 == "" || Docente3 == " " || Docente3 == null)
                {
                    Docente3 = "";
                }
                if (Docente4 == "" || Docente4 == " " || Docente4 == null)
                {
                    Docente4 = "";
                }

                using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    try
                    {
                        if (Materia1 != "" && Docente1 == "")
                        {
                            using (SqlCommand cmd = new SqlCommand("EXT_P_CallDocentes", conexion))
                            {
                                if (cmd.Connection.State == ConnectionState.Closed)
                                {
                                    cmd.Connection.Open();
                                }

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idMateria", idMateria1);
                                cmd.Parameters.AddWithValue("@idPlantel", idPlantel_ID_tr);
                                cmd.Parameters.AddWithValue("@idAlumno", idAlumno_tr);

                                using (SqlDataAdapter da1 = new SqlDataAdapter(cmd))
                                {
                                    da1.SelectCommand = cmd;
                                    da1.Fill(dt1);
                                    try
                                    {
                                        if (dt1.Rows.Count > 0)
                                        {
                                            foreach (DataRow dbRow in dt1.Rows)
                                            {
                                                lst_Docentes1.Add(new DocentesModel
                                                {
                                                    id = dbRow["idMaestro"].ToString(),
                                                    Nombre = dbRow["Opcion"].ToString()
                                                });
                                            }
                                        }
                                        else
                                        {
                                            lst_Docentes1.Add(new DocentesModel
                                            {
                                                id = "0",
                                                Nombre = "No hay docentes disponibles"
                                            });
                                        }
                                        ViewBag.Docentes1 = lst_Docentes1;
                                    }
                                    catch (SqlException ex){
                                        for (int i = 0; i < ex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + ex.Errors[i].Message + "\n" +
                                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                "Source: " + ex.Errors[i].Source + "\n" +
                                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                    }
                                    catch (Exception ex){
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }else{
                            lst_Docentes1.Add(new DocentesModel
                            {
                                id = "0",
                                Nombre = Docente1
                            });
                            ViewBag.Docentes1 = lst_Docentes1;
                        }
                        if (Materia2 != "" && Docente2 == "")
                        {
                            using (SqlCommand cmd = new SqlCommand("EXT_P_CallDocentes", conexion))
                            {
                                if (cmd.Connection.State == ConnectionState.Closed)
                                {
                                    cmd.Connection.Open();
                                }

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idMateria", idMateria2);
                                cmd.Parameters.AddWithValue("@idPlantel", idPlantel_ID_tr);
                                cmd.Parameters.AddWithValue("@idAlumno", idAlumno_tr);

                                using (SqlDataAdapter da2 = new SqlDataAdapter(cmd))
                                {
                                    da2.SelectCommand = cmd;
                                    da2.Fill(dt2);
                                    try
                                    {
                                        if (dt2.Rows.Count > 0)
                                        {
                                            foreach (DataRow dbRow in dt2.Rows)
                                            {
                                                lst_Docentes2.Add(new DocentesModel
                                                {
                                                    id = dbRow["idMaestro"].ToString(),
                                                    Nombre = dbRow["Opcion"].ToString()
                                                });
                                            }
                                        }
                                        else{
                                            lst_Docentes2.Add(new DocentesModel
                                            {
                                                id = "0",
                                                Nombre = "No hay docentes disponibles"
                                            });
                                        }
                                        ViewBag.Docentes2 = lst_Docentes2;
                                    }
                                    catch (SqlException ex){
                                        for (int i = 0; i < ex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + ex.Errors[i].Message + "\n" +
                                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                "Source: " + ex.Errors[i].Source + "\n" +
                                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                    }
                                    catch (Exception ex){
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }else{
                            lst_Docentes2.Add(new DocentesModel
                            {
                                id = "0",
                                Nombre = Docente2
                            });
                            ViewBag.Docentes2 = lst_Docentes2;
                        }
                        if (Materia3 != "" && Docente3 == "")
                        {
                            using (SqlCommand cmd = new SqlCommand("EXT_P_CallDocentes", conexion))
                            {
                                if (cmd.Connection.State == ConnectionState.Closed)
                                {
                                    cmd.Connection.Open();
                                }

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idMateria", idMateria3);
                                cmd.Parameters.AddWithValue("@idPlantel", idPlantel_ID_tr);
                                cmd.Parameters.AddWithValue("@idAlumno", idAlumno_tr);

                                using (SqlDataAdapter da3 = new SqlDataAdapter(cmd))
                                {
                                    da3.SelectCommand = cmd;
                                    da3.Fill(dt3);
                                    try
                                    {
                                        if (dt3.Rows.Count > 0)
                                        {
                                            foreach (DataRow dbRow in dt3.Rows)
                                            {
                                                lst_Docentes3.Add(new DocentesModel
                                                {
                                                    id = dbRow["idMaestro"].ToString(),
                                                    Nombre = dbRow["Opcion"].ToString()
                                                });
                                            }
                                        }else{
                                            lst_Docentes3.Add(new DocentesModel
                                            {
                                                id = "0",
                                                Nombre = "No hay docentes disponibles"
                                            });
                                        }
                                        ViewBag.Docentes3 = lst_Docentes3;
                                    }
                                    catch (SqlException ex){
                                        for (int i = 0; i < ex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + ex.Errors[i].Message + "\n" +
                                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                "Source: " + ex.Errors[i].Source + "\n" +
                                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                    }
                                    catch (Exception ex){
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }else{
                            lst_Docentes3.Add(new DocentesModel
                            {
                                id = "0",
                                Nombre = Docente3
                            });
                            ViewBag.Docentes3 = lst_Docentes3;
                        }
                        if (Materia4 != "" && Docente4 == ""){
                            using (SqlCommand cmd = new SqlCommand("EXT_P_CallDocentes", conexion))
                            {
                                if (cmd.Connection.State == ConnectionState.Closed)
                                {
                                    cmd.Connection.Open();
                                }

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idMateria", idMateria4);
                                cmd.Parameters.AddWithValue("@idPlantel", idPlantel_ID_tr);
                                cmd.Parameters.AddWithValue("@idAlumno", idAlumno_tr);

                                using (SqlDataAdapter da4 = new SqlDataAdapter(cmd))
                                {
                                    da4.SelectCommand = cmd;
                                    da4.Fill(dt4);
                                    try
                                    {
                                        if (dt4.Rows.Count > 0)
                                        {
                                            foreach (DataRow dbRow in dt4.Rows)
                                            {
                                                lst_Docentes4.Add(new DocentesModel
                                                {
                                                    id = dbRow["idMaestro"].ToString(),
                                                    Nombre = dbRow["Opcion"].ToString()
                                                });
                                            }
                                        }
                                        else{
                                            lst_Docentes4.Add(new DocentesModel
                                            {
                                                id = "0",
                                                Nombre = "No hay docentes disponibles"
                                            });
                                        }
                                        ViewBag.Docentes4 = lst_Docentes4;
                                    }
                                    catch (SqlException ex){
                                        for (int i = 0; i < ex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + ex.Errors[i].Message + "\n" +
                                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                "Source: " + ex.Errors[i].Source + "\n" +
                                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }else{
                            lst_Docentes4.Add(new DocentesModel
                            {
                                id = "0",
                                Nombre = Docente4
                            });
                            ViewBag.Docentes4 = lst_Docentes4;
                        }

                        lst_Papeleta.Add(new PapeletaModel
                        {
                            Materia1 = Materia1,
                            Materia2 = Materia2,
                            Materia3 = Materia3,
                            Materia4 = Materia4,
                            idPlantel_tr = idPlantel_tr,
                            idPlantel_ID_tr = idPlantel_ID_tr,
                            Alumno_tr = Alumno_tr,
                            Matricula_tr = Matricula_tr,
                            CostoMateria_tr = CostoMateria_tr,
                            CIE_tr = CIE_tr,
                            idAlumno_tr = idAlumno_tr,
                            idSemestre1 = idSemestre1,
                            idSemestre2 = idSemestre2,
                            idSemestre3 = idSemestre3,
                            idSemestre4 = idSemestre4,
                            idMateria1 = idMateria1,
                            idMateria2 = idMateria2,
                            idMateria3 = idMateria3,
                            idMateria4 = idMateria4,
                            Docente1 = Docente1,
                            Docente2 = Docente2,
                            Docente3 = Docente3,
                            Docente4 = Docente4,
                            Grupo1 = Grupo1,
                            Grupo2 = Grupo2,
                            Grupo3 = Grupo3,
                            Grupo4 = Grupo4,
                            DatExam1 = DatExam1,
                            DatExam2 = DatExam2,
                            DatExam3 = DatExam3,
                            DatExam4 = DatExam4,
                            Referencia = Referencia
                        });
                        ViewBag.Papeleta = lst_Papeleta;
                    }
                    catch (SqlException ex){
                        for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append("Index #" + i + "\n" +
                                "Message: " + ex.Errors[i].Message + "\n" +
                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                "Source: " + ex.Errors[i].Source + "\n" +
                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                        }
                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                    }
                    catch (Exception ex){
                        Console.WriteLine("Error Conexion:" + ex.Message);
                    }

                    return View();
                }
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult PickDocente4Admin(string Materia1, string Materia2, string Materia3, string Materia4, string idPlantel_tr, string idPlantel_ID_tr, string Referencia,
                                     string Alumno_tr, string Matricula_tr, string CostoMateria_tr, string CIE_tr, string idAlumno_tr,
                                     string idSemestre1, string idSemestre2, string idSemestre3, string idSemestre4,
                                     string idMateria1, string idMateria2, string idMateria3, string idMateria4,
                                     string Docente1, string Docente2, string Docente3, string Docente4,
                                     string Grupo1, string Grupo2, string Grupo3, string Grupo4,
                                     string DatExam1, string DatExam2, string DatExam3, string DatExam4)
        {
            string usu = (string)Session["usu"];
            string pas = (string)Session["pas"];
            string plantel = (string)Session["idplantel"];

            if (usu != null && pas != null && plantel != null)
            {
                DataTable dt1 = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                DataTable dt4 = new DataTable();
                List<PapeletaModel> lst_Papeleta = new List<PapeletaModel>();
                List<DocentesModel> lst_Docentes1 = new List<DocentesModel>();
                List<DocentesModel> lst_Docentes2 = new List<DocentesModel>();
                List<DocentesModel> lst_Docentes3 = new List<DocentesModel>();
                List<DocentesModel> lst_Docentes4 = new List<DocentesModel>();
                StringBuilder errorMessages = new StringBuilder();

                if (Materia1 == "" || Materia1 == null)
                {
                    Materia1 = "";
                }
                if (Materia2 == "" || Materia2 == null)
                {
                    Materia2 = "";
                }
                if (Materia3 == "" || Materia3 == null)
                {
                    Materia3 = "";
                }
                if (Materia4 == "" || Materia4 == null)
                {
                    Materia4 = "";
                }
                if (Docente1 == "" || Docente1 == " " || Docente1 == null)
                {
                    Docente1 = "";
                }
                if (Docente2 == "" || Docente2 == " " || Docente2 == null)
                {
                    Docente2 = "";
                }
                if (Docente3 == "" || Docente3 == " " || Docente3 == null)
                {
                    Docente3 = "";
                }
                if (Docente4 == "" || Docente4 == " " || Docente4 == null)
                {
                    Docente4 = "";
                }

                using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    try
                    {
                        if (Materia1 != "" && Docente1 == "")
                        {
                            using (SqlCommand cmd = new SqlCommand("EXT_P_CallDocentes", conexion))
                            {
                                if (cmd.Connection.State == ConnectionState.Closed)
                                {
                                    cmd.Connection.Open();
                                }

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idMateria", idMateria1);
                                cmd.Parameters.AddWithValue("@idPlantel", idPlantel_ID_tr);
                                cmd.Parameters.AddWithValue("@idAlumno", idAlumno_tr);

                                using (SqlDataAdapter da1 = new SqlDataAdapter(cmd))
                                {
                                    da1.SelectCommand = cmd;
                                    da1.Fill(dt1);
                                    try
                                    {
                                        if (dt1.Rows.Count > 0)
                                        {
                                            foreach (DataRow dbRow in dt1.Rows)
                                            {
                                                lst_Docentes1.Add(new DocentesModel
                                                {
                                                    id = dbRow["idMaestro"].ToString(),
                                                    Nombre = dbRow["Opcion"].ToString()
                                                });
                                            }
                                        }
                                        else
                                        {
                                            lst_Docentes1.Add(new DocentesModel
                                            {
                                                id = "0",
                                                Nombre = "No hay docentes disponibles"
                                            });
                                        }
                                        ViewBag.Docentes1 = lst_Docentes1;
                                    }
                                    catch (SqlException ex)
                                    {
                                        for (int i = 0; i < ex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + ex.Errors[i].Message + "\n" +
                                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                "Source: " + ex.Errors[i].Source + "\n" +
                                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            lst_Docentes1.Add(new DocentesModel
                            {
                                id = "0",
                                Nombre = Docente1
                            });
                            ViewBag.Docentes1 = lst_Docentes1;
                        }
                        if (Materia2 != "" && Docente2 == "")
                        {
                            using (SqlCommand cmd = new SqlCommand("EXT_P_CallDocentes", conexion))
                            {
                                if (cmd.Connection.State == ConnectionState.Closed)
                                {
                                    cmd.Connection.Open();
                                }

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idMateria", idMateria2);
                                cmd.Parameters.AddWithValue("@idPlantel", idPlantel_ID_tr);
                                cmd.Parameters.AddWithValue("@idAlumno", idAlumno_tr);

                                using (SqlDataAdapter da2 = new SqlDataAdapter(cmd))
                                {
                                    da2.SelectCommand = cmd;
                                    da2.Fill(dt2);
                                    try
                                    {
                                        if (dt2.Rows.Count > 0)
                                        {
                                            foreach (DataRow dbRow in dt2.Rows)
                                            {
                                                lst_Docentes2.Add(new DocentesModel
                                                {
                                                    id = dbRow["idMaestro"].ToString(),
                                                    Nombre = dbRow["Opcion"].ToString()
                                                });
                                            }
                                        }
                                        else
                                        {
                                            lst_Docentes2.Add(new DocentesModel
                                            {
                                                id = "0",
                                                Nombre = "No hay docentes disponibles"
                                            });
                                        }
                                        ViewBag.Docentes2 = lst_Docentes2;
                                    }
                                    catch (SqlException ex)
                                    {
                                        for (int i = 0; i < ex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + ex.Errors[i].Message + "\n" +
                                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                "Source: " + ex.Errors[i].Source + "\n" +
                                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            lst_Docentes2.Add(new DocentesModel
                            {
                                id = "0",
                                Nombre = Docente2
                            });
                            ViewBag.Docentes2 = lst_Docentes2;
                        }
                        if (Materia3 != "" && Docente3 == "")
                        {
                            using (SqlCommand cmd = new SqlCommand("EXT_P_CallDocentes", conexion))
                            {
                                if (cmd.Connection.State == ConnectionState.Closed)
                                {
                                    cmd.Connection.Open();
                                }

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idMateria", idMateria3);
                                cmd.Parameters.AddWithValue("@idPlantel", idPlantel_ID_tr);
                                cmd.Parameters.AddWithValue("@idAlumno", idAlumno_tr);

                                using (SqlDataAdapter da3 = new SqlDataAdapter(cmd))
                                {
                                    da3.SelectCommand = cmd;
                                    da3.Fill(dt3);
                                    try
                                    {
                                        if (dt3.Rows.Count > 0)
                                        {
                                            foreach (DataRow dbRow in dt3.Rows)
                                            {
                                                lst_Docentes3.Add(new DocentesModel
                                                {
                                                    id = dbRow["idMaestro"].ToString(),
                                                    Nombre = dbRow["Opcion"].ToString()
                                                });
                                            }
                                        }
                                        else
                                        {
                                            lst_Docentes3.Add(new DocentesModel
                                            {
                                                id = "0",
                                                Nombre = "No hay docentes disponibles"
                                            });
                                        }
                                        ViewBag.Docentes3 = lst_Docentes3;
                                    }
                                    catch (SqlException ex)
                                    {
                                        for (int i = 0; i < ex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + ex.Errors[i].Message + "\n" +
                                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                "Source: " + ex.Errors[i].Source + "\n" +
                                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            lst_Docentes3.Add(new DocentesModel
                            {
                                id = "0",
                                Nombre = Docente3
                            });
                            ViewBag.Docentes3 = lst_Docentes3;
                        }
                        if (Materia4 != "" && Docente4 == "")
                        {
                            using (SqlCommand cmd = new SqlCommand("EXT_P_CallDocentes", conexion))
                            {
                                if (cmd.Connection.State == ConnectionState.Closed)
                                {
                                    cmd.Connection.Open();
                                }

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idMateria", idMateria4);
                                cmd.Parameters.AddWithValue("@idPlantel", idPlantel_ID_tr);
                                cmd.Parameters.AddWithValue("@idAlumno", idAlumno_tr);

                                using (SqlDataAdapter da4 = new SqlDataAdapter(cmd))
                                {
                                    da4.SelectCommand = cmd;
                                    da4.Fill(dt4);
                                    try
                                    {
                                        if (dt4.Rows.Count > 0)
                                        {
                                            foreach (DataRow dbRow in dt4.Rows)
                                            {
                                                lst_Docentes4.Add(new DocentesModel
                                                {
                                                    id = dbRow["idMaestro"].ToString(),
                                                    Nombre = dbRow["Opcion"].ToString()
                                                });
                                            }
                                        }
                                        else
                                        {
                                            lst_Docentes4.Add(new DocentesModel
                                            {
                                                id = "0",
                                                Nombre = "No hay docentes disponibles"
                                            });
                                        }
                                        ViewBag.Docentes4 = lst_Docentes4;
                                    }
                                    catch (SqlException ex)
                                    {
                                        for (int i = 0; i < ex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + ex.Errors[i].Message + "\n" +
                                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                "Source: " + ex.Errors[i].Source + "\n" +
                                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                        else
                        {
                            lst_Docentes4.Add(new DocentesModel
                            {
                                id = "0",
                                Nombre = Docente4
                            });
                            ViewBag.Docentes4 = lst_Docentes4;
                        }

                        lst_Papeleta.Add(new PapeletaModel
                        {
                            Materia1 = Materia1,
                            Materia2 = Materia2,
                            Materia3 = Materia3,
                            Materia4 = Materia4,
                            idPlantel_tr = idPlantel_tr,
                            idPlantel_ID_tr = idPlantel_ID_tr,
                            Alumno_tr = Alumno_tr,
                            Matricula_tr = Matricula_tr,
                            CostoMateria_tr = CostoMateria_tr,
                            CIE_tr = CIE_tr,
                            idAlumno_tr = idAlumno_tr,
                            idSemestre1 = idSemestre1,
                            idSemestre2 = idSemestre2,
                            idSemestre3 = idSemestre3,
                            idSemestre4 = idSemestre4,
                            idMateria1 = idMateria1,
                            idMateria2 = idMateria2,
                            idMateria3 = idMateria3,
                            idMateria4 = idMateria4,
                            Docente1 = Docente1,
                            Docente2 = Docente2,
                            Docente3 = Docente3,
                            Docente4 = Docente4,
                            Grupo1 = Grupo1,
                            Grupo2 = Grupo2,
                            Grupo3 = Grupo3,
                            Grupo4 = Grupo4,
                            DatExam1 = DatExam1,
                            DatExam2 = DatExam2,
                            DatExam3 = DatExam3,
                            DatExam4 = DatExam4,
                            Referencia = Referencia
                        });
                        ViewBag.Papeleta = lst_Papeleta;
                    }
                    catch (SqlException ex)
                    {
                        for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append("Index #" + i + "\n" +
                                "Message: " + ex.Errors[i].Message + "\n" +
                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                "Source: " + ex.Errors[i].Source + "\n" +
                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                        }
                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Conexion:" + ex.Message);
                    }

                    return View();
                }
            }
            else
            {
                return RedirectToAction("LoginAdmin", "Account");
            }
        }

        [HttpPost]
        public ActionResult Papeleta(string Materia1, string Materia2, string Materia3, string Materia4, string idPlantel_tr, string Referencia,
                                     string Alumno_tr, string Matricula_tr, string CostoMateria_tr, string CIE_tr, string idAlumno_tr,
                                     string idSemestre1, string idSemestre2, string idSemestre3, string idSemestre4,
                                     string idMateria1, string idMateria2, string idMateria3, string idMateria4,
                                     string Docente1, string Docente2, string Docente3, string Docente4,
                                     string Grupo1, string Grupo2, string Grupo3, string Grupo4,
                                     string DatExam1, string DatExam2, string DatExam3, string DatExam4)
        {
            if (User.Identity.IsAuthenticated)
            {
                ViewBag.Title = "Generar Papeleta de Pago";
                string FechaGenerada = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                //string Referencia;
                string costoM = CostoMateria_tr.Substring(1, 6);
                int CostoMulti=0;
                double costoMateria = Convert.ToDouble(costoM);
                double total = 0.00;
                string totals="$";
                string idMaestro1, idMaestro2, idMaestro3, idMaestro4;
                StringBuilder errorMessages = new StringBuilder();
                if (Materia1 == "" || Materia1 == null)
                {
                    Materia1 = "";
                }
                if (Materia2 == "" || Materia2 == null)
                {
                    Materia2 = "";
                }
                if (Materia3 == "" || Materia3 == null)
                {
                    Materia3 = "";
                }
                if (Materia4 == "" || Materia4 == null)
                { 
                    Materia4 = "";
                }
                if (Docente1 == "" || Docente1 == " " || Docente1 == null)
                {
                    Docente1 = "";
                    idMaestro1 = "";
                }
                else 
                {
                    if(Docente1.Substring(0, 1) == "0")
                    {
                        idMaestro1 = "0";
                        Docente1 = Docente1.Substring(2);
                    }
                    else 
                    {
                        idMaestro1 = Docente1.Substring(0, 4);
                        Docente1 = Docente1.Substring(5);
                    }
                }
                if (Docente2 == "" || Docente2 == " " || Docente2 == null)
                {
                    Docente2 = "";
                    idMaestro2 = "";
                }
                else
                {
                    if (Docente2.Substring(0,1) == "0")
                    {
                        idMaestro2 = "0";
                        Docente2 = Docente2.Substring(2);
                    }
                    else
                    {
                        idMaestro2 = Docente2.Substring(0, 4);
                        Docente2 = Docente2.Substring(5);
                    }
                }
                if (Docente3 == "" || Docente3 == " " || Docente3 == null)
                {
                    Docente3 = "";
                    idMaestro3 = "";
                }
                else
                {
                    if (Docente3.Substring(0, 1) == "0")
                    {
                        idMaestro3 = "0";
                        Docente3 = Docente3.Substring(2);
                    }
                    else
                    {
                        idMaestro3 = Docente3.Substring(0, 4);
                        Docente3 = Docente3.Substring(5);
                    }
                }
                if (Docente4 == "" || Docente4 == " " || Docente4 == null)
                {
                    Docente4 = "";
                    idMaestro4 = "";
                }
                else
                {
                    if (Docente4.Substring(0, 1) == "0")
                    {
                        idMaestro4 = "0";
                        Docente4 = Docente4.Substring(2);
                    }
                    else
                    {
                        idMaestro4 = Docente4.Substring(0, 4);
                        Docente4 = Docente4.Substring(5);
                    }

                }

                string[] Materias = { Materia1, Materia2, Materia3, Materia4 };
                string[] idMaterias = { idMateria1, idMateria2, idMateria3, idMateria4 };
                string[] idSemestres = { idSemestre1, idSemestre2, idSemestre3, idSemestre4 };
                string[] idDocente = { Docente1, Docente2, Docente3, Docente4 };
                string[] idMaestro = { idMaestro1, idMaestro2, idMaestro3, idMaestro4 };
                string[] idGrupo = { Grupo1, Grupo2, Grupo3, Grupo4 };
                string[] DatExams = { DatExam1, DatExam2, DatExam3, DatExam4 };

                //Referencia = Matricula_tr + digitoVerificador00(Matricula_tr);

                using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    try
                    {
                        int row = 0;
                        foreach (string elote in Materias)
                        {
                            if (elote != "")
                            {
                                CostoMulti = CostoMulti + 1;
                                using (SqlCommand cmd = new SqlCommand("EXT_P_Insert_PapeletasAdeudo", conexion))
                                {
                                    try
                                    {
                                        if (cmd.Connection.State == ConnectionState.Closed)
                                        {
                                            cmd.Connection.Open();
                                        }

                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@Materia", elote);
                                        cmd.Parameters.AddWithValue("@idPlantel", idPlantel_tr);
                                        cmd.Parameters.AddWithValue("@Alumno", Alumno_tr);
                                        cmd.Parameters.AddWithValue("@Matricula", Matricula_tr);
                                        cmd.Parameters.AddWithValue("@CostoMateria", CostoMateria_tr);
                                        cmd.Parameters.AddWithValue("@CIE", CIE_tr);
                                        cmd.Parameters.AddWithValue("@Referencia", Referencia);
                                        cmd.Parameters.AddWithValue("@FechaGenerada", FechaGenerada);
                                        cmd.Parameters.AddWithValue("@Docente", idDocente[row]);
                                        cmd.Parameters.AddWithValue("@Grupo", idGrupo[row]);
                                        cmd.Parameters.AddWithValue("@FechaAlta", FechaGenerada);
                                        cmd.Parameters.AddWithValue("@UsuarioAlta", Matricula_tr);
                                        cmd.Parameters.AddWithValue("@idSemestre", idSemestres[row]);
                                        cmd.Parameters.AddWithValue("@idMateria", idMaterias[row]);
                                        cmd.Parameters.AddWithValue("@idAlumno", idAlumno_tr);
                                        cmd.Parameters.AddWithValue("@FechaExamen", DatExams[row]); 
                                        cmd.Parameters.AddWithValue("@idMaestro", idMaestro[row]);
                                        //cmd.Parameters.AddWithValue("@FechaModificacion", FechaModificacion);
                                        //cmd.Parameters.AddWithValue("@UsuarioModificacion", UsuarioModificacion);

                                        SqlDataReader reader3 = cmd.ExecuteReader();
                                        while (reader3.Read())
                                        {
                                            //Luego guarda ese Id y le suma 1 para el nuevo insert
                                            Console.WriteLine("Good");
                                        }

                                        reader3.Close();
                                        conexion.Close();
                                    }
                                    catch (SqlException ex)
                                    {
                                        for (int i = 0; i < ex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + ex.Errors[i].Message + "\n" +
                                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                "Source: " + ex.Errors[i].Source + "\n" +
                                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine(errorMessages.ToString());
                                    }
                                    catch (Exception ex)
                                    {Console.WriteLine("Error Conexion:" + ex.Message);
                                    }
                                }
                                row++;
                            }                            
                        }
                        total = costoMateria * CostoMulti;
                        totals = "$ " + total.ToString() + ".00";
                    }
                    catch (SqlException ex)
                    {
                        for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append("Index #" + i + "\n" +
                                "Message: " + ex.Errors[i].Message + "\n" +
                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                "Source: " + ex.Errors[i].Source + "\n" +
                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                        }
                        Console.WriteLine("Error SQL:" + errorMessages.ToString());

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Conexion:" + ex.Message);
                    }
                }
                return RedirectToAction("DisplayPapeleta", new{ Matricula = Matricula_tr, idPlantel = idPlantel_tr, Alumno = Alumno_tr, CostoMateria = totals, CIE = CIE_tr, Referencia = Referencia, FechaGenerada = FechaGenerada });
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public ActionResult Papeleta4Admin(string Materia1, string Materia2, string Materia3, string Materia4, string idPlantel_tr, string Referencia,
                                     string Alumno_tr, string Matricula_tr, string CostoMateria_tr, string CIE_tr, string idAlumno_tr,
                                     string idSemestre1, string idSemestre2, string idSemestre3, string idSemestre4,
                                     string idMateria1, string idMateria2, string idMateria3, string idMateria4,
                                     string Docente1, string Docente2, string Docente3, string Docente4,
                                     string Grupo1, string Grupo2, string Grupo3, string Grupo4,
                                     string DatExam1, string DatExam2, string DatExam3, string DatExam4)
        {
            string usu = (string)Session["usu"];
            string pas = (string)Session["pas"];
            string plantel = (string)Session["idplantel"];

            if (usu != null && pas != null && plantel != null)
            {
                ViewBag.Title = "Generar Papeleta de Pago";
                string FechaGenerada = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
                //string Referencia;
                string costoM = CostoMateria_tr.Substring(1, 6);
                int CostoMulti = 0;
                double costoMateria = Convert.ToDouble(costoM);
                double total = 0.00;
                string totals = "$";
                string idMaestro1, idMaestro2, idMaestro3, idMaestro4;
                StringBuilder errorMessages = new StringBuilder();
                if (Materia1 == "" || Materia1 == null)
                {
                    Materia1 = "";
                }
                if (Materia2 == "" || Materia2 == null)
                {
                    Materia2 = "";
                }
                if (Materia3 == "" || Materia3 == null)
                {
                    Materia3 = "";
                }
                if (Materia4 == "" || Materia4 == null)
                {
                    Materia4 = "";
                }
                if (Docente1 == "" || Docente1 == " " || Docente1 == null)
                {
                    Docente1 = "";
                    idMaestro1 = "";
                }
                else
                {
                    if (Docente1.Substring(0, 1) == "0")
                    {
                        idMaestro1 = "0";
                        Docente1 = Docente1.Substring(2);
                    }
                    else
                    {
                        idMaestro1 = Docente1.Substring(0, 4);
                        Docente1 = Docente1.Substring(5);
                    }
                }
                if (Docente2 == "" || Docente2 == " " || Docente2 == null)
                {
                    Docente2 = "";
                    idMaestro2 = "";
                }
                else
                {
                    if (Docente2.Substring(0, 1) == "0")
                    {
                        idMaestro2 = "0";
                        Docente2 = Docente2.Substring(2);
                    }
                    else
                    {
                        idMaestro2 = Docente2.Substring(0, 4);
                        Docente2 = Docente2.Substring(5);
                    }
                }
                if (Docente3 == "" || Docente3 == " " || Docente3 == null)
                {
                    Docente3 = "";
                    idMaestro3 = "";
                }
                else
                {
                    if (Docente3.Substring(0, 1) == "0")
                    {
                        idMaestro3 = "0";
                        Docente3 = Docente3.Substring(2);
                    }
                    else
                    {
                        idMaestro3 = Docente3.Substring(0, 4);
                        Docente3 = Docente3.Substring(5);
                    }
                }
                if (Docente4 == "" || Docente4 == " " || Docente4 == null)
                {
                    Docente4 = "";
                    idMaestro4 = "";
                }
                else
                {
                    if (Docente4.Substring(0, 1) == "0")
                    {
                        idMaestro4 = "0";
                        Docente4 = Docente4.Substring(2);
                    }
                    else
                    {
                        idMaestro4 = Docente4.Substring(0, 4);
                        Docente4 = Docente4.Substring(5);
                    }

                }

                string[] Materias = { Materia1, Materia2, Materia3, Materia4 };
                string[] idMaterias = { idMateria1, idMateria2, idMateria3, idMateria4 };
                string[] idSemestres = { idSemestre1, idSemestre2, idSemestre3, idSemestre4 };
                string[] idDocente = { Docente1, Docente2, Docente3, Docente4 };
                string[] idMaestro = { idMaestro1, idMaestro2, idMaestro3, idMaestro4 };
                string[] idGrupo = { Grupo1, Grupo2, Grupo3, Grupo4 };
                string[] DatExams = { DatExam1, DatExam2, DatExam3, DatExam4 };

                //Referencia = Matricula_tr + digitoVerificador00(Matricula_tr);

                using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    try
                    {
                        int row = 0;
                        foreach (string elote in Materias)
                        {
                            if (elote != "")
                            {
                                CostoMulti = CostoMulti + 1;
                                using (SqlCommand cmd = new SqlCommand("EXT_P_Insert_PapeletasAdeudo", conexion))
                                {
                                    try
                                    {
                                        if (cmd.Connection.State == ConnectionState.Closed)
                                        {
                                            cmd.Connection.Open();
                                        }

                                        cmd.CommandType = CommandType.StoredProcedure;
                                        cmd.Parameters.AddWithValue("@Materia", elote);
                                        cmd.Parameters.AddWithValue("@idPlantel", idPlantel_tr);
                                        cmd.Parameters.AddWithValue("@Alumno", Alumno_tr);
                                        cmd.Parameters.AddWithValue("@Matricula", Matricula_tr);
                                        cmd.Parameters.AddWithValue("@CostoMateria", CostoMateria_tr);
                                        cmd.Parameters.AddWithValue("@CIE", CIE_tr);
                                        cmd.Parameters.AddWithValue("@Referencia", Referencia);
                                        cmd.Parameters.AddWithValue("@FechaGenerada", FechaGenerada);
                                        cmd.Parameters.AddWithValue("@Docente", idDocente[row]);
                                        cmd.Parameters.AddWithValue("@Grupo", idGrupo[row]);
                                        cmd.Parameters.AddWithValue("@FechaAlta", FechaGenerada);
                                        cmd.Parameters.AddWithValue("@UsuarioAlta", Matricula_tr);
                                        cmd.Parameters.AddWithValue("@idSemestre", idSemestres[row]);
                                        cmd.Parameters.AddWithValue("@idMateria", idMaterias[row]);
                                        cmd.Parameters.AddWithValue("@idAlumno", idAlumno_tr);
                                        cmd.Parameters.AddWithValue("@FechaExamen", DatExams[row]);
                                        cmd.Parameters.AddWithValue("@idMaestro", idMaestro[row]);
                                        //cmd.Parameters.AddWithValue("@FechaModificacion", FechaModificacion);
                                        //cmd.Parameters.AddWithValue("@UsuarioModificacion", UsuarioModificacion);

                                        SqlDataReader reader3 = cmd.ExecuteReader();
                                        while (reader3.Read())
                                        {
                                            //Luego guarda ese Id y le suma 1 para el nuevo insert
                                            Console.WriteLine("Good");
                                        }

                                        reader3.Close();
                                        conexion.Close();
                                    }
                                    catch (SqlException ex)
                                    {
                                        for (int i = 0; i < ex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + ex.Errors[i].Message + "\n" +
                                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                "Source: " + ex.Errors[i].Source + "\n" +
                                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine(errorMessages.ToString());
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("Error Conexion:" + ex.Message);
                                    }
                                }
                                row++;
                            }
                        }
                        total = costoMateria * CostoMulti;
                        totals = "$ " + total.ToString() + ".00";
                    }
                    catch (SqlException ex)
                    {
                        for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append("Index #" + i + "\n" +
                                "Message: " + ex.Errors[i].Message + "\n" +
                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                "Source: " + ex.Errors[i].Source + "\n" +
                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                        }
                        Console.WriteLine("Error SQL:" + errorMessages.ToString());

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Conexion:" + ex.Message);
                    }
                }
                return RedirectToAction("DisplayPapeleta", new { Matricula = Matricula_tr, idPlantel = idPlantel_tr, Alumno = Alumno_tr, CostoMateria = totals, CIE = CIE_tr, Referencia = Referencia, FechaGenerada = FechaGenerada });
            }
            else
            {
                return RedirectToAction("LoginAdmin", "Account");
            }
        }

        public ActionResult DisplayPapeleta2(string Matricula)
        {
            string costoUni = "";
            int CostoMulti = 0;
            double costoUnii = 0.00;
            ViewBag.Title = "Generar Papeleta de Pago";
            DataTable dt2 = new DataTable();
            StringBuilder errorMessages = new StringBuilder();
            List<PapeletasAdeudo> lst_PapeletasAdeudo = new List<PapeletasAdeudo>();
            using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                try
                {
                    using (SqlCommand cmd2 = new SqlCommand("EXT_P_CallPapeletaAdeudo", conexion))
                    {
                        if (cmd2.Connection.State == ConnectionState.Closed)
                        {
                            cmd2.Connection.Open();
                        }

                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@Matricula", Matricula);

                        using (SqlDataAdapter da2 = new SqlDataAdapter(cmd2))
                        {
                            da2.SelectCommand = cmd2;
                            da2.Fill(dt2);
                            try
                            {
                                if (dt2.Rows.Count > 0)
                                {
                                    foreach (DataRow dbRow in dt2.Rows)
                                    {
                                        CostoMulti = CostoMulti + 1;

                                        lst_PapeletasAdeudo.Add(new PapeletasAdeudo
                                        {
                                            Materia = dbRow["Materia"].ToString() + " - " + dbRow["FechaExamen"].ToString().Substring(0, 10) + " - " + dbRow["Docente"].ToString()

                                        });
                                        ViewBag.Materias = lst_PapeletasAdeudo;
                                        ViewData["idPlantel"] = dbRow["idPlantel"].ToString();
                                        ViewData["Alumno"] = dbRow["Alumno"].ToString();
                                        ViewData["Matricula"] = Matricula;
                                        ViewData["Correo"] = Matricula + "@cobachih.edu.mx";
                                        ViewData["Referencia"] = dbRow["Referencia"].ToString();
                                        ViewData["CIE"] = dbRow["CIE"].ToString();
                                        costoUni = dbRow["CostoMateria"].ToString().Substring(1);
                                        ViewData["FechaGenerada"] = dbRow["FechaGenerada"].ToString();
                                    }
                                    costoUnii = Convert.ToDouble(costoUni);
                                    ViewData["CostoMateria"] = "$" + Convert.ToString(costoUnii * CostoMulti) + ".00";
                                }
                                else
                                {
                                    ViewData["PapelError"] = "No existe papeleta";
                                    return RedirectToAction("PanelPlantel", "Home");
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }

                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    Console.WriteLine("Error SQL:" + errorMessages.ToString());

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Conexion:" + ex.Message);
                }
            }
            return View("DisplayPapeleta");
        }

        public ActionResult DisplayPapeleta(string Matricula, string idPlantel, string Alumno, string CostoMateria, string CIE, string Referencia, string FechaGenerada)
        {
            ViewBag.Title = "Generar Papeleta de Pago";
            DataTable dt2 = new DataTable();
            StringBuilder errorMessages = new StringBuilder();
            List<PapeletasAdeudo> lst_PapeletasAdeudo = new List<PapeletasAdeudo>();
            using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
            {
                try
                {
                    using (SqlCommand cmd2 = new SqlCommand("EXT_P_CallPapeletaAdeudo", conexion))
                    {
                        if (cmd2.Connection.State == ConnectionState.Closed)
                        {
                            cmd2.Connection.Open();
                        }

                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.AddWithValue("@Matricula", Matricula);

                        using (SqlDataAdapter da2 = new SqlDataAdapter(cmd2))
                        {
                            da2.SelectCommand = cmd2;
                            da2.Fill(dt2);
                            try
                            {
                                if (dt2.Rows.Count > 0)
                                {
                                    foreach (DataRow dbRow in dt2.Rows)
                                    {
                                        lst_PapeletasAdeudo.Add(new PapeletasAdeudo
                                        {
                                            Materia = dbRow["Materia"].ToString() + " - " + dbRow["FechaExamen"].ToString().Substring(0, 10) + " - " + dbRow["Docente"].ToString()
                                        });
                                    }
                                    ViewBag.Materias = lst_PapeletasAdeudo;
                                    ViewData["idPlantel"] = idPlantel;
                                    ViewData["Alumno"] = Alumno;
                                    ViewData["Correo"] = Matricula + "@cobachih.edu.mx";
                                    ViewData["Matricula"] = Matricula;
                                    ViewData["CostoMateria"] = CostoMateria;
                                    ViewData["Referencia"] = Referencia;
                                    ViewData["CIE"] = CIE;
                                    ViewData["FechaGenerada"] = FechaGenerada;
                                    //  ViewData["Docente"] = Docente;
                                    //  ViewData["Grupo"] = Grupo;
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }

                }
                catch (SqlException ex)
                {
                    for (int i = 0; i < ex.Errors.Count; i++)
                    {
                        errorMessages.Append("Index #" + i + "\n" +
                            "Message: " + ex.Errors[i].Message + "\n" +
                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                            "Source: " + ex.Errors[i].Source + "\n" +
                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                    }
                    Console.WriteLine("Error SQL:" + errorMessages.ToString());

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error Conexion:" + ex.Message);
                }
            }
            return View("DisplayPapeleta");
        }
        
        public ActionResult PanelPlantel()
        {
            string usu = (string)Session["usu"];
            string pas = (string)Session["pas"];
            string plantel = (string)Session["idplantel"];

            if (usu != null && pas != null && plantel != null)
            {
                ViewBag.Title = "Papeletas Extraordinarias Plantel " + plantel;
                @ViewData["Plantel"] = plantel;
                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                StringBuilder errorMessages = new StringBuilder();
                List<PapeletasAdeudo> lst_PapeletasAdeudo = new List<PapeletasAdeudo>();
                List<DocentesModel> lst_Docentes = new List<DocentesModel>();
                List<GruposModel> lst_Grupos = new List<GruposModel>();
                using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("EXT_P_CallPapeletaAdeudoByPlantel", conexion))
                        {
                            if (cmd.Connection.State == ConnectionState.Closed)
                            {
                                cmd.Connection.Open();
                            }

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@plantel", plantel);

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.SelectCommand = cmd;
                                da.Fill(dt);
                                try
                                {
                                    if (dt.Rows.Count > 0)
                                    {
                                        foreach (DataRow dbRow in dt.Rows)
                                        {
                                            lst_PapeletasAdeudo.Add(new PapeletasAdeudo
                                            {
                                                Folio = dbRow["idPapeletaAd"].ToString(),
                                                Matricula = dbRow["Matricula"].ToString(),
                                                Materia = dbRow["Materia"].ToString(),
                                                idPlantel = dbRow["idPlantel"].ToString(),
                                                Alumno = dbRow["Alumno"].ToString(),
                                                CostoMateria = dbRow["CostoMateria"].ToString(),
                                                CIE = dbRow["CIE"].ToString(),
                                                Referencia = dbRow["Referencia"].ToString(),
                                                FechaGenerada = dbRow["FechaGenerada"].ToString(),
                                                Docente = dbRow["Docente"].ToString(),
                                                Grupo = dbRow["Grupo"].ToString(),
                                                Comprobante = dbRow["Comprobante"].ToString(),
                                                Revisado = dbRow["Revisado"].ToString(),
                                                idSemestre = dbRow["idSemestre"].ToString()
                                            });
                                        }
                                    }
                                    else
                                    {
                                        TempData["empty"] = "EMPTY";
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    for (int i = 0; i < ex.Errors.Count; i++)
                                    {
                                        errorMessages.Append("Index #" + i + "\n" +
                                            "Message: " + ex.Errors[i].Message + "\n" +
                                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                            "Source: " + ex.Errors[i].Source + "\n" +
                                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                                    }
                                    Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                }
                                catch (Exception ex)
                                {Console.WriteLine(ex.Message);}
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand("EXT_P_CallDocentes", conexion))
                        {
                            if (cmd.Connection.State == ConnectionState.Closed)
                            {
                                cmd.Connection.Open();
                            }

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@plantel", plantel);

                            using (SqlDataAdapter da3 = new SqlDataAdapter(cmd))
                            {
                                da3.SelectCommand = cmd;
                                da3.Fill(dt3);
                                try
                                {
                                    if (dt3.Rows.Count > 0)
                                    {
                                        foreach (DataRow dbRow in dt3.Rows)
                                        {
                                            lst_Docentes.Add(new DocentesModel
                                            {
                                                id = dbRow["idMaestro"].ToString(),
                                                Nombre = dbRow["Nombre"].ToString(),
                                                ApellidoPaterno = dbRow["ApellidoPaterno"].ToString(),
                                                ApellidoMaterno = dbRow["ApellidoMaterno"].ToString()
                                            });
                                        }
                                    }
                                    ViewBag.Docentes = lst_Docentes;
                                }
                                catch (SqlException ex)
                                {
                                    for (int i = 0; i < ex.Errors.Count; i++)
                                    {
                                        errorMessages.Append("Index #" + i + "\n" +
                                            "Message: " + ex.Errors[i].Message + "\n" +
                                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                            "Source: " + ex.Errors[i].Source + "\n" +
                                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                                    }
                                    Console.WriteLine("Error SQL:" + errorMessages.ToString());

                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }

                        foreach (var semestre in lst_PapeletasAdeudo)
                        {
                            using (SqlCommand cmd = new SqlCommand("P_CallGrupos", conexion))
                            {
                                if (cmd.Connection.State == ConnectionState.Closed)
                                {
                                    cmd.Connection.Open();
                                }

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@idSemestre", Convert.ToString(semestre.idSemestre));

                                using (SqlDataAdapter da2 = new SqlDataAdapter(cmd))
                                {
                                    da2.SelectCommand = cmd;
                                    da2.Fill(dt2);
                                    try
                                    {
                                        if (dt2.Rows.Count > 0)
                                        {
                                            foreach (DataRow dbRow in dt2.Rows)
                                            {
                                                lst_Grupos.Add(new GruposModel
                                                {
                                                    idGrupo = dbRow["idGrupo"].ToString(),
                                                    CveGrupo = dbRow["CveGrupo"].ToString()
                                                });
                                            }
                                        }
                                        ViewBag.Grupos = lst_Grupos;
                                    }
                                    catch (SqlException ex)
                                    {
                                        for (int i = 0; i < ex.Errors.Count; i++)
                                        {
                                            errorMessages.Append("Index #" + i + "\n" +
                                                "Message: " + ex.Errors[i].Message + "\n" +
                                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                                "Source: " + ex.Errors[i].Source + "\n" +
                                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                                        }
                                        Console.WriteLine("Error SQL:" + errorMessages.ToString());

                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex.Message);
                                    }
                                }
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append("Index #" + i + "\n" +
                                "Message: " + ex.Errors[i].Message + "\n" +
                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                "Source: " + ex.Errors[i].Source + "\n" +
                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                        }
                        Console.WriteLine("Error SQL:" + errorMessages.ToString());
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Conexion:" + ex.Message);
                    }
                }
                return View(lst_PapeletasAdeudo);
            }
            else if (User.Identity.IsAuthenticated)
            { //inicio de sesion
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("LoginAdmin", "Account");
            }
        }

        [HttpPost]
        public ActionResult UpdPapeleta(string Matricula, string Materia, string Docente, string Grupo, string Revisado)
        {
            ViewBag.Title = "Confirmación de pago";

            string FechaEdicion = DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");

            if (Docente != null && Grupo != null)
            {
                StringBuilder errorMessages = new StringBuilder();
                using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand("EXT_P_Update_PapeletasAdeudo2", conexion))
                        {
                            try
                            {
                                if (cmd.Connection.State == ConnectionState.Closed)
                                {
                                    cmd.Connection.Open();
                                }

                                if (Revisado == null )
                                {
                                    Revisado = "off";
                                }

                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.AddWithValue("@Matricula", Matricula);
                                cmd.Parameters.AddWithValue("@Materia", Materia);
                                cmd.Parameters.AddWithValue("@Docente", Docente);
                                cmd.Parameters.AddWithValue("@idGrupo", Grupo);
                                cmd.Parameters.AddWithValue("@Revisado", Revisado);
                                cmd.Parameters.AddWithValue("@FechaEdicion", FechaEdicion);
                                cmd.Parameters.AddWithValue("@UsuarioEdicion", (string)Session["usu"]);


                                SqlDataReader reader3 = cmd.ExecuteReader();
                                while (reader3.Read())
                                {
                                    Console.WriteLine("Good");
                                }
                                ViewBag.Mensaje = "¡Actualizado!";

                                reader3.Close();
                                conexion.Close();
                            }
                            catch (SqlException ex)
                            {
                                for (int i = 0; i < ex.Errors.Count; i++)
                                {
                                    errorMessages.Append("Index #" + i + "\n" +
                                        "Message: " + ex.Errors[i].Message + "\n" +
                                        "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                        "Source: " + ex.Errors[i].Source + "\n" +
                                        "Procedure: " + ex.Errors[i].Procedure + "\n");
                                }
                                Console.WriteLine(errorMessages.ToString());
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Error Conexion:" + ex.Message);
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append("Index #" + i + "\n" +
                                "Message: " + ex.Errors[i].Message + "\n" +
                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                "Source: " + ex.Errors[i].Source + "\n" +
                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                        }
                        Console.WriteLine("Error SQL:" + errorMessages.ToString());

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error Conexion:" + ex.Message);
                    }

                }
            }
            else
            {
                ViewBag.Mensaje = "¡ERROR!";
            }
            return RedirectToAction("PanelPlantel","Home");
        }
            
        public ActionResult Reportes()
        {
            string usu = (string)Session["usu"];
            string pas = (string)Session["pas"];
            string plantel = (string)Session["idplantel"];

            if (usu != null && pas != null && plantel != null)
            {
                ViewBag.Title = "Reportes Plantel " + plantel;
                ViewData["Plantel"] = plantel;

                DataTable dt = new DataTable();
                DataTable dt2 = new DataTable();
                DataTable dt3 = new DataTable();
                DataTable dt4 = new DataTable();
                DataTable dt5 = new DataTable();
                StringBuilder errorMessages = new StringBuilder();
                List<PapeletasAdeudo> lst_PapeletasAdeudo = new List<PapeletasAdeudo>();
                List<IndicadoresModel> lst_Indicadores1 = new List<IndicadoresModel>();
                List<IndicadoresModel> lst_Indicadores2 = new List<IndicadoresModel>();
                List<IndicadoresModel> lst_Indicadores3 = new List<IndicadoresModel>();
                List<IndicadoresModel> lst_Indicadores5 = new List<IndicadoresModel>();
                using (SqlConnection conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["conn"].ConnectionString)){
                    try{
                        using (SqlCommand cmd = new SqlCommand("EXT_P_CallPapeletaAdeudoByPlantel", conexion)){
                            if (cmd.Connection.State == ConnectionState.Closed)
                            {   cmd.Connection.Open();}

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@plantel", plantel);

                            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            {
                                da.SelectCommand = cmd;
                                da.Fill(dt);
                                try{
                                    if (dt.Rows.Count > 0){
                                        foreach (DataRow dbRow in dt.Rows){
                                            lst_PapeletasAdeudo.Add(new PapeletasAdeudo{
                                                Folio = dbRow["idPapeletaAd"].ToString(),
                                                Matricula = dbRow["Matricula"].ToString(),
                                                Materia = dbRow["Materia"].ToString(),
                                                idPlantel = dbRow["idPlantel"].ToString(),
                                                Alumno = dbRow["Alumno"].ToString(),
                                                CostoMateria = dbRow["CostoMateria"].ToString(),
                                                CIE = dbRow["CIE"].ToString(),
                                                Referencia = dbRow["Referencia"].ToString(),
                                                FechaGenerada = dbRow["FechaGenerada"].ToString(),
                                                Docente = dbRow["Docente"].ToString(),
                                                Grupo = dbRow["Grupo"].ToString(),
                                                Comprobante = dbRow["Comprobante"].ToString(),
                                                Revisado = dbRow["Revisado"].ToString(),
                                                idSemestre = dbRow["idSemestre"].ToString()
                                            });
                                        }
                                    }else{   TempData["empty"] = "EMPTY";}
                                }
                                catch (SqlException ex){   for (int i = 0; i < ex.Errors.Count; i++)
                                    {
                                        errorMessages.Append("Index #" + i + "\n" +
                                            "Message: " + ex.Errors[i].Message + "\n" +
                                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                            "Source: " + ex.Errors[i].Source + "\n" +
                                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                                    }
                                    Console.WriteLine("Error SQL:" + errorMessages.ToString());}
                                catch (Exception ex)
                                {   Console.WriteLine(ex.Message); }
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand("EXT_P_CallExtrasxMaestro", conexion)){
                            if (cmd.Connection.State == ConnectionState.Closed)
                            { cmd.Connection.Open(); }

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@plantel", plantel);

                            using (SqlDataAdapter da2 = new SqlDataAdapter(cmd)){
                                da2.SelectCommand = cmd;
                                da2.Fill(dt2);
                                try{
                                    if (dt2.Rows.Count > 0){
                                        foreach (DataRow dbRow in dt2.Rows){
                                            lst_Indicadores1.Add(new IndicadoresModel
                                            {
                                                Docente = dbRow["Docente"].ToString(),
                                                Qty_Alumnos = dbRow["Qty_Alumnos"].ToString()
                                            });
                                        }
                                        ViewBag.ExtrasxMaestro = lst_Indicadores1;
                                    }
                                }
                                catch (SqlException ex){
                                    for (int i = 0; i < ex.Errors.Count; i++){
                                        errorMessages.Append("Index #" + i + "\n" +
                                            "Message: " + ex.Errors[i].Message + "\n" +
                                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                            "Source: " + ex.Errors[i].Source + "\n" +
                                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                                    }
                                    Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                }catch (Exception ex)
                                { Console.WriteLine(ex.Message); }
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand("EXT_P_CallAlumnosxMateria", conexion)){
                            if (cmd.Connection.State == ConnectionState.Closed)
                            { cmd.Connection.Open(); }

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@plantel", plantel);

                            using (SqlDataAdapter da3 = new SqlDataAdapter(cmd)){
                                da3.SelectCommand = cmd;
                                da3.Fill(dt3);
                                try{
                                    if (dt3.Rows.Count > 0){
                                        foreach (DataRow dbRow in dt3.Rows){
                                            lst_Indicadores2.Add(new IndicadoresModel
                                            {
                                                Materia = dbRow["Materia"].ToString(),
                                                Qty_Alumnos = dbRow["Qty_Alumnos"].ToString()
                                            });
                                        }
                                        ViewBag.AlumnosxMateria = lst_Indicadores2;
                                    }
                                }
                                catch (SqlException ex)
                                {   for (int i = 0; i < ex.Errors.Count; i++)
                                    {
                                        errorMessages.Append("Index #" + i + "\n" +
                                            "Message: " + ex.Errors[i].Message + "\n" +
                                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                            "Source: " + ex.Errors[i].Source + "\n" +
                                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                                    }
                                    Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                }
                                catch (Exception ex)
                                { Console.WriteLine(ex.Message); }
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand("EXT_P_CallTotalesPapeleta", conexion))
                        {
                            if (cmd.Connection.State == ConnectionState.Closed)
                            { cmd.Connection.Open(); }

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@plantel", plantel);

                            using (SqlDataAdapter da4 = new SqlDataAdapter(cmd))
                            {
                                da4.SelectCommand = cmd;
                                da4.Fill(dt4);
                                try{
                                    if (dt4.Rows.Count > 0){
                                        foreach (DataRow dbRow in dt4.Rows){
                                            lst_Indicadores3.Add(new IndicadoresModel
                                            {
                                                TotalPapeletas = dbRow["TotalPapeletas"].ToString(),
                                                TotalGrupos = dbRow["TotalGrupos"].ToString(),
                                                TotalIngreso = dbRow["TotalIngreso"].ToString(),
                                                TotalComprobantes = dbRow["TotalComprobantes"].ToString()
                                            });
                                        }
                                        ViewBag.TotalesPapeleta = lst_Indicadores3;
                                    }
                                }
                                catch (SqlException ex){
                                    for (int i = 0; i < ex.Errors.Count; i++){
                                        errorMessages.Append("Index #" + i + "\n" +
                                            "Message: " + ex.Errors[i].Message + "\n" +
                                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                            "Source: " + ex.Errors[i].Source + "\n" +
                                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                                    }
                                    Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                }catch (Exception ex)
                                { Console.WriteLine(ex.Message); }
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand("EXT_P_CallAlumnosxGrupo", conexion)){
                            if (cmd.Connection.State == ConnectionState.Closed)
                            { cmd.Connection.Open(); }

                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@plantel", plantel);

                            using (SqlDataAdapter da5 = new SqlDataAdapter(cmd)){
                                da5.SelectCommand = cmd;
                                da5.Fill(dt5);
                                try{
                                    if (dt5.Rows.Count > 0){
                                        foreach (DataRow dbRow in dt5.Rows){
                                            lst_Indicadores5.Add(new IndicadoresModel{
                                                Grupo = dbRow["Grupo"].ToString(),
                                                Qty_Alumnos = dbRow["Qty_Alumnos"].ToString()
                                            });
                                        }
                                        ViewBag.AlumnosxGrupo = lst_Indicadores5;
                                    }
                                }
                                catch (SqlException ex){   
                                    for (int i = 0; i < ex.Errors.Count; i++)
                                    {
                                        errorMessages.Append("Index #" + i + "\n" +
                                            "Message: " + ex.Errors[i].Message + "\n" +
                                            "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                            "Source: " + ex.Errors[i].Source + "\n" +
                                            "Procedure: " + ex.Errors[i].Procedure + "\n");
                                    }
                                    Console.WriteLine("Error SQL:" + errorMessages.ToString());
                                }catch (Exception ex)
                                { Console.WriteLine(ex.Message); }
                            }
                        }
                    }
                    catch (SqlException ex){   for (int i = 0; i < ex.Errors.Count; i++)
                        {
                            errorMessages.Append("Index #" + i + "\n" +
                                "Message: " + ex.Errors[i].Message + "\n" +
                                "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                "Source: " + ex.Errors[i].Source + "\n" +
                                "Procedure: " + ex.Errors[i].Procedure + "\n");
                        }
                        Console.WriteLine("Error SQL:" + errorMessages.ToString());}
                    catch (Exception ex)
                    {   Console.WriteLine("Error Conexion:" + ex.Message);}
                }

                return View(lst_PapeletasAdeudo);
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public int digitoVerificador00(string Matricula)
        {
            //Definicion var
            int cantidad;
            int mult;
            int resul1;
            int resultadofinal;
            int Resultado;
            int digit_matric;
            int decena_proxima;
            int ii;
            double opt1;
            double opt2;

            //Iniciarlizar var
            cantidad = Matricula.Length;
            mult = 0;
            resultadofinal = 0;
            ii = 1;
            opt1 = 0.00;
            opt2 = 0.00;

            for (int i = 0; i < cantidad; i++)
            {
                if ((ii % 2) != 0)
                { mult = 2; }
                else
                { mult = 1; }

                digit_matric = Convert.ToInt32(Matricula.Substring(i, 1));
                resul1 = digit_matric * mult;

                if (resul1 > 9)
                {
                    string separar_resul1 = resul1.ToString();
                    Resultado = Convert.ToInt32(separar_resul1.Substring(0, 1)) + Convert.ToInt32(separar_resul1.Substring(1, 1));
                }
                else
                {
                    Resultado = resul1;
                }

                resultadofinal = resultadofinal + Resultado;
                ii++;
            }

            //Redondear decenas
            opt1 = (double)resultadofinal / 10.00;
            opt2 = Math.Ceiling(opt1);
            decena_proxima = Convert.ToInt32(opt2 * 10);
            resultadofinal = decena_proxima - resultadofinal;
            return resultadofinal;
        }
    }
}