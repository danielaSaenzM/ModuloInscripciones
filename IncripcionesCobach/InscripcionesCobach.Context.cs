﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace IncripcionesCobach
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class InscripcionesEntities : DbContext
    {
        public InscripcionesEntities()
            : base("name=InscripcionesEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
    
        public virtual ObjectResult<Nullable<int>> sp_UpdatePago(string matricula, string codigo, string importe)
        {
            var matriculaParameter = matricula != null ?
                new ObjectParameter("Matricula", matricula) :
                new ObjectParameter("Matricula", typeof(string));
    
            var codigoParameter = codigo != null ?
                new ObjectParameter("Codigo", codigo) :
                new ObjectParameter("Codigo", typeof(string));
    
            var importeParameter = importe != null ?
                new ObjectParameter("Importe", importe) :
                new ObjectParameter("Importe", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("sp_UpdatePago", matriculaParameter, codigoParameter, importeParameter);
        }
    
        public virtual ObjectResult<sp_GetCostoPlantel_Result13> sp_GetCostoPlantel(string matricula)
        {
            var matriculaParameter = matricula != null ?
                new ObjectParameter("Matricula", matricula) :
                new ObjectParameter("Matricula", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetCostoPlantel_Result13>("sp_GetCostoPlantel", matriculaParameter);
        }
    
        public virtual int sp_UploadPago(string matricula, string nombre)
        {
            var matriculaParameter = matricula != null ?
                new ObjectParameter("Matricula", matricula) :
                new ObjectParameter("Matricula", typeof(string));
    
            var nombreParameter = nombre != null ?
                new ObjectParameter("Nombre", nombre) :
                new ObjectParameter("Nombre", typeof(string));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("sp_UploadPago", matriculaParameter, nombreParameter);
        }
    }
}
