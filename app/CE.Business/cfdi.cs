using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Xml.Linq;
using System.Xml;

namespace CE.Business
{
    public class cfdi
    {
        XNamespace ns_cfdi;
        TransformerXML plantillaXslt;
        public cfdi(String nameSpace, String rutaXslt)
        {
            ns_cfdi = nameSpace;
            plantillaXslt = new TransformerXML(rutaXslt);
        }

        /// <summary>
        /// Verificación de de la integridad del comprobante
        /// </summary>
        /// <param name="cadenaOriginal"></param>
        /// <param name="firmaCodificadaEnBase64"></param>
        /// <param name="pemCertificate"></param>
        /// <returns></returns>
        public bool VerificaIntegridadDeDatosSellados(String cadenaOriginal, String firmaCodificadaEnBase64, String pemCertificate)
        {
            bool verificado = false;

            byte[] DataToVerify = Encoding.UTF8.GetBytes(cadenaOriginal);

            byte[] SignedData = System.Convert.FromBase64String(firmaCodificadaEnBase64);
            byte[] rawCertificate = System.Convert.FromBase64String(pemCertificate);

            X509Certificate2 certificate = new X509Certificate2(rawCertificate);
            RSACryptoServiceProvider rsaprovider = (RSACryptoServiceProvider)certificate.PublicKey.Key;

            if (rsaprovider.VerifyData(DataToVerify, "SHA256", SignedData))
            {
                verificado = true;
            }
            return verificado;
        }

        public bool ValidarSello(String archivo)
        {
            
            if (System.IO.File.Exists(archivo))
            {
                string xml = System.IO.File.ReadAllText(archivo);

                XmlDocument comprobanteXml = new XmlDocument();
                comprobanteXml.LoadXml(xml);
                plantillaXslt.getCadenaOriginal(comprobanteXml);

                XDocument xdoc = XDocument.Parse(xml);
                //plantillaXslt.TransformXDocument(xdoc);
                var comprobante = xdoc.Descendants(ns_cfdi + "Comprobante")
                                    .Select(c => new
                                    {
                                        Sello = c.Attribute("Sello") == null ? "" : c.Attribute("Sello").Value,
                                        Certificado = c.Attribute("Certificado") == null ? "" : c.Attribute("Certificado").Value
                                    }).First();

                return VerificaIntegridadDeDatosSellados(plantillaXslt.cadenaOriginal, comprobante.Sello, comprobante.Certificado);
            }
            else
                return false;
        }
    }
}
