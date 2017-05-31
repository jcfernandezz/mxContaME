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

namespace CE.WinFormUI
{
    public partial class Form1 : Form
    {
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

        #region ExportarXML
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

            XDocument xdoc = XDocument.Parse(oL.GetXML2(year1, periodid, tipo));

            object items = null;

            XNamespace cfdi = "";

            switch (tipo)
            {
                case "Catálogo":
                    cfdi = @"www.sat.gob.mx/esquemas/ContabilidadE/1_1/CatalogoCuentas";
                    items = from l in xdoc.Descendants(cfdi + "Ctas")
                            select new
                            {
                                NumCta = l.Attribute("NumCta").Value,
                                Desc = l.Attribute("Desc").Value,
                                CodAgrup = l.Attribute("CodAgrup").Value
                            };
                    break;
                case "Balanza":
                    cfdi = @"www.sat.gob.mx/esquemas/ContabilidadE/1_1/BalanzaComprobacion";
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
                    cfdi = @"www.sat.gob.mx/esquemas/ContabilidadE/1_1/PolizasPeriodo";
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
                    cfdi = @"www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarCtas";
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
                    cfdi = @"www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarFolios";
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

        private void btnProcesar_Click(object sender, EventArgs e)
        {
            lblError.Text = "";
            lblProcesos.Text = "";

            LecturaContabilidadFactory oL = new LecturaContabilidadFactory(companySelected());

            string directorio = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_directorio"].ToString();
            string archivo1 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo1"].ToString();
            string archivo2 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo2"].ToString();
            string archivo3 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo3"].ToString();
            string archivo4 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo4"].ToString();
            string archivo5 = System.Configuration.ConfigurationManager.AppSettings[companySelected() + "_archivo5"].ToString();

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
                            MessageBox.Show("Debe completar Tipo de Solicitud para periodo: "+ item.periodid.ToString() + " año: "+ item.year1.ToString() + " tipo doc: "+ item.tipodoc);
                            return;
                        }
                        else
                        {
                            item.TipoSolicitud = row.Cells[4].Value.ToString();

                            string tipoSolicitud = row.Cells[4].Value.ToString().Substring(0,2);
                            if (tipoSolicitud == "AF" || tipoSolicitud == "FC")
                            {
                                if (row.Cells[5].Value == null || row.Cells[5].Value.ToString() == "")
                                {
                                    MessageBox.Show("Debe completar N. Orden para periodo: " + item.periodid.ToString() + " año: " + item.year1.ToString() + " tipo doc: " + item.tipodoc);
                                    return;
                                }
                                else
                                {
                                    Regex rgx = new Regex(@"^[A-Z]{3}[0-6][0-9][0-9]{5}(/)[0-9]{2}$");
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
                                        Regex rgx = new Regex(@"^[0-9]{10}$");
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
                List<XmlExportado> xmls = oL.SaveFiles(l, directorio, archivo1, archivo2, archivo3, archivo4, archivo5, Application.StartupPath + "\\xsd\\");

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
        #endregion

        #region Importar Facturas Electronicas
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
                                               LugarExpedicion = c.Attribute("LugarExpedicion").Value
                                           }).ToList();

                        var emisor = (from c in xdoc.Descendants(cfdi + "Emisor")
                                      select new
                                      {
                                          rfc = c.Attribute("rfc").Value,
                                          nombre = c.Attribute("nombre") == null ? "" : c.Attribute("nombre").Value
                                      }).ToList();

                        var emisorDomicilio = (from c in xdoc.Descendants(cfdi + "DomicilioFiscal")
                                               select new
                                               {
                                                   calle = c.Attribute("calle").Value,
                                                   noExterior = c.Attribute("noExterior") == null ? "" : c.Attribute("noExterior").Value,
                                                   noInterior = c.Attribute("noInterior") == null ? "" : c.Attribute("noInterior").Value,
                                                   colonia = c.Attribute("colonia") == null ? "" : c.Attribute("colonia").Value,
                                                   localidad = c.Attribute("localidad") == null ? "" : c.Attribute("localidad").Value,
                                                   municipio = c.Attribute("municipio") == null ? "" : c.Attribute("municipio").Value,
                                                   estado = c.Attribute("estado").Value,
                                                   pais = c.Attribute("pais").Value,
                                                   codigoPostal = c.Attribute("codigoPostal") == null ? "" : c.Attribute("codigoPostal").Value,
                                               }).ToList();

                        var emisorRegimen = (from c in xdoc.Descendants(cfdi + "RegimenFiscal")
                                             select new
                                             {
                                                 Regimen = c.Attribute("Regimen").Value
                                             }).ToList();

                        var receptor = (from c in xdoc.Descendants(cfdi + "Receptor")
                                        select new
                                        {
                                            nombre = c.Attribute("nombre") == null ? "" : c.Attribute("nombre").Value
                                        }).ToList();

                        var receptorDomicilio = (from c in xdoc.Descendants(cfdi + "Domicilio")
                                                 select new
                                                 {
                                                     calle = c.Attribute("calle") == null ? "" : c.Attribute("calle").Value,
                                                     noExterior = c.Attribute("noExterior") == null ? "" : c.Attribute("noExterior").Value,
                                                     noInterior = c.Attribute("noInterior") == null ? "" : c.Attribute("noInterior").Value,
                                                     colonia = c.Attribute("colonia") == null ? "" : c.Attribute("colonia").Value,
                                                     localidad = c.Attribute("localidad") == null ? "" : c.Attribute("localidad").Value,
                                                     municipio = c.Attribute("municipio") == null ? "" : c.Attribute("municipio").Value,
                                                     estado = c.Attribute("estado") == null ? "" : c.Attribute("estado").Value,
                                                     pais = c.Attribute("pais") == null ? "" : c.Attribute("pais").Value,
                                                     codigoPostal = c.Attribute("codigoPostal") == null ? "" : c.Attribute("codigoPostal").Value,
                                                 }).ToList();

                        var concepto = (from c in xdoc.Descendants(cfdi + "Concepto")
                                        select new
                                        {
                                            cantidad = c.Attribute("cantidad").Value,
                                            unidad = c.Attribute("unidad").Value,
                                            noIdentificacion = c.Attribute("noIdentificacion") == null ? "" : c.Attribute("noIdentificacion").Value,
                                            descripcion = c.Attribute("descripcion").Value,
                                            valorUnitario = c.Attribute("valorUnitario").Value,
                                            importe = c.Attribute("importe").Value
                                        }).ToList();

                        var translado = (from c in xdoc.Descendants(cfdi + "Traslado")
                                         select new
                                         {
                                             impuesto = c.Attribute("impuesto").Value,
                                             tasa = c.Attribute("tasa").Value,
                                             importe = c.Attribute("importe").Value
                                         }).ToList();

                        var timbreDigital = (from c in xdoc.Descendants(tfd + "TimbreFiscalDigital")
                                             select new
                                             {
                                                 UUID = c.Attribute("UUID").Value
                                             }).ToList();

                        dataGridView1.DataSource = comprobante;
                        dataGridView2.DataSource = emisor;
                        dataGridView3.DataSource = emisorDomicilio;
                        dataGridView4.DataSource = emisorRegimen;
                        dataGridView5.DataSource = receptor;
                        dataGridView6.DataSource = receptorDomicilio;
                        dataGridView7.DataSource = concepto;
                        dataGridView8.DataSource = translado;
                        dataGridView9.DataSource = timbreDigital;
                    }
                    catch (Exception abr)
                    {
                        lblError.Text += "Excepción al abrir el archivo. Es probable que el xml no sea válido. Revise el archivo." + Environment.NewLine;
                        lblError.Refresh();
                    }
                }
            }
        }

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

            oL.ErrorImportarPM += new EventHandler<LecturaContabilidadFactory.ErrorImportarPMEventArgs>(oL_ErrorImportarPM);
            oL.ProcesoOkImportarPM += new EventHandler<LecturaContabilidadFactory.ProcesoOkImportarPMEventArgs>(oL_ProcesoOkImportarPM);
            
            int metodo = 1;
            if (radPOP.Checked)
                metodo = 2;

            oL.ImportarGPPM(archivos, metodo);

            if (archivos.Count == 0)
                MessageBox.Show("Debe seleccionar archivos");

            gridFiles.DataSource = null;
        }

        private void oL_ProcesoOkImportarPM(object sender, LecturaContabilidadFactory.ProcesoOkImportarPMEventArgs e)
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

        private void oL_ErrorImportarPM(object sender, LecturaContabilidadFactory.ErrorImportarPMEventArgs e)
        {
            lblError.Text += e.Archivo + ": " + e.Error + Environment.NewLine;
            lblError.Text += "---------------------------" + Environment.NewLine;
            lblError.Refresh();
        }
        #endregion

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
    }
}
