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
        private List<ParametrosDeArchivo> _lParametros = new List<ParametrosDeArchivo>();

        public List<ParametrosDeArchivo> LParametros
        {
            get
            {
                return _lParametros;
            }

            set
            {
                _lParametros = value;
            }
        }

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
        /// <param name="sp">Nombre del stored procedure</param>
        public void corregirDocsConError(string sp)
        {
            if (!sp.Equals(""))
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 3600;
                    cmd.CommandText = sp;
                    cmd.ExecuteNonQuery();
                }

        }
        /// <summary>
        /// Ejecuta un sp que corrige los docs marcados con error
        /// </summary>
        /// <param name="tipo"></param>
        public void marcarDocsConError(string sp, int year)
        {
            if (!sp.Equals(""))
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = sp;
                    SqlParameter param = new SqlParameter();
                    cmd.Parameters.Add(param);

                    foreach (string je in l_ErroresValidarXml)
                    {
                        param.ParameterName = "@jrnentry";
                        param.Value = je;
                        cmd.ExecuteNonQuery();
                    }
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
            var iTipoDoc = _lParametros.Where(x => x.Tipo == tipo);
            string tabla = iTipoDoc.First().FuncionSql;

            //switch (tipo)
            //{
            //    case "Auxiliar Cuentas":
            //        tabla = "DCEMFCNAUXILIARCTAS";
            //        break;
            //    case "Auxiliar folios":
            //        tabla = "DCEMFCNAUXILIARFOLIOS";
            //        break;
            //    case "Balanza":
            //        tabla = "DCEMFCNBALANCE";
            //        break;
            //    case "Catálogo":
            //        tabla = "DCEMFCNCATALOGOXML";
            //        break;
            //    case "Pólizas":
            //        tabla = "DCEMFCNPOLIZAS";
            //        break;
            //}

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

        void validarUnaPolizaPorVez(string docXml, string esquemaXml, string nameSpace)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            schemas.Add(null, esquemaXml);
            XNamespace ns = nameSpace;  // "www.sat.gob.mx/esquemas/ContabilidadE/1_1/PolizasPeriodo";

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
        public List<XmlExportado> ProcesarArchivos(List<DcemVwContabilidad> items, string directorio, string directorioXSD)
                //string archivo1, string archivo2, string archivo3, string archivo4, string archivo5, 
        {
            string archivo = "";
            
            List<XmlExportado> xmls = new List<XmlExportado>();

            if (!System.IO.Directory.Exists(directorio))
                System.IO.Directory.CreateDirectory(directorio);

            foreach (var item in items)
            {
                string archivoXSD = directorioXSD;
                ErroresValidarXml = "";

                var iTipoDoc = _lParametros.Where(x => x.Tipo == item.tipodoc);
                archivo = iTipoDoc.First().Archivo;
                archivoXSD += iTipoDoc.First().Esquema;
                if (item.tipodoc.Equals("Pólizas"))
                {
                    this.corregirDocsConError("dcem.dcemCorrigePoliza");
                }

                int version = GetVersionXML(item);
                archivo = System.IO.Path.GetFileNameWithoutExtension(archivo) + "_" + version + System.IO.Path.GetExtension(archivo);

                string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
                xml += Environment.NewLine;
                
                item.catalogo = this.GetXML2(item.year1, item.periodid, item.tipodoc);

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

                //Detecta y marca pólizas con error
                if (item.tipodoc.Equals("Pólizas"))
                {
                    validarUnaPolizaPorVez(item.catalogo, archivoXSD, iTipoDoc.First().NameSpace);
                    marcarDocsConError("dcem.dcemMarcarPolizasConError", item.year1);
                }
                else
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

    }
}
