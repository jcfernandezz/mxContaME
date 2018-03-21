using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace CE.Business
{
    public class UtilitarioArchivos
    {
        String nscfdi;
        TransformerXML plantillaXslt;
        public UtilitarioArchivos(String nameSpace, String rutaXslt)
        {
            nscfdi = nameSpace;
            plantillaXslt = new TransformerXML(rutaXslt);
        }


        public async Task<Cfdi> CargarArchivoAsync(String archivo)
        {
            var cfdi = new Cfdi(nscfdi, plantillaXslt);
            cfdi.ArchivoYCarpeta = archivo;

            if (System.IO.File.Exists(archivo))
            {
                using (var reader = File.OpenText(archivo))
                {
                    cfdi.Sxml = await reader.ReadToEndAsync();
                }
            }
            return cfdi;
        }
    }
}
