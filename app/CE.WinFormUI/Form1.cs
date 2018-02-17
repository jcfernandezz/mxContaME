﻿using System;
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

namespace CE.WinFormUI
{
    public partial class Form1 : Form
    {
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

            inicializarExportarXML();
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

            //gridVista.Columns[4].Visible = false;
            
            gridVista.AutoResizeColumns();
            gridVista.RowHeadersVisible = false;
        }

        private void mostrarContenido(Int16 year1, Int16 periodid, string tipo)
        {
            LecturaContabilidadFactory oL = new LecturaContabilidadFactory(companySelected());
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

            bindingSource1.DataSource = items;
            grid.AutoGenerateColumns = true;
            grid.DataSource = bindingSource1;
            grid.RowHeadersVisible = false;
            grid.AutoResizeColumns();
        }

        private void mostrarMensaje()
        {
            XDocument xdoc = XDocument.Parse("<Mensajes><Mensaje texto = \"Para ver el contenido presione el botón Mostrar. ¡Atención! Esto puede demorar varios minutos dependiendo de la cantidad de datos a mostrar.\" /></Mensajes>");
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
                    dgv.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;
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
                    XNamespace tfd = @"http://www.sat.gob.mx/TimbreFiscalDigital";

                    try
                    {

                        XDocument xdoc = XDocument.Parse(text);

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

                        var traslado = (from c in xdoc.Descendants(cfdi + "Impuestos").Where(x => x.Attribute("TotalImpuestosTrasladados")!= null).Descendants(cfdi + "Traslado")
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
                        //dataGridView8.DataSource = ;
                        //dataGridView9.DataSource = ;
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

        private void tsButtonSeleccionarArchivo_Click(object sender, EventArgs e)
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


                //Cambia de color si validación de integridad es incorrecto
                cfdi comprobanteCfdi = new cfdi(@"http://www.sat.gob.mx/cfd/3", System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivoXslt"].ToString());
                foreach (DataGridViewRow row in gridFiles.Rows)
                {

                    try
                    {
                        var item = row.DataBoundItem;
                        if (item != null)
                        {
                            System.Type type = item.GetType();
                            string archivo = (string)type.GetProperty("archivo").GetValue(item, null);
                            string directorio = (string)type.GetProperty("directorio").GetValue(item, null);

                            if (comprobanteCfdi.ValidarSello(directorio + "\\" + archivo))
                                row.Cells[1].Style.BackColor = Color.Green;
                        }

                    }
                    catch (Exception ex)
                    {
                        lblError.Text += "Validación de integridad. " + ex.Message + Environment.NewLine;
                    }
                }

            }

        }

        private void tsButtonImportarArchivos_Click(object sender, EventArgs e)
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

            gpCompras.Importar(archivos, metodo);

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

        private void label7_Click(object sender, EventArgs e)
        {

        }
    }
}
