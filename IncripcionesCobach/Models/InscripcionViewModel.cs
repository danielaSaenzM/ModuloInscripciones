using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IncripcionesCobach.Models
{
    public class InscripcionViewModel
    {
        public string idplantel { get; set; }
        public string Plantel { get; set; }
        public int Matricula { get; set; }
        public string NombreCompleto { get; set; }
        public string CveGrupo { get; set; }
        public string Nodata { get; set; }
        public string CIE { get; set; }
        public int Semestre { get; set; }
        public int? Inscripcion { get; set; }
        public int? sociedadAlumnos { get; set; }
        public int? Referencia { get; set; }
        public DateTime Fecha { get; set; }
        public bool? pagoInscripcion { get; set; }
        public bool? pagoSA { get; set; }
        public int NoAcreditadas { get; set; }
        public string TurnoM { get; set; }
        public string TurnoV { get; set; }



    }
    public class pago
    {
        public string s_transm { get; set; }
        public string importe { get; set; }
        public string tarjetahabiente { get; set; }
        public string mensaje { get; set; }
        public string referencia { get; set; }
        public string autorizacion { get; set; }
        public string codigo { get; set; }
    }
  

}