using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CE.Model
{
    public class DcemVwContabilidad
    {
        public Int16 year1 { get; set; }
        public Int16 periodid { get; set; }
        public string catalogo { get; set; }
        public string tipodoc { get; set; }
        public string TipoSolicitud { get; set; }
        public string NumOrden { get; set; }
        public string NumTramite { get; set; }
        public bool existe { get; set; }
    }
}
