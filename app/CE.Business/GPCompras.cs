using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Threading.Tasks;

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
    public class GPCompras
    {
        private string connectionString = "";
        private string _pre = "";

        public GPCompras(string pre)
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings[pre].ToString();
            _pre = pre;
        }



        #region Importar Facturas Electronicas

        public async Task IntegrarDocumentosGPAsync(List<string> archivos, int metodo)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            XNamespace cfdi = @"http://www.sat.gob.mx/cfd/3";
            XNamespace tfd = @"http://www.sat.gob.mx/TimbreFiscalDigital";
            XNamespace implocal = @"http://www.sat.gob.mx/implocal";

            string BACHNUMB = DateTime.Now.ToString("yyyyMMddHHmmss");
            string formatoFecha = System.Configuration.ConfigurationManager.AppSettings[_pre + "_FormatoFecha"].ToString();

            foreach (string archivo in archivos)
            {
                try
                {
                    if (System.IO.File.Exists(archivo))
                    {
                        string xml = System.IO.File.ReadAllText(archivo);
                        XDocument xdoc = XDocument.Parse(xml);

                        string tipoComprobante = AveriguarElTipoDeCfdi(xdoc, cfdi);
                        switch (tipoComprobante)
                        {
                            case "I":
                                bool integrado = IntegraFacturaPmPop(xdoc, cfdi, tfd, implocal, archivo, metodo, formatoFecha, BACHNUMB);
                                if (integrado)
                                    await CargaComprobanteCfdiAsync(xdoc, cfdi, tfd, archivo);
                                break;
                            case "P":
                                await CargaComprobanteCfdiAsync(xdoc, cfdi, tfd, archivo);
                                break;
                            default:
                                ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                args.Archivo = archivo;
                                args.Error = "Este comprobante no se puede integrar a GP porque no es un comprobante de Ingreso ni de Pago";
                                OnErrorImportarPM(args);
                                break;
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
            }
        }

        /// <summary>
        /// Importar facturas a GP
        /// </summary>
        /// <param name="archivos">Path de los archivos</param>
        /// <param name="metodo">1: PM - 2: POP</param>
        /// 
        public void Importar(List<string> archivos, int metodo)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

            XNamespace cfdi = @"http://www.sat.gob.mx/cfd/3";
            XNamespace tfd = @"http://www.sat.gob.mx/TimbreFiscalDigital";
            XNamespace implocal = @"http://www.sat.gob.mx/implocal";

            string BACHNUMB = DateTime.Now.ToString("yyyyMMddHHmmss");
            string formatoFecha = System.Configuration.ConfigurationManager.AppSettings[_pre + "_FormatoFecha"].ToString();

            foreach (string archivo in archivos)
            {
                try
                {
                    if (System.IO.File.Exists(archivo))
                    {
                        string xml = System.IO.File.ReadAllText(archivo);
                        XDocument xdoc = XDocument.Parse(xml);

                        string tipoComprobante = AveriguarElTipoDeCfdi(xdoc, cfdi);
                        switch (tipoComprobante)
                        {
                            case "I":
                                IntegraFacturaPmPop(xdoc, cfdi, tfd, implocal, archivo, metodo, formatoFecha, BACHNUMB);
                                break;
                            case "P":
                                CargaComprobanteCfdi(xdoc, cfdi, tfd, archivo);
                                break;
                            default:
                                ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                args.Archivo = archivo;
                                args.Error = "Este comprobante no se puede integrar a GP porque no es un comprobante de Ingreso ni de Pago";
                                OnErrorImportarPM(args);
                                break;
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
            }
        }

        private async Task<int> CargaComprobanteCfdiAsync(XDocument xdoc, XNamespace cfdi, XNamespace tfd, string carpetaYArchivo)
        {
            string carpeta = Path.GetDirectoryName(carpetaYArchivo);
            string archivo = Path.GetFileName(carpetaYArchivo);
            var comprobante = (from c in xdoc.Descendants(cfdi + "Comprobante")
                               select new
                               {
                                   folio = c.Attribute("Folio") == null ? "" : c.Attribute("Folio").Value,
                                   fecha = c.Attribute("Fecha").Value,
                                   metodoDePago = c.Attribute("MetodoPago") == null ? string.Empty : c.Attribute("MetodoPago").Value,
                                   Moneda = c.Attribute("Moneda") == null ? "" : c.Attribute("Moneda").Value,
                                   total = c.Attribute("Total").Value,
                                   tipoDeComprobante = c.Attribute("TipoDeComprobante").Value,
                               }).FirstOrDefault();

            var emisor = (from c in xdoc.Descendants(cfdi + "Emisor")
                          select new
                          {
                              rfc = c.Attribute("Rfc").Value,
                              nombre = c.Attribute("Nombre") == null ? "" : c.Attribute("Nombre").Value
                          }).FirstOrDefault();

            var timbre = (from c in xdoc.Descendants(tfd + "TimbreFiscalDigital")
                          select new
                          {
                              UUID = c.Attribute("UUID").Value
                          }).FirstOrDefault();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("dace.spComprobanteCFDIInsDel", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UUID", timbre.UUID));
                    cmd.Parameters.Add(new SqlParameter("@TIPOCOMPROBANTE", comprobante.tipoDeComprobante));
                    cmd.Parameters.Add(new SqlParameter("@FOLIO", comprobante.folio));
                    cmd.Parameters.Add(new SqlParameter("@FECHA", comprobante.fecha));
                    cmd.Parameters.Add(new SqlParameter("@TOTAL", decimal.Parse(comprobante.total)));
                    cmd.Parameters.Add(new SqlParameter("@MONEDA", comprobante.Moneda));
                    cmd.Parameters.Add(new SqlParameter("@METODOPAGO", comprobante.metodoDePago));
                    cmd.Parameters.Add(new SqlParameter("@EMISOR_RFC", emisor.rfc));
                    cmd.Parameters.Add(new SqlParameter("@RESUMENCFDI", string.Empty));
                    cmd.Parameters.Add(new SqlParameter("@NOMBREARCHIVO", archivo));
                    cmd.Parameters.Add(new SqlParameter("@CARPETAARCHIVO", carpeta));
                    cmd.Parameters.Add(new SqlParameter("@comprobanteXml", xdoc.ToString()));
                    cmd.CommandTimeout = 0;
                    await conn.OpenAsync();
                    return await cmd.ExecuteNonQueryAsync();

                }

            }
        }

        private void CargaComprobanteCfdi(XDocument xdoc, XNamespace cfdi, XNamespace tfd, string carpetaYArchivo)
        {
            string carpeta = Path.GetDirectoryName(carpetaYArchivo);
            string archivo = Path.GetFileName(carpetaYArchivo);
            var comprobante = (from c in xdoc.Descendants(cfdi + "Comprobante")
                                select new
                                {
                                    folio = c.Attribute("Folio") == null ? "" : c.Attribute("Folio").Value,
                                    fecha = c.Attribute("Fecha").Value,
                                    metodoDePago = c.Attribute("MetodoPago") == null ? string.Empty : c.Attribute("MetodoPago").Value,
                                    Moneda = c.Attribute("Moneda") == null ? "" : c.Attribute("Moneda").Value,
                                    total = c.Attribute("Total").Value,
                                    tipoDeComprobante = c.Attribute("TipoDeComprobante").Value,
                                }).FirstOrDefault();

            var emisor = (from c in xdoc.Descendants(cfdi + "Emisor")
                            select new
                            {
                                rfc = c.Attribute("Rfc").Value,
                                nombre = c.Attribute("Nombre") == null ? "" : c.Attribute("Nombre").Value
                            }).FirstOrDefault();

            var timbre = (from c in xdoc.Descendants(tfd + "TimbreFiscalDigital")
                                  select new
                                  {
                                      UUID = c.Attribute("UUID").Value
                                  }).FirstOrDefault();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("dace.spComprobanteCFDIInsDel", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UUID", timbre.UUID));
                    cmd.Parameters.Add(new SqlParameter("@TIPOCOMPROBANTE", comprobante.tipoDeComprobante));
                    cmd.Parameters.Add(new SqlParameter("@FOLIO", comprobante.folio));
                    cmd.Parameters.Add(new SqlParameter("@FECHA", comprobante.fecha));
                    cmd.Parameters.Add(new SqlParameter("@TOTAL", decimal.Parse(comprobante.total)));
                    cmd.Parameters.Add(new SqlParameter("@MONEDA", comprobante.Moneda));
                    cmd.Parameters.Add(new SqlParameter("@METODOPAGO", comprobante.metodoDePago));
                    cmd.Parameters.Add(new SqlParameter("@EMISOR_RFC", emisor.rfc));
                    cmd.Parameters.Add(new SqlParameter("@RESUMENCFDI", string.Empty));
                    cmd.Parameters.Add(new SqlParameter("@NOMBREARCHIVO", archivo));
                    cmd.Parameters.Add(new SqlParameter("@CARPETAARCHIVO", carpeta));
                    cmd.Parameters.Add(new SqlParameter("@comprobanteXml", xdoc.ToString()));
                    cmd.CommandTimeout = 0;
                    cmd.ExecuteNonQuery();

                }
            }
        }

        /// <summary>
        /// Integra una factura pm o pop a partir de un archivo xml (xdoc)
        /// </summary>
        /// <param name="xdoc">archivo xml cfdi</param>
        /// <param name="cfdi">namespace</param>
        /// <param name="archivo">nombre del archivo</param>
        /// <param name="metodo">1 pm, 2 pop</param>
        /// <param name="formatoFecha"></param>
        /// <param name="BACHNUMB"></param>
        private bool IntegraFacturaPmPop(XDocument xdoc, XNamespace cfdi, XNamespace tfd, XNamespace implocal, string archivo, int metodo, string formatoFecha, string BACHNUMB)
        {
            bool error = false;
            using (eConnectMethods eConnectMethods = new eConnectMethods())
            {
                try
                {
                    eConnectMethods.RequireProxyService = true;
                    List<PMTransactionType> masterPMTransactionTypes = new List<PMTransactionType>();
                    List<POPReceivingsType> masterPOPReceivingsTypes = new List<POPReceivingsType>();
                    PMTransactionType PAInvoiceEntry = new PMTransactionType();
                    POPReceivingsType POPInvoiceEntry = new POPReceivingsType();
                    taPMTransactionInsert PMHeader = new taPMTransactionInsert();
                    taPopRcptHdrInsert POPHeader = new taPopRcptHdrInsert();

                    List<taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert> items = new List<taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert>();

                    var comprobantes = (from c in xdoc.Descendants(cfdi + "Comprobante")
                                        select new
                                        {
                                            folio = c.Attribute("Folio") == null ? "" : c.Attribute("Folio").Value,
                                            fecha = c.Attribute("Fecha").Value,
                                            formaDePago = c.Attribute("FormaPago").Value,
                                            condicionesDePago = c.Attribute("CondicionesDePago") == null ? "" : c.Attribute("CondicionesDePago").Value,
                                            subTotal = c.Attribute("SubTotal").Value,
                                            TipoCambio = c.Attribute("TipoCambio") == null ? "" : c.Attribute("TipoCambio").Value,
                                            Moneda = c.Attribute("Moneda") == null ? "" : c.Attribute("Moneda").Value,
                                            total = c.Attribute("Total").Value,
                                            tipoDeComprobante = c.Attribute("TipoDeComprobante").Value,
                                            metodoDePago = c.Attribute("MetodoPago").Value,
                                            LugarExpedicion = c.Attribute("LugarExpedicion").Value,
                                            Descuento = c.Attribute("Descuento") == null ? "0" : c.Attribute("Descuento").Value,
                                        }).ToList();
                    var comprobante = comprobantes[0];

                    var impuestos = (from c in xdoc.Descendants(cfdi + "Impuestos").Where(x => x.Attribute("TotalImpuestosTrasladados") != null || x.Attribute("TotalImpuestosRetenidos") != null)
                                     select new
                                     {
                                         totalImpuestosTrasladados = c.Attribute("TotalImpuestosTrasladados") == null ? "0" : c.Attribute("TotalImpuestosTrasladados").Value,
                                         totalImpuestosRetenidos = c.Attribute("TotalImpuestosRetenidos") == null ? "0" : c.Attribute("TotalImpuestosRetenidos").Value
                                     }).ToList();

                    //var impuesto = impuestos[0];

                    var retenciones = (from c in xdoc.Descendants(cfdi + "Impuestos").Where(x => x.Attribute("TotalImpuestosRetenidos") != null).Descendants(cfdi + "Retencion")
                                       select new
                                       {
                                           impuesto = c.Attribute("Impuesto").Value,
                                           importe = c.Attribute("Importe").Value
                                       }).ToList();
                    var traslados = (from c in xdoc.Descendants(cfdi + "Impuestos").Where(x => x.Attribute("TotalImpuestosTrasladados") != null).Descendants(cfdi + "Traslado")
                                     select new
                                     {
                                         impuesto = c.Attribute("Impuesto").Value,
                                         tipoFActor = c.Attribute("TipoFactor").Value,
                                         tasa = c.Attribute("TasaOCuota").Value,
                                         importe = c.Attribute("Importe").Value
                                     }).ToList();

                    var impuestosLocales = (from c in xdoc.Descendants(implocal + "ImpuestosLocales")
                                            select new
                                            {
                                                TotaldeTraslados = c.Attribute("TotaldeTraslados") == null ? "0" : c.Attribute("TotaldeTraslados").Value,
                                                TotaldeRetenciones = c.Attribute("TotaldeRetenciones") == null ? "0" : c.Attribute("TotaldeRetenciones").Value
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
                                         cantidad = c.Attribute("Cantidad").Value,
                                         unidad = c.Attribute("ClaveUnidad").Value,
                                         noIdentificacion = c.Attribute("NoIdentificacion") == null ? "" : c.Attribute("NoIdentificacion").Value,
                                         descripcion = c.Attribute("Descripcion").Value,
                                         valorUnitario = c.Attribute("ValorUnitario").Value,
                                         importe = c.Attribute("Importe").Value
                                     }).ToList();

                    var emisores = (from c in xdoc.Descendants(cfdi + "Emisor")
                                    select new
                                    {
                                        rfc = c.Attribute("Rfc").Value,
                                        nombre = c.Attribute("Nombre") == null ? "" : c.Attribute("Nombre").Value
                                    }).ToList();

                    var emisor = emisores[0];

                    var timbresDigital = (from c in xdoc.Descendants(tfd + "TimbreFiscalDigital")
                                          select new
                                          {
                                              UUID = c.Attribute("UUID").Value
                                          }).ToList();

                    var timbreDigital = timbresDigital[0];

                    string vendorid = getVendroID(emisor.rfc);
                    if (string.IsNullOrEmpty(vendorid))
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
                        if (string.IsNullOrEmpty(folio))
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

                        if (string.IsNullOrEmpty(VCHRNMBR))
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
                                PMHeader.DOCAMNT = Decimal.Round(decimal.Parse(comprobante.total), 2);
                                PMHeader.CHRGAMNT = PMHeader.DOCAMNT;
                                PMHeader.DOCDATE = DateTime.Parse(comprobante.fecha).ToString(formatoFecha);
                                PMHeader.TAXSCHID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_TAXSCHID"].ToString();
                                PMHeader.PRCHAMNT = Decimal.Round(decimal.Parse(comprobante.subTotal), 2);
                                PMHeader.TRDISAMT = Decimal.Round(decimal.Parse(comprobante.Descuento), 2);

                                decimal totalImpuestosTrasladados = 0;
                                if (traslados.Count > 0)
                                    totalImpuestosTrasladados = Decimal.Round(traslados.Sum(x => decimal.Parse(x.importe)), 2);

                                //decimal totalImpuestosTrasladados = decimal.Parse(impuesto.totalImpuestosTrasladados);
                                if (impuestosLocales.Count > 0)
                                {
                                    var impuestoLocal = impuestosLocales[0];
                                    totalImpuestosTrasladados += Decimal.Round(decimal.Parse(impuestoLocal.TotaldeTraslados), 2);
                                }

                                decimal tImpuestosRetenidos = 0;
                                if (impuestos.Count > 0)
                                    decimal.TryParse(impuestos.First().totalImpuestosRetenidos, out tImpuestosRetenidos);

                                PMHeader.TAXAMNT = totalImpuestosTrasladados - Decimal.Round(tImpuestosRetenidos, 2);
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

                                    if (retencion.impuesto.Trim() == "001")     //isr
                                        item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_ret_ISR"].ToString();
                                    else
                                        if (retencion.impuesto.Trim() == "002") //iva
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

                                    item.TAXAMNT = -1 * Decimal.Round(retencion.importe, 2);
                                    item.TDTTXPUR = PMHeader.PRCHAMNT;
                                    item.TXDTTPUR = PMHeader.PRCHAMNT;

                                    items.Add(item);
                                }
                                #endregion

                                #region Traslados
                                var trasladoGroup = traslados
                                                    .GroupBy(x => new { x.tasa, x.impuesto })
                                                    .Select(g => new
                                                    {
                                                        g.Key.impuesto,
                                                        g.Key.tasa,
                                                        importe = g.Sum(y => decimal.Parse(y.importe))
                                                    });

                                foreach (var lTraslado in trasladoGroup)
                                {
                                    taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert item = new taPMTransactionTaxInsert_ItemsTaPMTransactionTaxInsert();

                                    item.VENDORID = PMHeader.VENDORID;
                                    item.VCHRNMBR = PMHeader.VCHNUMWK;
                                    item.DOCTYPE = PMHeader.DOCTYPE;
                                    item.BACHNUMB = PMHeader.BACHNUMB;

                                    if (lTraslado.impuesto.Trim() == "002") //iva
                                    {
                                        if (decimal.Parse(lTraslado.tasa) == decimal.Parse("0"))
                                            item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_tra_IVA0"].ToString();
                                        else
                                            if (decimal.Equals(decimal.Parse(lTraslado.tasa), decimal.Parse("0.11")))
                                            item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_tra_IVA11"].ToString();
                                        else
                                                if (decimal.Equals(decimal.Parse(lTraslado.tasa), decimal.Parse("0.16")))
                                            item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_tra_IVA16"].ToString();
                                        else
                                        {
                                            error = true;

                                            ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                            args.Archivo = archivo;
                                            args.Error = "Error en traslados IVA";

                                            OnErrorImportarPM(args);
                                        }

                                    }
                                    else
                                    {
                                        if (lTraslado.impuesto.Trim() == "003") //IEPS
                                            item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_tra_IEPS"].ToString();
                                        else
                                        {
                                            error = true;

                                            ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                            args.Archivo = archivo;
                                            args.Error = "Error en traslados IEPS";

                                            OnErrorImportarPM(args);
                                        }
                                    }

                                    item.TAXAMNT = Decimal.Round(lTraslado.importe, 2);
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

                                    if (implocalTrasladosLocal.ImpLocTrasladado == "ISH" || implocalTrasladosLocal.ImpLocTrasladado.ToUpper() == "IMPUESTO SOBRE HOSPEDAJE")
                                    {
                                        item.TAXDTLID = System.Configuration.ConfigurationManager.AppSettings[_pre + "_loctra_ISH"].ToString();
                                    }
                                    else
                                    {
                                        error = true;

                                        ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                                        args.Archivo = archivo;
                                        args.Error = "Error en traslados locales. Ej. ISH";

                                        OnErrorImportarPM(args);
                                    }

                                    item.TAXAMNT = Decimal.Round(decimal.Parse(implocalTrasladosLocal.Importe), 2);
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

                        //if (!error)
                        //{
                        //    CargaComprobanteCfdi();
                        //}
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
                    return !error;
                }
                catch (Exception ex)
                {
                    ErrorImportarPMEventArgs args = new ErrorImportarPMEventArgs();
                    args.Archivo = archivo;
                    args.Error = ex.Message + " - " + ex.StackTrace + " - " + ex.Source;

                    OnErrorImportarPM(args);
                    return false;
                }
                finally
                {
                    eConnectMethods.Dispose();
                }

            }

        }

        static public string AveriguarElTipoDeCfdi(XDocument xdoc, XNamespace cfdi)
        {
            var comprobantes = (from c in xdoc.Descendants(cfdi + "Comprobante")
                                select new
                                {
                                    tipoDeComprobante = c.Attribute("TipoDeComprobante").Value,
                                }).FirstOrDefault();
            return(comprobantes.tipoDeComprobante);
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
