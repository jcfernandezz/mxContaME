using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CE.Model;
using System.Xml.Linq;

namespace CE.Business
{
    public class ContabilidadElectronicaPresentacion
    {  
        
        public static DataTable ConvierteLinqQueryADataTable(IEnumerable<dynamic> v)
        {
            //We really want to know if there is any data at all
            var firstRecord = v.FirstOrDefault();
            if (firstRecord == null)
                return null;

            //So dear record, what do you have?
            PropertyInfo[] infos = firstRecord.GetType().GetProperties();

            //Our table should have the columns to support the properties
            DataTable table = new DataTable();

            //columns
            foreach (var info in infos)
            {

                Type propType = info.PropertyType;

                if (propType.IsGenericType && propType.GetGenericTypeDefinition() == typeof(Nullable<>)) //Nullable types should be handled too
                {
                    table.Columns.Add(info.Name, Nullable.GetUnderlyingType(propType));
                }
                else
                {
                    table.Columns.Add(info.Name, info.PropertyType);
                }
            }

            DataRow row;
            //rows
            foreach (var record in v)
            {
                row = table.NewRow();
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    row[i] = infos[i].GetValue(record) != null ? infos[i].GetValue(record) : DBNull.Value;
                }

                table.Rows.Add(row);
            }

            //Table is ready to serve.
            table.AcceptChanges();

            return table;
        }

        public static object ObtieneContenidoLinqDeXML(Int16 year1, Int16 periodid, string tipo, string company, List<ParametrosDeArchivo> lParametros)
        {
            LecturaContabilidadFactory oL = new LecturaContabilidadFactory(company);
            oL.LParametros = lParametros;

            XDocument xdoc = XDocument.Parse(oL.GetXML2(year1, periodid, tipo));

            object items = null;

            var nspace = lParametros.Where(x => x.Tipo == tipo);

            XNamespace cfdi = nspace.First().NameSpace;

            switch (tipo)
            {
                case "Catálogo":
                    //cfdi = @"www.sat.gob.mx/esquemas/ContabilidadE/1_1/CatalogoCuentas";
                    items = from l in xdoc.Descendants(cfdi + "Ctas")
                            select new
                            {
                                NumCta = l.Attribute("NumCta").Value,
                                Desc = l.Attribute("Desc").Value,
                                CodAgrup = l.Attribute("CodAgrup").Value
                            };
                    break;
                case "Balanza":
                    //cfdi = @"www.sat.gob.mx/esquemas/ContabilidadE/1_1/BalanzaComprobacion";
                    items = from l in xdoc.Descendants(cfdi + "Ctas")
                            select new
                            {
                                NumCta = l.Attribute("NumCta").Value,
                                SaldoIni = (l.Attribute("SaldoIni").Value),
                                Debe = (l.Attribute("Debe").Value),
                                Haber = (l.Attribute("Haber").Value),
                                SaldoFin = (l.Attribute("SaldoFin").Value)
                            };

                    //var itemsB = from l in xdoc.Descendants(cfdi + "Ctas")
                    //        select new
                    //        {
                    //            NumCta = l.Attribute("NumCta").Value,
                    //            SaldoIni = Convert.ToDecimal(l.Attribute("SaldoIni").Value.Replace(".",".")),
                    //            Debe = Convert.ToDecimal(l.Attribute("Debe").Value.Replace(".", ".")),
                    //            Haber = Convert.ToDecimal(l.Attribute("Haber").Value.Replace(".", ".")),
                    //            SaldoFin = Convert.ToDecimal(l.Attribute("SaldoFin").Value.Replace(".", "."))
                    //        };

                    //var sum =
                    //    from l in xdoc.Descendants(cfdi + "Ctas")
                    //    select new
                    //        {
                    //            NumCta = "Total",
                    //            SaldoIni = itemsB.Sum(x => Convert.ToDecimal(x.SaldoIni.ToString().Replace(".", "."))),
                    //            Debe = itemsB.Sum(x => Convert.ToDecimal(x.Debe.ToString().Replace(".", "."))),
                    //            Haber = itemsB.Sum(x => Convert.ToDecimal(x.Haber.ToString().Replace(".", "."))),
                    //            SaldoFin = itemsB.Sum(x => Convert.ToDecimal(x.SaldoFin.ToString().Replace(".", ".")))
                    //        };

                    //items = itemsB.Union(sum);

                    break;
                case "Pólizas":
                    //cfdi = @"www.sat.gob.mx/esquemas/ContabilidadE/1_1/PolizasPeriodo";
                    items = from l in xdoc.Descendants(cfdi + "Transaccion")
                            select new
                            {
                                NumCta = l.Attribute("NumCta").Value,
                                DesCta = l.Attribute("DesCta").Value,
                                Concepto = l.Attribute("Concepto").Value,
                                Debe = l.Attribute("Debe").Value,
                                Haber = l.Attribute("Haber").Value
                            };
                    break;
                case "Auxiliar Cuentas":
                    //cfdi = @"www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarCtas";
                    items = from l in xdoc.Descendants(cfdi + "DetalleAux")
                            select new
                            {
                                Fecha = l.Attribute("Fecha").Value,
                                NumUnIdenPol = l.Attribute("NumUnIdenPol").Value,
                                Concepto = l.Attribute("Concepto").Value,
                                Debe = l.Attribute("Debe").Value,
                                Haber = l.Attribute("Haber").Value
                            };
                    break;
                case "Auxiliar folios":
                    //cfdi = @"www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarFolios";
                    items = from l in xdoc.Descendants(cfdi + "DetAuxFol")
                            select new
                            {
                                NumUnIdenPol = l.Attribute("NumUnIdenPol").Value,
                                Fecha = l.Attribute("Fecha").Value
                            };
                    break;
            }

            return items;
        }

    }
}
