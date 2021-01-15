using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IncripcionesCobach.Models
{
    public class MateriasAlumno
    {
        public string idPlantel_ID { get; set; }
        public string idSemestre { get; set; }
        public string idMateria { get; set; }
        public string Alumno { get; set; }
        public string Materia { get; set; }
        public string CveMateria { get; set; }
        public string idAlumno { get; set; }
        public string Matricula { get; set; }
        public string CostoMateria { get; set; }
        public string CIE { get; set; }
        public string idPlantel { get; set; }
        public string FechaGenerada { get; set; }
        public string Docente { get; set; }
        public string Grupo { get; set; }
        public string Referencia { get; set; }

        
    }
}