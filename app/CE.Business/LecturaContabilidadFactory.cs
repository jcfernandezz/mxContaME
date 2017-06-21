using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using CE.Model;
using Microsoft.Dynamics.GP.eConnect;
//using Microsoft.Dynamics.GP.eConnect.MiscRoutines;
using Microsoft.Dynamics.GP.eConnect.Serialization;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using System.Threading;
using System.Xml.Schema;

namespace CE.Business
{
    public class LecturaContabilidadFactory
    {
        private string connectionString = "";
        private string _pre = "";
        private string ErroresValidarXml = "";
        private List<string> l_ErroresValidarXml = null;

        public LecturaContabilidadFactory(string pre)
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[pre].ToString();
            _pre = pre;
        }

        public string GetCompany()
        {
            string sql = "select CMPNYNAM from dynamics..sy01500 where INTERID = DB_NAME()";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return dt.Rows[0]["CMPNYNAM"].ToString();
                }
            }
        }

        #region ExportarXML
        public string GetRFC()
        {
            string sql = "select replace(cia.TAXREGTN, 'RFC ', '') TAXREGTN FROM DYNAMICS..SY01500 cia where cia.INTERID = DB_NAME()";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return dt.Rows[0]["TAXREGTN"].ToString().Trim();
                }
            }
        }

        public DcemVwContabilidad GetXML(int year, int perdiodo, string tipo)
        {
            string sql = "select * from DcemVwContabilidad where year1 = @year1 and periodid = @periodid and tipodoc = @tipodoc)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@year1", SqlDbType.SmallInt).Value = year;
                cmd.Parameters.Add("@periodid", SqlDbType.SmallInt).Value = perdiodo;
                cmd.Parameters.Add("@tipodoc", SqlDbType.VarChar,8).Value = tipo;
                
                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    List<DcemVwContabilidad> l = dt.DataTableToList<DcemVwContabilidad>();
                    return l[0];
                }
            }
        }

        /// <summary>
        /// Ejecuta un sp que corrige los docs marcados con error
        /// </summary>
        /// <param name="tipo"></param>
        public void corregirDocsConError(string tipo)
        {
            string sprocedure = "";
            switch (tipo)
            {
                case "Pólizas":
                    sprocedure = "dcem.dcemCorrigePoliza";
                    break;
            }

            if (!sprocedure.Equals(""))
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sprocedure;
                    cmd.ExecuteNonQuery();
                }

        }

        /// <summary>
        /// Obtiene el xml del periodo desde la bd. El tipo indica el tipo de xml.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="perdiodo"></param>
        /// <param name="tipo">Tipo de xml</param>
        /// <returns></returns>
        public string GetXML2(int year, int perdiodo, string tipo)
        {
            string tabla = "";

            switch (tipo)
            {
                case "Auxiliar Cuentas":
                    tabla = "DCEMFCNAUXILIARCTAS";
                    break;
                case "Auxiliar folios":
                    tabla = "DCEMFCNAUXILIARFOLIOS";
                    break;
                case "Balanza":
                    tabla = "DCEMFCNBALANCE";
                    break;
                case "Catálogo":
                    tabla = "DCEMFCNCATALOGOXML";
                    break;
                case "Pólizas":
                    tabla = "DCEMFCNPOLIZAS";
                    break;
            }

            string sql = "select dbo."+ tabla +" (@periodid, @year1)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@year1", SqlDbType.SmallInt).Value = year;
                cmd.Parameters.Add("@periodid", SqlDbType.SmallInt).Value = perdiodo;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return dt.Rows[0][0].ToString();
                    
                    //List<DcemVwContabilidad> l = dt.DataTableToList<DcemVwContabilidad>();
                    //return l[0][0];
                }
            }
        }

        public List<DcemVwContabilidad> GetAll(int year, string tipodoc)
        {
            if (tipodoc == null)
                tipodoc = "";

            string sql = "select *, coalesce((select top 1 1 from DcemContabilidadExportados de where de.year1 = dv.year1 and de.periodid = dv.periodid and de.tipodoc = dv.tipodoc),0) existe from DcemVwContabilidad dv where year1 = @year1 and (tipodoc = @tipodoc or @tipodoc = '')";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@year1", SqlDbType.SmallInt).Value = year;
                cmd.Parameters.Add("@tipodoc", SqlDbType.VarChar).Value = tipodoc;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    List<DcemVwContabilidad> l = dt.DataTableToList<DcemVwContabilidad>();
                    return l;
                }
            }
        }

        void validarUnaPolizaPorVez(string docXml, string esquemaXml)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null, esquemaXml);
            XNamespace ns = "www.sat.gob.mx/esquemas/ContabilidadE/1_1/PolizasPeriodo";

            XDocument xDocAValidar = new XDocument();
            xDocAValidar = XDocument.Parse(docXml);
            xDocAValidar.Root.Elements(ns + "Poliza").Remove();

            XDocument xDocContable = XDocument.Parse(docXml);
            l_ErroresValidarXml = new List<string>();

            foreach (XElement ele in xDocContable.Elements(ns + "Polizas").Elements())
            {
                xDocAValidar.Root.Add(new XElement(ele));
                
                xDocAValidar.Validate(schemas, (o, e) =>
                {
                    ErroresValidarXml += "ed: "+ ele.Attribute("NumUnIdenPol").Value.ToString() + " "+ e.Message + " " + o.ToString() + Environment.NewLine;
                    l_ErroresValidarXml.Add(ele.Attribute("NumUnIdenPol").Value.ToString());
                });

                xDocAValidar.Root.Elements(ns + "Poliza").Remove();
            }

        }

        void validarXml(string docXml, string esquemaXml)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null, esquemaXml);

            //Console.WriteLine("Attempting to validate");
            XDocument xDocContable = XDocument.Parse(docXml);
            //bool errors = false;
            xDocContable.Validate(schemas, (o, e) =>
            {
                ErroresValidarXml += e.Message + " " +o.ToString() + Environment.NewLine;
                //Console.WriteLine("msj {0} nodo {1}", e.Message, o.ToString());
                //errors = true;
            });
            //Console.WriteLine();
        }

        /// <summary>
        /// Genera el xml a partir de los parámetros archivox, valida y lo guarda.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="directorio"></param>
        /// <param name="archivo1"></param>
        /// <param name="archivo2"></param>
        /// <param name="archivo3"></param>
        /// <param name="archivo4"></param>
        /// <param name="archivo5"></param>
        /// <param name="directorioXSD"></param>
        /// <returns></returns>
        public List<XmlExportado> SaveFiles(List<DcemVwContabilidad> items, string directorio, string archivo1, string archivo2, string archivo3, string archivo4, string archivo5, string directorioXSD)
        {
            string archivo = "";
            
            List<XmlExportado> xmls = new List<XmlExportado>();

            if (!System.IO.Directory.Exists(directorio))
                System.IO.Directory.CreateDirectory(directorio);

            foreach (var item in items)
            {
                string archivoXSD = directorioXSD;
                ErroresValidarXml = "";

                switch (item.tipodoc)
                {
                    case "Catálogo":
                        archivo = archivo1;
                        archivoXSD += "CatalogoCuentas_1_1.xsd";
                        break;
                    case "Balanza":
                        archivo = archivo2;
                        archivoXSD += "BalanzaComprobacion_1_1.xsd";
                        break;
                    case "Pólizas":
                        archivo = archivo3;
                        archivoXSD += "PolizasPeriodo_1_1.xsd";
                        this.corregirDocsConError(item.tipodoc);
                        break;
                    case "Auxiliar Cuentas":
                        archivo = archivo4;
                        archivoXSD += "AuxiliarCtas_1_1.xsd";
                        break;
                    case "Auxiliar folios":
                        archivo = archivo5;
                        archivoXSD += "AuxiliarFolios_1_2.xsd";
                        break;
                }

                int version = GetVersionXML(item);
                archivo = System.IO.Path.GetFileNameWithoutExtension(archivo) + "_" + version + System.IO.Path.GetExtension(archivo);

                string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                xml += Environment.NewLine;
                
                item.catalogo = this.GetXML2(item.year1, item.periodid, item.tipodoc);

                //xml += item.catalogo;

                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(item.catalogo);
                XmlElement root = xmlDoc.DocumentElement;

                XmlAttribute attr;

                if (item.TipoSolicitud != null)
                {
                    
                    attr = xmlDoc.CreateAttribute(null, "TipoSolicitud", null);
                    attr.Value = item.TipoSolicitud.Substring(0, 2);
                    root.Attributes.Append(attr);
                }

                if (item.NumOrden != null)
                {
                    attr = xmlDoc.CreateAttribute(null, "NumOrden", null);
                    attr.Value = item.NumOrden;
                    root.Attributes.Append(attr);
                }

                if (item.NumTramite != null)
                {
                    attr = xmlDoc.CreateAttribute(null, "NumTramite", null);
                    attr.Value = item.NumTramite;
                    root.Attributes.Append(attr);
                }
                
                item.catalogo = xmlDoc.InnerXml;
                xml += item.catalogo;

                XmlExportado xmle = new XmlExportado();
                xmle.DcemVwContabilidad = item;
                xmle.archivo = archivo;

                // Guarda xml
                System.IO.File.WriteAllText(directorio + "\\" + this.GetRFC() + item.year1.ToString() + item.periodid.ToString().PadLeft(2, '0') + archivo, xml);
                InsertDatosExportados(item, (Int16)version);

                //if(item.tipodoc.Equals("Pólizas"))
                //        validarUnaPolizaPorVez(item.catalogo, archivoXSD);
                //        marcarPolizasConError();
                //else
                validarXml(item.catalogo, archivoXSD);

                // Raise exception, if XML validation fails
                xmle.error = false;
                if (ErroresValidarXml != "")
                    {
                        xmle.error = true;
                        xmle.mensaje = ErroresValidarXml;
                    }

                xmls.Add(xmle);
            }

            return xmls;
        }

        private void vr_ValidationEventHandler(object sender, ValidationEventArgs e)
        {
            
            ErroresValidarXml += e.Message + " "  + Environment.NewLine;

        }
        
        private int GetVersionXML(DcemVwContabilidad item)
        {
            string sql = "select coalesce(max([version]), 0) as v from DcemContabilidadExportados where year1 = @year1 and periodid = @periodid and tipodoc = @tipodoc";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@year1", SqlDbType.SmallInt).Value = item.year1;
                cmd.Parameters.Add("@periodid", SqlDbType.SmallInt).Value = item.periodid;
                cmd.Parameters.Add("@tipodoc", SqlDbType.VarChar, 20).Value = item.tipodoc;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return int.Parse(dt.Rows[0]["v"].ToString()) + 1;
                }
            }
        }

        private void InsertDatosExportados(DcemVwContabilidad item, Int16 version)
        {
            string sql = "insert into DcemContabilidadExportados (fecha, year1, periodid, catalogo, tipodoc, version) values (@fecha, @year1, @periodid, @catalogo, @tipodoc, @version)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@fecha", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@year1", SqlDbType.SmallInt).Value = item.year1;
                cmd.Parameters.Add("@periodid", SqlDbType.SmallInt).Value = item.periodid;
                cmd.Parameters.Add("@tipodoc", SqlDbType.VarChar, 20).Value = item.tipodoc;
                cmd.Parameters.Add("@catalogo", SqlDbType.Xml).Value = item.catalogo;
                cmd.Parameters.Add("@version", SqlDbType.SmallInt).Value = version;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region Importar Facturas Electronicas
        /// <summary>
        /// Importar facturas a GP
        /// </summary>
        /// <param name="archivos">Path de los archivos</param>
        /// <param name="metodo">1: PM - 2: POP</param>
        public void ImportarGPPM(List<string> archivos, int metodo)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            XNamespace cfdi = @"http://www.sat.gob.mx/cfd/3";
            XNamespace tfd = @"http://www.sat.gob.mx/TimbreFiscalDigital";
            XNamespace implocal = @"http://www.sat.gob.mx/implocal";

            string BACHNUMB = DateTime.Now.ToString("yyyyMMddHHmmss");
            string formatoFecha = System.Configuration.ConfigurationManager.AppSettings[_pre + "_FormatoFecha"].ToString();

            foreach (string archivo in archivos)
            {
                if (System.IO.File.Exists(archivo))
                {
                    using (eConnectMethods eConnectMethods = new eConnectMethods())
                    {
                        eConnectMethods.RequireProxyService = true;
                        
                        List<PMTransactionType> masterPMTransactionTypes = new List<PMTransactionType>();
                        List<POPReceivingsType> masterPOPReceivingsTypes = new List<POPReceivingsType>();


                        try
                        {
                            bool error = false;

                            string xml = System.IO.File.ReadAllText(archivo);

                            XDocument xdoc = XDocument.Parse(xml);

                            PMTransactionType PAInvoiceEntry = new PMTransactionType();
                            POPReceivingsType POPInvoiceEntry = new POPReceivingsType();

                            taPMTransactionInsert PMHeader = new taPMTransactionInsert();
                            taPopRcptHdrInsert POPHeader = new taPopRcptHdrInsert();

                            List<taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert> items = new List<taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert>();

                            var comprobantes = (from c in xdoc.Descendants(cfdi + "Comprobante")
                                                select new
                                                {
                                                    folio = c.Attribute("folio") == null ? "" : c.Attribute("folio").Value,
                                                    fecha = c.Attribute("fecha").Value,
                                                    formaDePago = c.Attribute("formaDePago").Value,
                                                    condicionesDePago = c.Attribute("condicionesDePago") == null ? "" : c.Attribute("condicionesDePago").Value,
                                                    subTotal = c.Attribute("subTotal").Value,
                                                    TipoCambio = c.Attribute("TipoCambio") == null ? "" : c.Attribute("TipoCambio").Value,
                                                    Moneda = c.Attribute("Moneda") == null ? "" : c.Attribute("Moneda").Value,
                                                    total = c.Attribute("total").Value,
                                                    tipoDeComprobante = c.Attribute("tipoDeComprobante").Value,
                                                    metodoDePago = c.Attribute("metodoDePago").Value,
                                                    LugarExpedicion = c.Attribute("LugarExpedicion").Value,
                                                    Descuento = c.Attribute("descuento") == null ? "0" : c.Attribute("descuento").Value,
                                                }).ToList();

                            var comprobante = comprobantes[0];

                            var impuestos = (from c in xdoc.Descendants(cfdi + "Impuestos")
                                             select new
                                             {
                                                 totalImpuestosTrasladados = c.Attribute("totalImpuestosTrasladados") == null ? "0" : c.Attribute("totalImpuestosTrasladados").Value,
                                                 totalImpuestosRetenidos = c.Attribute("totalImpuestosRetenidos") == null ? "0" : c.Attribute("totalImpuestosRetenidos").Value
                                             }).ToList();

                            var impuesto = impuestos[0];

                            var impuestosLocales = (from c in xdoc.Descendants(implocal + "ImpuestosLocales")
                                                    select new
                                                    {
                                                        TotaldeTraslados = c.Attribute("TotaldeTraslados") == null ? "0" : c.Attribute("TotaldeTraslados").Value,
                                                        TotaldeRetenciones = c.Attribute("TotaldeRetenciones") == null ? "0" : c.Attribute("TotaldeRetenciones").Value
                                                    }).ToList();

                            var retenciones = (from c in xdoc.Descendants(cfdi + "Retencion")
                                               select new
                                               {
                                                   impuesto = c.Attribute("impuesto").Value,
                                                   importe = c.Attribute("importe").Value
                                               }).ToList();

                            var translados = (from c in xdoc.Descendants(cfdi + "Traslado")
                                              select new
                                              {
                                                  impuesto = c.Attribute("impuesto").Value,
                                                  tasa = c.Attribute("tasa").Value,
                                                  importe = c.Attribute("importe").Value
                                              }).ToList();

                            var implocalTrasladosLocales = (from c in xdoc.Descendants(implocal + "TrasladosLocales")
                                                            select new
                                                            {
                                                                ImpLocTrasladado = c.Attribute("ImpLocTrasladado").Value,
                                                                TasadeTraslado = c.Attribute("TasadeTraslado").Value,
                                                                Importe = c.Attribute("Importe").Value
                                                            }).ToList();

                            var conceptos = (from c in xdoc.Descendants(cfdi + "Concepto")
                                             select new
                                             {
                                                 cantidad = c.Attribute("cantidad").Value,
                                                 unidad = c.Attribute("unidad").Value,
                                                 noIdentificacion = c.Attribute("noIdentificacion") == null ? "" : c.Attribute("noIdentificacion").Value,
                                                 descripcion = c.Attribute("descripcion").Value,
                                                 valorUnitario = c.Attribute("valorUnitario").Value,
                                                 importe = c.Attribute("importe").Value
                                             }).ToList();

                            var emisores = (from c in xdoc.Descendants(cfdi + "Emisor")
                                            select new
                                            {
                                                rfc = c.Attribute("rfc").Value,
                                                nombre = c.Attribute("nombre") == null ? "" : c.Attribute("nombre").Value
                                            }).ToList();

                            var emisor = emisores[0];

                            var timbresDigital = (from c in xdoc.Descendants(tfd + "TimbreFiscalDigital")
                                                  select new
                                                  {
                                                      UUID = c.Attribute("UUID").Value
                                                  }).ToList();

                            var timbreDigital = timbresDigital[0];

                            string vendorid = getVendroID(emisor.rfc);
                            if (vendorid == null)
                            {
                                ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                args.Archivo = archivo;
                                args.Error = "Proveedor " + emisor.rfc + " no encontrado";

                                OnErrorImportarPM(args);

                                error = true;
                            }
                            else
                            {
                                string folio = comprobante.folio;
                                if (folio == "")
                                {
                                    var numeros = timbreDigital.UUID.Substring(timbreDigital.UUID.Length - 6, 6);
                                    folio = numeros;
                                }

                                string VCHRNMBR = "";
                                if (metodo == 1)
                                    VCHRNMBR = this.GetVchrnmbrFacturaExists(vendorid, folio, 1, DateTime.Parse(comprobante.fecha).Date, decimal.Parse(comprobante.total));
                                else
                                    if (metodo == 2)
                                        VCHRNMBR = this.FacturaPOPExists(vendorid, folio, DateTime.Parse(comprobante.fecha).Date);

                                if (VCHRNMBR == null)
                                {
                                    //la factura no existe en GP
                                    //Factura tipo POP
                                    if (metodo == 2)
                                    {
                                        VCHRNMBR = getNum(metodo);

                                        POPHeader.POPRCTNM = VCHRNMBR;
                                        POPHeader.POPTYPE = 1;
                                        POPHeader.VNDDOCNM = folio;
                                        POPHeader.receiptdate = DateTime.Parse(comprobante.fecha).ToString(formatoFecha);
                                        POPHeader.BACHNUMB = BACHNUMB;
                                        POPHeader.VENDORID = vendorid;
                                        POPHeader.REFRENCE = conceptos.First().descripcion.Length > 30 ? conceptos.First().descripcion.Substring(0, 30) : conceptos.First().descripcion;
                                        POPHeader.CURNCYID = "MXN";
                                        POPHeader.DISAVAMT = 0;
                                    }

                                    //Factura tipo PM
                                    if (metodo == 1)
                                    {
                                        PMHeader.BACHNUMB = BACHNUMB;

                                        VCHRNMBR = getNum(metodo);
                                        PMHeader.VCHNUMWK = VCHRNMBR;

                                        PMHeader.VENDORID = vendorid;
                                        PMHeader.DOCNUMBR = folio;
                                        PMHeader.DOCTYPE = 1;
                                        PMHeader.DOCAMNT = Decimal.Round( decimal.Parse(comprobante.total), 2);
                                        PMHeader.CHRGAMNT = PMHeader.DOCAMNT;
                                        PMHeader.DOCDATE = DateTime.Parse(comprobante.fecha).ToString(formatoFecha);
                                        PMHeader.TAXSCHID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_TAXSCHID"].ToString();
                                        PMHeader.PRCHAMNT = Decimal.Round( decimal.Parse(comprobante.subTotal), 2);
                                        PMHeader.TRDISAMT = Decimal.Round( decimal.Parse(comprobante.Descuento), 2);

                                        decimal totalImpuestosTrasladados = 0;
                                        if (translados.Count > 0)
                                            totalImpuestosTrasladados = Decimal.Round( translados.Sum(x => decimal.Parse(x.importe)), 2);
                                                            
                                        //decimal totalImpuestosTrasladados = decimal.Parse(impuesto.totalImpuestosTrasladados);
                                        if (impuestosLocales.Count > 0)
                                        {
                                            var impuestoLocal = impuestosLocales[0];
                                            totalImpuestosTrasladados += Decimal.Round( decimal.Parse(impuestoLocal.TotaldeTraslados), 2);
                                        }

                                        PMHeader.TAXAMNT = totalImpuestosTrasladados - Decimal.Round( decimal.Parse(impuesto.totalImpuestosRetenidos), 2);
                                        PMHeader.TRXDSCRN = conceptos[0].descripcion.Length > 30 ? conceptos[0].descripcion.Substring(0, 30) : conceptos[0].descripcion;
                                        PMHeader.SHIPMTHD = System.Configuration.ConfigurationManager.AppSettings[_pre + "_SHIPMTHD"].ToString(); ;
                                        PMHeader.CURNCYID = "MXN";
                                        PMHeader.CREATEDIST = 1;

                                        //DOCAMNT = MSCCHAMT + PRCHAMNT + TAXAMNT + FRTAMNT - TRDISAMT

                                        decimal totalCalculo = PMHeader.PRCHAMNT + PMHeader.TAXAMNT - PMHeader.TRDISAMT;
                                        PMHeader.MSCCHAMT = PMHeader.DOCAMNT - totalCalculo;
                                        
                                        #region Retenciones
                                        var retencionGroup = retenciones
                                                            .GroupBy(x => new { x.importe, x.impuesto })
                                                            .Select(g => new
                                                            {
                                                                g.Key.impuesto,
                                                                importe = g.Sum(y => decimal.Parse(y.importe))
                                                            });

                                        foreach (var retencion in retencionGroup)
                                        {
                                            taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert item = new taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert();

                                            item.VENDORID = PMHeader.VENDORID;
                                            item.VCHRNMBR = PMHeader.VCHNUMWK;
                                            item.DOCTYPE = PMHeader.DOCTYPE;
                                            item.BACHNUMB = PMHeader.BACHNUMB;

                                            if (retencion.impuesto.Trim() == "ISR")
                                                item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_ret_ISR"].ToString();
                                            else
                                                if (retencion.impuesto.Trim() == "IVA")
                                                {
                                                    item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_ret_IVA"].ToString();
                                                }
                                                else
                                                {
                                                    error = true;

                                                    ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                                    args.Archivo = archivo;
                                                    args.Error = "Error en retención";

                                                    OnErrorImportarPM(args);
                                                }

                                            item.TAXAMNT = -1 * Decimal.Round( retencion.importe, 2);
                                            item.TDTTXPUR = PMHeader.PRCHAMNT;
                                            item.TXDTTPUR = PMHeader.PRCHAMNT;

                                            items.Add(item);
                                        }
                                        #endregion

                                        #region Traslados
                                        var trasladoGroup = translados
                                                            .GroupBy(x => new { x.tasa, x.impuesto })
                                                            .Select(g => new
                                                            {
                                                                g.Key.impuesto,
                                                                g.Key.tasa,
                                                                importe = g.Sum(y => decimal.Parse(y.importe))
                                                            });

                                        foreach (var retencion in trasladoGroup)
                                        {
                                            taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert item = new taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert();

                                            item.VENDORID = PMHeader.VENDORID;
                                            item.VCHRNMBR = PMHeader.VCHNUMWK;
                                            item.DOCTYPE = PMHeader.DOCTYPE;
                                            item.BACHNUMB = PMHeader.BACHNUMB;

                                            if (retencion.impuesto.Trim() == "IVA")
                                            {
                                                if (decimal.Parse(retencion.tasa) == 0)
                                                    item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_tra_IVA0"].ToString();
                                                else
                                                    if (decimal.Parse(retencion.tasa) == 11)
                                                        item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_tra_IVA11"].ToString();
                                                    else
                                                        if (decimal.Parse(retencion.tasa) == 16)
                                                            item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_tra_IVA16"].ToString();
                                                        else
                                                        {
                                                            error = true;

                                                            ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                                            args.Archivo = archivo;
                                                            args.Error = "Error en traslados";

                                                            OnErrorImportarPM(args);
                                                        }

                                            }
                                            else
                                            {
                                                if (retencion.impuesto.Trim() == "IEPS")
                                                    item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_tra_IEPS"].ToString();
                                                else
                                                {
                                                    error = true;

                                                    ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                                    args.Archivo = archivo;
                                                    args.Error = "Error en traslados";

                                                    OnErrorImportarPM(args);
                                                }
                                            }

                                            item.TAXAMNT = Decimal.Round( retencion.importe, 2);
                                            item.TDTTXPUR = PMHeader.PRCHAMNT;
                                            item.TXDTTPUR = PMHeader.PRCHAMNT;

                                            items.Add(item);
                                        }
                                        
                                        foreach (var implocalTrasladosLocal in implocalTrasladosLocales)
                                        {
                                            taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert item = new taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert();

                                            item.VENDORID = PMHeader.VENDORID;
                                            item.VCHRNMBR = PMHeader.VCHNUMWK;
                                            item.DOCTYPE = PMHeader.DOCTYPE;
                                            item.BACHNUMB = PMHeader.BACHNUMB;

                                            if (implocalTrasladosLocal.ImpLocTrasladado == "ISH" || implocalTrasladosLocal.ImpLocTrasladado == "IMPUESTO SOBRE HOSPEDAJE")
                                            {
                                                item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_loctra_ISH"].ToString();
                                            }
                                            else
                                            {
                                                error = true;

                                                ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                                args.Archivo = archivo;
                                                args.Error = "Error en traslados";

                                                OnErrorImportarPM(args);
                                            }

                                            item.TAXAMNT = Decimal.Round( decimal.Parse(implocalTrasladosLocal.Importe), 2);
                                            item.TDTTXPUR = PMHeader.PRCHAMNT;
                                            item.TXDTTPUR = PMHeader.PRCHAMNT;

                                            items.Add(item);
                                        }
                                    }
                                    #endregion

                                    if (!error)
                                    {
                                        // Serialize the master vendor type in memory.
                                        eConnectType eConnectType = new eConnectType();
                                        MemoryStream memoryStream = new MemoryStream();
                                        XmlSerializer xmlSerializer = new XmlSerializer(eConnectType.GetType());

                                        if (metodo == 1)
                                        {
                                            PAInvoiceEntry.taPMTransactionInsert = PMHeader;
                                            PAInvoiceEntry.taPMTransactionTaxInsert_Items = items.ToArray();
                                            masterPMTransactionTypes.Add(PAInvoiceEntry);

                                            // Assign the master vendor types to the eConnectType.
                                            eConnectType.PMTransactionType = masterPMTransactionTypes.ToArray();
                                        }

                                        if (metodo == 2)
                                        {
                                            POPInvoiceEntry.taPopRcptHdrInsert = POPHeader;
                                            masterPOPReceivingsTypes.Add(POPInvoiceEntry);

                                            // Assign the master vendor types to the eConnectType.
                                            eConnectType.POPReceivingsType = masterPOPReceivingsTypes.ToArray();
                                        }

                                        // Serialize the eConnectType.
                                        xmlSerializer.Serialize(memoryStream, eConnectType);

                                        // Reset the position of the memory stream to the start.              
                                        memoryStream.Position = 0;

                                        // Create an XmlDocument from the serialized eConnectType in memory.
                                        XmlDocument xmlDocument = new XmlDocument();
                                        xmlDocument.Load(memoryStream);
                                        memoryStream.Close();

                                        string xmlEconn = xmlDocument.OuterXml;
                                        //xmlEconn = xmlEconn.Replace("</CURNCYID>", "</CURNCYID><DISAVAMT>0</DISAVAMT><ORTDISAM>0</ORTDISAM>");

                                        /*
                                        ErrorImportarPMEventArgs argse = new ErrorImportarPMEventArgs();
                                        argse.Archivo = archivo;
                                        argse.Error = xmlEconn;
                                        OnErrorImportarPM(argse);
                                        */
                                        
                                        // Call eConnect to process the XmlDocument.
                                        if (System.Configuration.ConfigurationManager.AppSettings[_pre + "_version"].ToString() == "2010")
                                            eConnectMethods.CreateEntity(connectionString, xmlEconn);
                                        else
                                            if (System.Configuration.ConfigurationManager.AppSettings[_pre + "_version"].ToString() == "10")
                                            {
                                                econn10.process p = new econn10.process(_pre);
                                                p.Execute(xmlEconn);
                                            }

                                        if (metodo == 1)
                                            this.FixDistributions(VCHRNMBR);
                                        else
                                            if (metodo == 2)
                                                this.ChangePopType(VCHRNMBR);

                                        ProcesoOkImportarPMEventArgs args = new ProcesoOkImportarPMEventArgs();
                                        args.Archivo = archivo;
                                        args.Msg = "Factura Importada";

                                        OnProcesoOkImportarPM(args);
                                        System.Threading.Thread.Sleep(100);
                                    }
                                }
                                else
                                {
                                    //factura ya existe en GP
                                    ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                    args.Archivo = archivo;
                                    args.Error = "Factura existente";

                                    OnErrorImportarPM(args);
                                }

                                if (!error)
                                {
                                    if (!this.FolioExists(1, VCHRNMBR))
                                    {
                                        this.InsertFolio(1, VCHRNMBR, timbreDigital.UUID);

                                        ProcesoOkImportarPMEventArgs args = new ProcesoOkImportarPMEventArgs();
                                        args.Archivo = archivo;
                                        args.Msg = "Folio Importado";

                                        OnProcesoOkImportarPM(args);
                                        System.Threading.Thread.Sleep(100);
                                    }
                                    else
                                    {
                                        ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                        args.Archivo = archivo;
                                        args.Error = "Folio ya cargado";

                                        OnErrorImportarPM(args);
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                            args.Archivo = archivo;
                            args.Error = ex.Message + " - " + ex.StackTrace + " - " + ex.Source;

                            OnErrorImportarPM(args);
                        }
                        finally
                        {
                            eConnectMethods.Dispose();
                        }
                    }
                }
            }
        }
            
        /// <summary>
        /// 
        /// </summary>
        /// <param name="metodo">1: PM - 2: POP</param>
        /// <returns></returns>
        private string getNum(int metodo)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[_pre + "_version"].ToString() == "2010")
            {
                GetNextDocNumbers myDocNumbers = new GetNextDocNumbers();
                GetSopNumber mySopNumber = new GetSopNumber();

                //n.Add(mySopNumber.GetNextSopNumber(3, "STDINV", connString));

                // Use each method of the GetNextDocNumber object to retrieve the next document number 
                // for the available Microsoft Dynamics GP document types
                if (metodo == 1)
                    return myDocNumbers.GetPMNextVoucherNumber(IncrementDecrement.Increment, connectionString);
                else
                    if (metodo == 2)
                        return myDocNumbers.GetNextPOPReceiptNumber(IncrementDecrement.Increment, connectionString);
                    else
                        return null;
            }
            else
                if (System.Configuration.ConfigurationManager.AppSettings[_pre + "_version"].ToString() == "10")
                {
                    econn10.process p = new econn10.process(_pre);
                    return p.getNum(metodo);
                }
                else
                    return null;
        }

        private string getVendroID(string rfc)
        {
            string sql = "select vendorid from pm00200 where txrgnnum = @txrgnnum";
            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add("@txrgnnum", SqlDbType.VarChar, 50).Value = rfc;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0]["vendorid"].ToString();
                    else
                        return null;
                }
            }
        }

        public string GetVchrnmbrFacturaExists(string VENDORID, string DOCNUMBR, Int16 DOCTYPE, DateTime DOCDATE, decimal DOCAMNT)
        {
            return GetVchrnmbrFactura(VENDORID, DOCNUMBR, DOCTYPE, DOCDATE, DOCAMNT);
        }

        private string GetVchrnmbrFactura(string VENDORID, string DOCNUMBR, Int16 DOCTYPE, DateTime DOCDATE, decimal DOCAMNT)
        {
            string Vchrnmbr = null;

            Vchrnmbr = FacturaExists("pm20000", "VCHRNMBR", VENDORID, DOCNUMBR, DOCTYPE, DOCDATE, DOCAMNT);
            if (Vchrnmbr == null)
                Vchrnmbr = FacturaExists("pm30200", "VCHRNMBR", VENDORID, DOCNUMBR, DOCTYPE, DOCDATE, DOCAMNT);
            if (Vchrnmbr == null)
                Vchrnmbr = FacturaExists("pm10000", "VCHRNMBR", VENDORID, DOCNUMBR, DOCTYPE, DOCDATE, DOCAMNT);

            /*
            if (Vchrnmbr == null)
                Vchrnmbr = FacturaExists("PM00400", "VCHRNMBR", VENDORID, DOCNUMBR, DOCTYPE, DOCDATE, DOCAMNT);
            if (Vchrnmbr == null)
                Vchrnmbr = FacturaExists("POP10300", "VCHRNMBR", VENDORID, DOCNUMBR, DOCTYPE, DOCDATE, DOCAMNT);
            if (Vchrnmbr == null)
                Vchrnmbr = FacturaExists("MC020103", "VCHRNMBR", VENDORID, DOCNUMBR, DOCTYPE, DOCDATE, DOCAMNT);
            */

            return Vchrnmbr;
        }

        private string FacturaExists(string tabla, string campo, string VENDORID, string DOCNUMBR, Int16 DOCTYPE, DateTime DOCDATE, decimal DOCAMNT)
        {
            //string sql = "select "+ campo +" from " + tabla + " where VENDORID = @VENDORID and DOCNUMBR = @DOCNUMBR and DOCTYPE = @DOCTYPE and DOCDATE = @DOCDATE and DOCAMNT = @DOCAMNT";
            string sql = "select " + campo + " from " + tabla + " where VENDORID = @VENDORID and DOCNUMBR = @DOCNUMBR";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@VENDORID", SqlDbType.VarChar, 15).Value = VENDORID;
                cmd.Parameters.Add("@DOCNUMBR", SqlDbType.VarChar, 21).Value = DOCNUMBR;
                //cmd.Parameters.Add("@DOCTYPE", SqlDbType.SmallInt).Value = DOCTYPE;
                //cmd.Parameters.Add("@DOCDATE", SqlDbType.DateTime).Value = DOCDATE;
                //cmd.Parameters.Add("@DOCAMNT", SqlDbType.Decimal).Value = DOCAMNT;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0][campo].ToString();
                    else
                        return null;
                }
            }
        }

        private string FacturaPOPExists(string VENDORID, string DOCNUMBR, DateTime DOCDATE)
        {
            //string sql = "select POPRCTNM from POP10300 where VENDORID = @VENDORID and VNDDOCNM = @DOCNUMBR and receiptdate = @DOCDATE";
            string sql = "select POPRCTNM from vwPopPmDocumentosDeCompraLoteAbieHist where VENDORID = @VENDORID and VNDDOCNM = @DOCNUMBR";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@VENDORID", SqlDbType.VarChar, 15).Value = VENDORID;
                cmd.Parameters.Add("@DOCNUMBR", SqlDbType.VarChar, 21).Value = DOCNUMBR;
                //cmd.Parameters.Add("@DOCDATE", SqlDbType.DateTime).Value = DOCDATE;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0]["POPRCTNM"].ToString();
                    else
                        return null;
                }
            }
        }

        public bool FolioExistsBlank(Int16 DOCTYPE, string VCHRNMBR)
        {
            string sql = "select * from ACA_IETU00400 where DOCTYPE = @DOCTYPE and VCHRNMBR = @VCHRNMBR";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@VCHRNMBR", SqlDbType.VarChar, 21).Value = VCHRNMBR;
                cmd.Parameters.Add("@DOCTYPE", SqlDbType.SmallInt).Value = DOCTYPE;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (dt.Rows.Count > 0 && dt.Rows[0]["MexFolioFiscal"].ToString().Trim() == "");
                }
            }
        }

        public bool FolioExists(Int16 DOCTYPE, string VCHRNMBR)
        {
            string sql = "select * from ACA_IETU00400 where DOCTYPE = @DOCTYPE and VCHRNMBR = @VCHRNMBR";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@VCHRNMBR", SqlDbType.VarChar, 21).Value = VCHRNMBR;
                cmd.Parameters.Add("@DOCTYPE", SqlDbType.SmallInt).Value = DOCTYPE;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    return (dt.Rows.Count > 0 && dt.Rows[0]["MexFolioFiscal"].ToString().Trim() != "");
                }
            }
        }

        public void ChangePopType(string VCHRNMBR)
        {
            string sql = "update POP10300 set poptype=3 where POPRCTNM = @VCHRNMBR";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@VCHRNMBR", SqlDbType.VarChar, 21).Value = VCHRNMBR;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void InsertFolio(Int16 DOCTYPE, string VCHRNMBR, string uuid)
        {
            string sql = "";

            if (!this.FolioExistsBlank(DOCTYPE, VCHRNMBR))
                sql = "insert into ACA_IETU00400 (DOCTYPE, VCHRNMBR, ACA_Gasto, ACA_IVA, MexFolioFiscal) values (@DOCTYPE, @VCHRNMBR, 1, 1, @MexFolioFiscal)";
            else
                sql = "update ACA_IETU00400 set MexFolioFiscal = @MexFolioFiscal where DOCTYPE = @DOCTYPE and VCHRNMBR = @VCHRNMBR";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@DOCTYPE", SqlDbType.SmallInt).Value = DOCTYPE;
                cmd.Parameters.Add("@VCHRNMBR", SqlDbType.VarChar, 21).Value = VCHRNMBR;
                cmd.Parameters.Add("@MexFolioFiscal", SqlDbType.VarChar, 41).Value = uuid;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public void FixDistributions(string VCHRNMBR)
        {
            string sql = "update pm10100 set CRDTAMNT= DEBITAMT*-1 , DEBITAMT=0 where VCHRNMBR = @VCHRNMBR and DEBITAMT<0";

            using (SqlConnection conn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.CommandType = CommandType.Text;
                //cmd.CommandTimeout = Settings.Default.reportTimeout;
                cmd.Parameters.Add("@VCHRNMBR", SqlDbType.VarChar, 21).Value = VCHRNMBR;

                cmd.CommandTimeout = 0;
                cmd.Connection.Open();
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region Eventos
        #region Error
        public event EventHandler<ErrorImportarPMEventArgs> ErrorImportarPM;

        protected virtual void OnErrorImportarPM(ErrorImportarPMEventArgs e)
        {
            EventHandler<ErrorImportarPMEventArgs> handler = ErrorImportarPM;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class ErrorImportarPMEventArgs : EventArgs
        {
            public string Archivo { get; set; }
            public string Error { get; set; }
        }
        #endregion

        #region OK
        public event EventHandler<ProcesoOkImportarPMEventArgs> ProcesoOkImportarPM;

        protected virtual void OnProcesoOkImportarPM(ProcesoOkImportarPMEventArgs e)
        {
            EventHandler<ProcesoOkImportarPMEventArgs> handler = ProcesoOkImportarPM;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public class ProcesoOkImportarPMEventArgs : EventArgs
        {
            public string Archivo { get; set; }
            public string Msg { get; set; }
        }
        #endregion
        #endregion
    }
}
