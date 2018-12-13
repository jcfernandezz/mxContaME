using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using CE.Model;
using System.Xml.Linq;
using CE.Business;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using ClosedXML.Excel;
using System.IO;

namespace CE.WinFormUI
{
    public partial class Form1 : Form
    {
        UtilitarioArchivos utileria;
        private List<ParametrosDeArchivo> lParametros = new List<ParametrosDeArchivo>();
        public Form1()
        {
            InitializeComponent();
        }

        private string companySelected()
        {
            return ((ComboBoxItem)cmbEmpresas.SelectedItem).Value;
        }

        private List<DcemVwContabilidad> listado = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            bool error = false;
            int count = 1;
            while (!error)
            {
                if (System.Configuration.ConfigurationManager.ConnectionStrings["GP_" + count.ToString()] != null)
                {
                    LecturaContabilidadFactory oLC = new LecturaContabilidadFactory("GP_" + count.ToString());

                    cmbEmpresas.Items.Add(new ComboBoxItem("GP_" + count.ToString(), oLC.GetCompany()));
                    count++;
                }
                else
                    error = true;
            }

            cmbEmpresas.SelectedIndex = 0;
            lblUsuario.Text = Environment.UserDomainName + "\\" + Environment.UserName;
            lblFecha.Text = DateTime.Now.ToString("dd/MM/yyyy");

            lblProcesos.Text = "Iniciando carga de parámetros..." + Environment.NewLine;
            inicializarExportarXML();
            inicializarImportarXML();
            lblProcesos.Text = "Listo!" + Environment.NewLine;

        }

        private void cmbEmpresas_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarVista();
        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region Contabilidad Electrónica ExportarXML
        private void inicializarExportarXML()
        {
            for (int j = DateTime.Now.Year; j >= DateTime.Now.Year - 10; j--)
            {
                cmbAno.Items.Add(new ComboBoxItem(j.ToString(), j.ToString()));
            }

            cmbTipoDoc.Items.Add(new ComboBoxItem("-1", "-Todos-"));
            cmbTipoDoc.Items.Add(new ComboBoxItem("Catálogo", "Catálogo"));
            cmbTipoDoc.Items.Add(new ComboBoxItem("Balanza", "Balanza"));
            cmbTipoDoc.Items.Add(new ComboBoxItem("Pólizas", "Pólizas"));
            cmbTipoDoc.Items.Add(new ComboBoxItem("Auxiliar Cuentas", "Auxiliar Cuentas"));
            cmbTipoDoc.Items.Add(new ComboBoxItem("Auxiliar folios", "Auxiliar folios"));

            cmbTipoDoc.SelectedIndex = 2;
            cmbAno.SelectedIndex = 0;

            cmbAno.SelectedIndexChanged += new EventHandler(cmbAno_SelectedIndexChanged);
            cmbTipoDoc.SelectedIndexChanged += new EventHandler(cmbTipoDoc_SelectedIndexChanged);
            cmbEmpresas.SelectedIndexChanged += new EventHandler(cmbEmpresas_SelectedIndexChanged);

            cargarVista();

            string archivo1 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo1"].ToString();
            string archivo2 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo2"].ToString();
            string archivo3 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo3"].ToString();
            string archivo4 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo4"].ToString();
            string archivo5 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo5"].ToString();

            lParametros.Add(new ParametrosDeArchivo() { Tipo= "Catálogo", Archivo=archivo1, FuncionSql= "DCEMFCNCATALOGOXML", NameSpace= "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/CatalogoCuentas", Esquema= "CatalogoCuentas_1_3.xsd" });
            lParametros.Add(new ParametrosDeArchivo() { Tipo = "Balanza", Archivo = archivo2, FuncionSql = "DCEMFCNBALANCE", NameSpace= "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/BalanzaComprobacion", Esquema = "BalanzaComprobacion_1_3.xsd" });
            lParametros.Add(new ParametrosDeArchivo() { Tipo = "Pólizas", Archivo = archivo3, FuncionSql = "DCEMFCNPOLIZAS", NameSpace= "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/PolizasPeriodo", Esquema = "PolizasPeriodo_1_3.xsd" });
            lParametros.Add(new ParametrosDeArchivo() { Tipo = "Auxiliar Cuentas", Archivo = archivo4, FuncionSql = "DCEMFCNAUXILIARCTAS", NameSpace= "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/AuxiliarCtas", Esquema = "AuxiliarCtas_1_3.xsd" });
            lParametros.Add(new ParametrosDeArchivo() { Tipo = "Auxiliar folios", Archivo = archivo5, FuncionSql = "DCEMFCNAUXILIARFOLIOS", NameSpace= "http://www.sat.gob.mx/esquemas/ContabilidadE/1_3/AuxiliarFolios", Esquema = "AuxiliarFolios_1_3.xsd" });
        }

        private void cargarVista()
        {
            int ano = 0;

            if (Int32.Parse(((ComboBoxItem)cmbAno.SelectedItem).Value) != -1)
                ano = Int32.Parse(((ComboBoxItem)cmbAno.SelectedItem).Value);

            string tipo = ((ComboBoxItem)cmbTipoDoc.SelectedItem).Value;
            if (tipo == "-1")
                tipo = null;

            LecturaContabilidadFactory oL = new LecturaContabilidadFactory(companySelected());
            
            var l = oL.GetAll(ano, tipo);

            //string empresa = oL.GetCompany();
            //lblEmpresa.Text = empresa;

            bindingSource2.DataSource = l;
            gridVista.AutoGenerateColumns = false;
            gridVista.DataSource = bindingSource2;
            gridVista.ClearSelection();
            //InicializaCheckBoxDelGrid(gridVista, 0, false);

            gridVista.AutoResizeColumns();
//            gridVista.RowHeadersVisible = false;
        }

        void InicializaCheckBoxDelGrid(DataGridView dataGrid, short idxChkBox, bool marca)
        {
            for (int r = 0; r < dataGrid.RowCount; r++)
            {
                dataGrid[idxChkBox, r].Value = marca;
            }
            dataGrid.EndEdit();
        }

        private object mostrarContenido(Int16 year1, Int16 periodid, string tipo)
        {
            string company = companySelected();
            object items = ContabilidadElectronicaPresentacion.ObtieneContenidoLinqDeXML(year1, periodid, tipo, company, lParametros);

            bindingSource1.DataSource = items;
            grid.AutoGenerateColumns = true;
            grid.DataSource = bindingSource1;
            grid.RowHeadersVisible = false;
            grid.AutoResizeColumns();

            return items;
        }

        private void mostrarMensaje()
        {
            XDocument xdoc = XDocument.Parse("<Mensajes><Mensaje texto = \"Para ver el contenido presione el botón Mostrar.\" /></Mensajes>");
            object items = null;
            items = xdoc.Descendants("Mensaje").Select(x => new { Atención = x.Attribute("texto").Value });

            bindingSource1.DataSource = items;
            grid.AutoGenerateColumns = true;
            grid.DataSource = bindingSource1;
            grid.RowHeadersVisible = false;
            grid.AutoResizeColumns();
        }

        private void gridVista_SelectionChanged(object sender, EventArgs e)
        {
            if (gridVista.SelectedRows.Count != 0)
            {
                try
                {
                    DcemVwContabilidad item = (DcemVwContabilidad)gridVista.SelectedRows[0].DataBoundItem;
                    if (item != null)
                        mostrarMensaje();
                    //mostrarContenido(item.year1, item.periodid, item.tipodoc);
                }
                catch
                {
                    grid.DataSource = null;
                }
            }
        }        

        private void cmbAno_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarVista();
        }

        private void cmbTipoDoc_SelectedIndexChanged(object sender, EventArgs e)
        {
            cargarVista();
        }

        private void gridVista_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                DataGridView dgv = sender as DataGridView;
                DcemVwContabilidad data = dgv.Rows[e.RowIndex].DataBoundItem as DcemVwContabilidad;

                if (data != null && data.existe)
                {
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LawnGreen;
                }
            }
        }

        private void tsButtonGenerar_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblProcesos.Text = "";

            LecturaContabilidadFactory oL = new LecturaContabilidadFactory(companySelected());
            oL.LParametros = lParametros;

            string directorio = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_directorio"].ToString();
            List<DcemVwContabilidad> l = new List<DcemVwContabilidad>();

            foreach (DataGridViewRow row in gridVista.Rows)
            {
                DataGridViewCheckBoxCell marca = row.Cells[0] as DataGridViewCheckBoxCell;
                
                if (marca.Value != null && (marca.Value.Equals(true) || marca.Value.ToString().Equals("1")))
                    {
                    var item = (DcemVwContabilidad)row.DataBoundItem;


                    if (item.tipodoc == "Pólizas" || item.tipodoc == "Auxiliar Cuentas" || item.tipodoc == "Auxiliar folios")
                    {
                        if (row.Cells[4].Value == null)
                        {
                            MessageBox.Show("Debe completar Tipo de Solicitud para periodo: " + item.periodid.ToString() + " año: " + item.year1.ToString() + " tipo doc: " + item.tipodoc);
                            return;
                        }
                        else
                        {
                            item.TipoSolicitud = row.Cells[4].Value.ToString();

                            string tipoSolicitud = row.Cells[4].Value.ToString().Substring(0, 2);
                            if (tipoSolicitud == "AF" || tipoSolicitud == "FC")
                            {
                                if (row.Cells[5].Value == null || row.Cells[5].Value.ToString() == "")
                                {
                                    MessageBox.Show("Debe completar N. Orden para periodo: " + item.periodid.ToString() + " año: " + item.year1.ToString() + " tipo doc: " + item.tipodoc);
                                    return;
                                }
                                else
                                {
                                    Regex rgx = new Regex(@"^[A-Z]{3}[0-9]{7}(/)[0-9]{2}$");

                                    if (!rgx.IsMatch(row.Cells[5].Value.ToString()))
                                    {
                                        MessageBox.Show("Debe completar N. Orden correctamente para periodo: " + item.periodid.ToString() + " año: " + item.year1.ToString() + " tipo doc: " + item.tipodoc);
                                        return;
                                    }
                                    else
                                    {
                                        item.NumOrden = row.Cells[5].Value.ToString();
                                    }
                                }
                            }
                            else
                            {
                                if (tipoSolicitud == "DE" || tipoSolicitud == "CO")
                                {
                                    if (row.Cells[6].Value == null || row.Cells[6].Value.ToString() == "")
                                    {
                                        MessageBox.Show("Debe completar N. Trámite para periodo: " + item.periodid.ToString() + " año: " + item.year1.ToString() + " tipo doc: " + item.tipodoc);
                                        return;
                                    }
                                    else
                                    {
                                        Regex rgx = new Regex(@"^[A-Z]{2}[0-9]{12}$");

                                        if (!rgx.IsMatch(row.Cells[6].Value.ToString()))
                                        {
                                            MessageBox.Show("Debe completar N. Trámite correctamente para periodo: " + item.periodid.ToString() + " año: " + item.year1.ToString() + " tipo doc: " + item.tipodoc);
                                            return;
                                        }
                                        else
                                        {
                                            item.NumTramite = row.Cells[6].Value.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    l.Add(item);
                }
            }

            try
            {
                List<XmlExportado> xmls = oL.ProcesarArchivos(l, directorio, Application.StartupPath + "\\xsd\\");  //archivo1, archivo2, archivo3, archivo4, archivo5, 

                string errores = "";
                foreach (var xmle in xmls.Where(x => x.error))
                {
                    errores += " Año: " + xmle.DcemVwContabilidad.year1.ToString() + " Mes: " + xmle.DcemVwContabilidad.periodid.ToString() + " Tipo: " + xmle.DcemVwContabilidad.tipodoc + Environment.NewLine + xmle.mensaje + Environment.NewLine;
                    errores += "-----------------------------------------------------------------------" + Environment.NewLine;
                }
                lblError.Text = errores;

                lblProcesos.Text = "Carpeta de trabajo: " + directorio + Environment.NewLine;
                foreach (var xmle in xmls)
                {
                    lblProcesos.Text += " Año: " + xmle.DcemVwContabilidad.year1.ToString() + " Mes:" + xmle.DcemVwContabilidad.periodid.ToString() + " Tipo: " + xmle.DcemVwContabilidad.tipodoc + " Archivo: " + xmle.archivo + Environment.NewLine;
                    lblProcesos.Refresh();
                }

                foreach (DataGridViewRow row in gridVista.Rows)
                {
                    if (row.Cells[0].Value != null && (bool)row.Cells[0].Value)
                    {
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                    }
                }

            }
            catch (Exception ex)
            {
                lblError.Text += ex.Message + Environment.NewLine;
            }


        }

        private void tsBtnMostrarContenido_Click(object sender, EventArgs e)
        {
            if (gridVista.SelectedRows.Count != 0)
            {
                try
                {
                    DcemVwContabilidad item = (DcemVwContabilidad)gridVista.SelectedRows[0].DataBoundItem;
                    if (item != null)
                        mostrarContenido(item.year1, item.periodid, item.tipodoc);
                }
                catch
                {
                    grid.DataSource = null;
                }
            }

        }

        #endregion

        #region IMPORTACION DE FACTURAS
        private void inicializarImportarXML()
        {
            utileria = new UtilitarioArchivos(@"http://www.sat.gob.mx/cfd/3", System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivoXslt"].ToString());

        }

        private void gridFiles_SelectionChanged(object sender, EventArgs e)
        {
            if (gridFiles.SelectedRows.Count != 0)
            {
                var item = gridFiles.SelectedRows[0].DataBoundItem;
                if (item != null)
                {
                    System.Type type = item.GetType();
                    string archivo = (string)type.GetProperty("archivo").GetValue(item, null);
                    string directorio = (string)type.GetProperty("directorio").GetValue(item, null);

                    string text = System.IO.File.ReadAllText(directorio + "\\" + archivo);

                    XNamespace cfdi = @"http://www.sat.gob.mx/cfd/3";
                    XNamespace pago10 = @"http://www.sat.gob.mx/Pagos";
                    XNamespace tfd = @"http://www.sat.gob.mx/TimbreFiscalDigital";

                    try
                    {
                        XDocument xdoc = XDocument.Parse(text);
                        string tipoComprobante = GPCompras.AveriguarElTipoDeCfdi(xdoc, cfdi);
                        switch (tipoComprobante)
                        {
                            case "I":
                                var comprobante = (from c in xdoc.Descendants(cfdi + "Comprobante")
                                                   select new
                                                   {
                                                       Folio = c.Attribute("Folio") == null ? "" : c.Attribute("Folio").Value,
                                                       Fecha = c.Attribute("Fecha").Value,
                                                       FormaDePago = c.Attribute("FormaPago").Value,
                                                       CondicionesDePago = c.Attribute("CondicionesDePago") == null ? "" : c.Attribute("CondicionesDePago").Value,
                                                       SubTotal = c.Attribute("SubTotal").Value,
                                                       TipoCambio = c.Attribute("TipoCambio") == null ? "" : c.Attribute("TipoCambio").Value,
                                                       Moneda = c.Attribute("Moneda") == null ? "" : c.Attribute("Moneda").Value,
                                                       Total = c.Attribute("Total").Value,
                                                       TipoDeComprobante = c.Attribute("TipoDeComprobante").Value,
                                                       MetodoDePago = c.Attribute("MetodoPago").Value,
                                                       LugarExpedicion = c.Attribute("LugarExpedicion").Value
                                                   }).ToList();

                                var emisor = (from c in xdoc.Descendants(cfdi + "Emisor")
                                              select new
                                              {
                                                  Rfc = c.Attribute("Rfc").Value,
                                                  Nombre = c.Attribute("Nombre") == null ? "" : c.Attribute("Nombre").Value,
                                                  Regimen = c.Attribute("RegimenFiscal") == null ? "" : c.Attribute("RegimenFiscal").Value,
                                              }).ToList();

                                var receptor = (from c in xdoc.Descendants(cfdi + "Receptor")
                                                select new
                                                {
                                                    Rfc = c.Attribute("Rfc").Value,
                                                    Nombre = c.Attribute("Nombre") == null ? "" : c.Attribute("Nombre").Value
                                                }).ToList();

                                var concepto = (from c in xdoc.Descendants(cfdi + "Concepto")
                                                select new
                                                {
                                                    Cantidad = c.Attribute("Cantidad").Value,
                                                    Unidad = c.Attribute("ClaveUnidad").Value,
                                                    NoIdentificacion = c.Attribute("NoIdentificacion") == null ? "" : c.Attribute("NoIdentificacion").Value,
                                                    Descripcion = c.Attribute("Descripcion").Value,
                                                    ValorUnitario = c.Attribute("ValorUnitario").Value,
                                                    Importe = c.Attribute("Importe").Value
                                                }).ToList();

                                var retenciones = (from c in xdoc.Descendants(cfdi + "Impuestos").Where(x => x.Attribute("TotalImpuestosRetenidos") != null).Descendants(cfdi + "Retencion")
                                                   select new
                                                   {
                                                       Impuesto = c.Attribute("Impuesto").Value,
                                                       Importe = c.Attribute("Importe").Value
                                                   }).ToList();

                                var traslado = (from c in xdoc.Descendants(cfdi + "Impuestos").Where(x => x.Attribute("TotalImpuestosTrasladados") != null).Descendants(cfdi + "Traslado")
                                                select new
                                                {
                                                    Impuesto = c.Attribute("Impuesto").Value,
                                                    TipoFactor = c.Attribute("TipoFactor").Value,
                                                    Tasa = c.Attribute("TasaOCuota").Value,
                                                    Importe = c.Attribute("Importe").Value
                                                }).ToList();

                                var timbreDigital = (from c in xdoc.Descendants(tfd + "TimbreFiscalDigital")
                                                     select new
                                                     {
                                                         UUID = c.Attribute("UUID").Value
                                                     }).ToList();

                                dataGridView1.DataSource = comprobante;
                                dataGridView2.DataSource = emisor;
                                dataGridView3.DataSource = receptor;
                                dataGridView4.DataSource = concepto;
                                dataGridView5.DataSource = traslado;
                                dataGridView6.DataSource = retenciones;
                                dataGridView7.DataSource = timbreDigital;
                                dataGridView8.DataSource = null;
                                //dataGridView9.DataSource = ;
                                break;
                            case "P":
                                var comprobanteP = (from c in xdoc.Descendants(cfdi + "Comprobante")
                                                   select new
                                                   {
                                                       folio = c.Attribute("Folio") == null ? "" : c.Attribute("Folio").Value,
                                                       fecha = c.Attribute("Fecha").Value,
                                                       tipoDeComprobante = c.Attribute("TipoDeComprobante").Value,
                                                   }).ToList();


                                var emisorP = (from c in xdoc.Descendants(cfdi + "Emisor")
                                          select new
                                          {
                                              Rfc = c.Attribute("Rfc").Value,
                                              Nombre = c.Attribute("Nombre") == null ? "" : c.Attribute("Nombre").Value,
                                              Regimen = c.Attribute("RegimenFiscal") == null ? "" : c.Attribute("RegimenFiscal").Value,
                                          }).ToList();

                                var receptorP = (from c in xdoc.Descendants(cfdi + "Receptor")
                                            select new
                                            {
                                                Rfc = c.Attribute("Rfc").Value,
                                                Nombre = c.Attribute("Nombre") == null ? "" : c.Attribute("Nombre").Value,
                                                UsoCFDI = c.Attribute("UsoCFDI") == null ? "" : c.Attribute("UsoCFDI").Value,
                                            }).ToList();

                                var conceptoP = (from c in xdoc.Descendants(cfdi + "Concepto")
                                            select new
                                            {
                                                Cantidad = c.Attribute("Cantidad").Value,
                                                Unidad = c.Attribute("ClaveUnidad").Value,
                                                Producto = c.Attribute("ClaveProdServ") == null ? "" : c.Attribute("ClaveProdServ").Value,
                                                Descripcion = c.Attribute("Descripcion").Value,
                                            }).ToList();

                                var pagoP = (from c in xdoc.Descendants(pago10 + "Pago")
                                            select new
                                            {
                                                CuentaDelBeneficiario = c.Attribute("CtaBeneficiario") == null ? "" : c.Attribute("CtaBeneficiario").Value,
                                                RFCEmisorCuentaBeneficiario = c.Attribute("RfcEmisorCtaBen") == null ? "" : c.Attribute("RfcEmisorCtaBen").Value,
                                                CuentaDelOrdenante = c.Attribute("CtaOrdenante") == null ? "" : c.Attribute("CtaOrdenante").Value,
                                                RFCEmisorCuentaOrdenante = c.Attribute("RfcEmisorCtaOrd") == null ? "" : c.Attribute("RfcEmisorCtaOrd").Value,
                                                NumeroDeOperacion = c.Attribute("NumOperacion") == null ? "" : c.Attribute("NumOperacion").Value,
                                                Monto = c.Attribute("Monto").Value,
                                                MonedaDelPago = c.Attribute("MonedaP").Value,
                                                FormaDePagoP = c.Attribute("FormaDePagoP").Value,
                                                FechaDelPago = c.Attribute("FechaPago").Value,
                                            }).ToList();

                                var docRelacionadoP = (from c in xdoc.Descendants(pago10 + "DoctoRelacionado")
                                    select new
                                             {
                                                SaldoInsoluto = c.Attribute("ImpSaldoInsoluto") == null ? "" : c.Attribute("ImpSaldoInsoluto").Value,
                                                Pagado = c.Attribute("ImpPagado") == null ? "" : c.Attribute("ImpPagado").Value,
                                                SaldoAnterior = c.Attribute("ImpSaldoAnt") == null ? "" : c.Attribute("ImpSaldoAnt").Value,
                                                Parcialidad = c.Attribute("NumParcialidad").Value,
                                                MetodoDePago = c.Attribute("MetodoDePagoDR").Value,
                                                TipoDeCambio = c.Attribute("TipoCambioDR") == null ? "" : c.Attribute("TipoCambioDR").Value,
                                                Moneda = c.Attribute("MonedaDR").Value,
                                                UUID = c.Attribute("IdDocumento").Value,
                                    }).ToList();

                                var timbreDigitalP = (from c in xdoc.Descendants(tfd + "TimbreFiscalDigital")
                                                 select new
                                                 {
                                                     UUID = c.Attribute("UUID").Value
                                                 }).ToList();

                                dataGridView1.DataSource = comprobanteP;
                                dataGridView2.DataSource = emisorP;
                                dataGridView3.DataSource = receptorP;
                                dataGridView4.DataSource = pagoP;
                                dataGridView5.DataSource = null;
                                dataGridView6.DataSource = null;
                                dataGridView7.DataSource = timbreDigitalP;
                                dataGridView8.DataSource = docRelacionadoP;
                                ////dataGridView9.DataSource = ;

                                break;
                            default:
                                lblError.Text += "Este comprobante no se puede integrar a GP porque no es un comprobante de Ingreso ni de Pago" + Environment.NewLine;
                                lblError.Refresh();
                                break;
                        }
                    }
                    catch (Exception abr)
                    {
                        lblError.Text += "Excepción al abrir el archivo. Es probable que el xml no sea válido. Revise el archivo." + Environment.NewLine + abr.Message;
                        lblError.Refresh();
                    }
                }
            }
        }

        private void oL_ProcesoOkImportarPM(object sender, GPCompras.ProcesoOkImportarPMEventArgs e)
        {
            lblProcesos.Text += e.Archivo + " - Procesado - " + e.Msg + Environment.NewLine;
            lblProcesos.Text += "---------------------------" + Environment.NewLine;
            lblProcesos.Refresh();

            string directorio = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_directorioDestino"].ToString();

            if (!System.IO.Directory.Exists(directorio))
                System.IO.Directory.CreateDirectory(directorio);

            //el archivo quizas ya se movio cuando se exporto la factura, y vuelve a entrar porque se asigno folio
            if (!System.IO.File.Exists(directorio + "\\" + System.IO.Path.GetFileName(e.Archivo)))
                System.IO.File.Move(e.Archivo, directorio + "\\" + System.IO.Path.GetFileName(e.Archivo));
            else
            {
                try
                {
                    System.IO.File.Delete(e.Archivo);
                }
                catch
                {
                }
            }
        }

        private void oL_ErrorImportarPM(object sender, GPCompras.ErrorImportarPMEventArgs e)
        {
            lblError.Text += e.Archivo + ": " + e.Error + Environment.NewLine;
            lblError.Text += "---------------------------" + Environment.NewLine;
            lblError.Refresh();
        }

        private async Task validaArchivosAsync(IProgress<int> prbar, UtilitarioArchivos utileria)
        {
            string archivo = String.Empty;
            string directorio = String.Empty;

            //carga y validar archivos
            int i = 1;
            int max = gridFiles.Rows.Count;
            foreach (DataGridViewRow row in gridFiles.Rows)
            {
                try
                {
                    var item = row.DataBoundItem;
                    if (item != null)
                    {
                        System.Type type = item.GetType();
                        archivo = (string)type.GetProperty("archivo").GetValue(item, null);
                        directorio = (string)type.GetProperty("directorio").GetValue(item, null);

                        var cmp = await utileria.CargarArchivoAsync(directorio + "\\" + archivo);
                        if (cmp.ValidaSelloAsync())
                            row.Cells[1].Style.BackColor = Color.LawnGreen;
                        else
                            row.Cells[1].Style.BackColor = Color.LightGray;

                        if (prbar != null)
                        {
                            prbar.Report(100 * i / max);
                            //lblProcesos.Text += "Validando : " + archivo + Environment.NewLine ;
                        }
                        i++;
                    }
                }
                catch (Exception ex)
                {
                    lblError.Text += "Excepción al leer "+ archivo + " " + ex.Message + Environment.NewLine;
                }
            }
            prbar.Report(0);
        }

        private async void tsButtonSeleccionarArchivo_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblProcesos.Text = "";
            dataGridView1.DataSource = null;
            dataGridView2.DataSource = null;
            dataGridView3.DataSource = null;
            dataGridView4.DataSource = null;
            dataGridView5.DataSource = null;
            dataGridView6.DataSource = null;
            dataGridView7.DataSource = null;
            tsProgressBar.Value = 0;

            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Multiselect = true;
            DialogResult dr = openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string[] filenames = openFileDialog1.FileNames;

                var f = from ff in filenames
                        select new { archivo = System.IO.Path.GetFileName(ff),
                                     directorio = System.IO.Path.GetDirectoryName(ff),
                                     };

                gridFiles.Columns.Clear();

                DataGridViewColumn col = new DataGridViewTextBoxColumn();
                col.DataPropertyName = "archivo";
                col.HeaderText = "Archivo";
                col.Name = "col1";
                gridFiles.Columns.Add(col);

                col = new DataGridViewTextBoxColumn();
                col.DataPropertyName = "selloValido";
                col.HeaderText = "Sello";
                col.Name = "selloValido";
                gridFiles.Columns.Add(col);

                bindingSource3.DataSource = f.ToList();
                gridFiles.AutoGenerateColumns = false;
                gridFiles.DataSource = bindingSource3;
                gridFiles.AutoResizeColumns();
                gridFiles.Refresh();

                IProgress<int> prbar = new Progress<int>(p => tsProgressBar.Value = p);
                await validaArchivosAsync(prbar, utileria);
                lblProcesos.Text += "Validaciones finalizadas." + Environment.NewLine;
                lblProcesos.Text += "Listo para importar a GP."+Environment.NewLine;
            }

        }

        private async void tsButtonImportarArchivos_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblProcesos.Text = "";

            GPCompras gpCompras = new GPCompras(companySelected());

            List<string> archivos = new List<string>();

            foreach (DataGridViewRow row in gridFiles.Rows)
            {
                var item = row.DataBoundItem;
                if (item != null)
                {
                    System.Type type = item.GetType();
                    string archivo = (string)type.GetProperty("archivo").GetValue(item, null);
                    string directorio = (string)type.GetProperty("directorio").GetValue(item, null);

                    archivos.Add(directorio + "\\" + archivo);
                }
            }

            gpCompras.ErrorImportarPM += new EventHandler<GPCompras.ErrorImportarPMEventArgs>(oL_ErrorImportarPM);
            gpCompras.ProcesoOkImportarPM += new EventHandler<GPCompras.ProcesoOkImportarPMEventArgs>(oL_ProcesoOkImportarPM);

            int metodo = 1;
            if (radPOP.Checked)
                metodo = 2;

            //gpCompras.Importar(archivos, metodo);
            await gpCompras.IntegrarDocumentosGPAsync(archivos, metodo);

            if (archivos.Count == 0)
                MessageBox.Show("Debe seleccionar archivos");

            gridFiles.DataSource = null;

        }


        #endregion

        #region Deprecated Importar Facturas Electronicas
        //deprecated
        private void bntSeleccionarArchivos_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblProcesos.Text = "";

            openFileDialog1.Filter = "XML Files|*.xml";
            openFileDialog1.Multiselect = true;
            DialogResult dr = openFileDialog1.ShowDialog();

            if (dr == DialogResult.OK)
            {
                string[] filenames = openFileDialog1.FileNames;

                var f = from ff in filenames
                        select new { archivo = System.IO.Path.GetFileName(ff), directorio = System.IO.Path.GetDirectoryName(ff) };

                gridFiles.Columns.Clear();

                DataGridViewColumn col = new DataGridViewTextBoxColumn();
                col.DataPropertyName = "archivo";
                col.HeaderText = "Archivo";
                col.Name = "col1";
                gridFiles.Columns.Add(col);

                bindingSource3.DataSource = f.ToList();
                gridFiles.AutoGenerateColumns = false;
                gridFiles.DataSource = bindingSource3;
                gridFiles.AutoResizeColumns();
            }
        }

        /// <summary>
        /// deprecated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProcesarFacturas_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblProcesos.Text = "";

            LecturaContabilidadFactory oL = new LecturaContabilidadFactory(companySelected());

            List<string> archivos = new List<string>();

            foreach (DataGridViewRow row in gridFiles.Rows)
            {
                var item = row.DataBoundItem;
                if (item != null)
                {
                    System.Type type = item.GetType();
                    string archivo = (string)type.GetProperty("archivo").GetValue(item, null);
                    string directorio = (string)type.GetProperty("directorio").GetValue(item, null);

                    archivos.Add(directorio + "\\" + archivo);
                }
            }

            //oL.ErrorImportarPM += new EventHandler<LecturaContabilidadFactory.ErrorImportarPMEventArgs>(oL_ErrorImportarPM);
            //oL.ProcesoOkImportarPM += new EventHandler<LecturaContabilidadFactory.ProcesoOkImportarPMEventArgs>(oL_ProcesoOkImportarPM);

            //int metodo = 1;
            //if (radPOP.Checked)
            //    metodo = 2;

            //oL.ImportarGPPM(archivos, metodo);

            //if (archivos.Count == 0)
            //    MessageBox.Show("Debe seleccionar archivos");

            gridFiles.DataSource = null;
        }

        #endregion

        #region Deprecated Conta Electrónica
        //deprecated
        private void btnMostrarContenido_Click(object sender, EventArgs e)
        {
            if (gridVista.SelectedRows.Count != 0)
            {
                try
                {
                    DcemVwContabilidad item = (DcemVwContabilidad)gridVista.SelectedRows[0].DataBoundItem;
                    if (item != null)
                        mostrarContenido(item.year1, item.periodid, item.tipodoc);
                }
                catch
                {
                    grid.DataSource = null;
                }
            }

        }
        //deprecated
        private void btnProcesar_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblProcesos.Text = "";

            LecturaContabilidadFactory oL = new LecturaContabilidadFactory(companySelected());
            oL.LParametros = lParametros;

            string directorio = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_directorio"].ToString();
            //string archivo1 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo1"].ToString();
            //string archivo2 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo2"].ToString();
            //string archivo3 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo3"].ToString();
            //string archivo4 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo4"].ToString();
            //string archivo5 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo5"].ToString();

            List<DcemVwContabilidad> l = new List<DcemVwContabilidad>();

            foreach (DataGridViewRow row in gridVista.Rows)
            {
                if (row.Cells[0].Value != null && (bool)row.Cells[0].Value)
                {
                    var item = (DcemVwContabilidad)row.DataBoundItem;


                    if (item.tipodoc == "Pólizas" || item.tipodoc == "Auxiliar Cuentas" || item.tipodoc == "Auxiliar folios")
                    {
                        if (row.Cells[4].Value == null)
                        {
                            MessageBox.Show("Debe completar Tipo de Solicitud para periodo: " + item.periodid.ToString() + " año: " + item.year1.ToString() + " tipo doc: " + item.tipodoc);
                            return;
                        }
                        else
                        {
                            item.TipoSolicitud = row.Cells[4].Value.ToString();

                            string tipoSolicitud = row.Cells[4].Value.ToString().Substring(0, 2);
                            if (tipoSolicitud == "AF" || tipoSolicitud == "FC")
                            {
                                if (row.Cells[5].Value == null || row.Cells[5].Value.ToString() == "")
                                {
                                    MessageBox.Show("Debe completar N. Orden para periodo: " + item.periodid.ToString() + " año: " + item.year1.ToString() + " tipo doc: " + item.tipodoc);
                                    return;
                                }
                                else
                                {
                                    //Regex rgx = new Regex(@"^[A-Z]{3}[0-6][0-9][0-9]{5}(/)[0-9]{2}$");
                                    Regex rgx = new Regex(@"^[A-Z]{3}[0-9]{7}(/)[0-9]{2}$");

                                    if (!rgx.IsMatch(row.Cells[5].Value.ToString()))
                                    {
                                        MessageBox.Show("Debe completar N. Orden correctamente para periodo: " + item.periodid.ToString() + " año: " + item.year1.ToString() + " tipo doc: " + item.tipodoc);
                                        return;
                                    }
                                    else
                                    {
                                        item.NumOrden = row.Cells[5].Value.ToString();
                                    }
                                }
                            }
                            else
                            {
                                if (tipoSolicitud == "DE" || tipoSolicitud == "CO")
                                {
                                    if (row.Cells[6].Value == null || row.Cells[6].Value.ToString() == "")
                                    {
                                        MessageBox.Show("Debe completar N. Trámite para periodo: " + item.periodid.ToString() + " año: " + item.year1.ToString() + " tipo doc: " + item.tipodoc);
                                        return;
                                    }
                                    else
                                    {
                                        //Regex rgx = new Regex(@"^[0-9]{10}$");
                                        Regex rgx = new Regex(@"^[A-Z]{2}[0-9]{12}$");

                                        if (!rgx.IsMatch(row.Cells[6].Value.ToString()))
                                        {
                                            MessageBox.Show("Debe completar N. Trámite correctamente para periodo: " + item.periodid.ToString() + " año: " + item.year1.ToString() + " tipo doc: " + item.tipodoc);
                                            return;
                                        }
                                        else
                                        {
                                            item.NumTramite = row.Cells[6].Value.ToString();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    l.Add(item);
                }
            }

            try
            {
                List<XmlExportado> xmls = oL.ProcesarArchivos(l, directorio, Application.StartupPath + "\\xsd\\");  //archivo1, archivo2, archivo3, archivo4, archivo5, 

                string errores = "";
                foreach (var xmle in xmls.Where(x => x.error))
                {
                    errores += " Año: " + xmle.DcemVwContabilidad.year1.ToString() + " Mes: " + xmle.DcemVwContabilidad.periodid.ToString() + " Tipo: " + xmle.DcemVwContabilidad.tipodoc + Environment.NewLine + xmle.mensaje + Environment.NewLine;
                    errores += "-----------------------------------------------------------------------" + Environment.NewLine;
                }
                lblError.Text = errores;

                lblProcesos.Text = "Carpeta de trabajo: " + directorio + Environment.NewLine;
                foreach (var xmle in xmls)
                {
                    lblProcesos.Text += " Año: " + xmle.DcemVwContabilidad.year1.ToString() + "Mes:" + xmle.DcemVwContabilidad.periodid.ToString() + " Tipo: " + xmle.DcemVwContabilidad.tipodoc + " Archivo: " + xmle.archivo + Environment.NewLine;
                    lblProcesos.Refresh();
                }

                foreach (DataGridViewRow row in gridVista.Rows)
                {
                    if (row.Cells[0].Value != null && (bool)row.Cells[0].Value)
                    {
                        row.DefaultCellStyle.BackColor = Color.Yellow;
                    }
                }

            }
            catch (Exception ex)
            {
                lblError.Text += ex.Message + Environment.NewLine;
            }


        }

        #endregion

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (gridVista.SelectedRows.Count != 0)
            {
                try
                {
                    string carpeta = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_directorio"].ToString();
                    DcemVwContabilidad item = (DcemVwContabilidad)gridVista.SelectedRows[0].DataBoundItem;
                    if (item != null)
                    {
                        var iTipoDoc = lParametros.Where(x => x.Tipo == item.tipodoc);
                        string nombreArchivo = iTipoDoc.First().Archivo;
                        nombreArchivo = Path.GetFileNameWithoutExtension(nombreArchivo) + "_" + DateTime.Now.ToString("yyyyMMdd HHmmss");
                        string archivo = Path.Combine(carpeta, nombreArchivo + ".xlsx");

                        object items = mostrarContenido(item.year1, item.periodid, item.tipodoc);
                        DataTable dtItems = ContabilidadElectronicaPresentacion.ConvierteLinqQueryADataTable((IEnumerable<dynamic>)items);
                        var wb = new ClosedXML.Excel.XLWorkbook();

                        dtItems.TableName = "test";
                        wb.Worksheets.Add(dtItems);
                        //wb.Worksheet(1).Cell("B1").Value = "0";
                        
                        //wb.Worksheet(1).Column(2).CellsUsed().SetDataType(XLDataType.Number);

                        //wb.Worksheet(1).Cell("B1").Value = "Saldo Inicial";

                        wb.SaveAs(archivo);
                        UtilitarioArchivos.AbrirArchivo(archivo);
                        lblProcesos.Text = "Archivo guardado en: " + archivo;

                    }
                }
                catch(Exception exl)
                {

                    grid.DataSource = null;
                    lblError.Text = exl.Message;

                }
            }

        }

        private void gridVista_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0) gridVista.EndEdit();

        }
    }
}
