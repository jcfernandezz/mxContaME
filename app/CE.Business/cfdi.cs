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
    public class Cfdi
    {
        String _archivoYCarpeta = String.Empty;
        bool _valida = false;
        String _sxml = String.Empty;
        TransformerXML _plantillaXslt = null;
        XNamespace _nscfdi;

        public string ArchivoYCarpeta
        {
            get
            {
                return _archivoYCarpeta;
            }

            set
            {
                _archivoYCarpeta = value;
            }
        }

        public bool Valida
        {
            get
            {
                return _valida;
            }

            set
            {
                _valida = value;
            }
        }

        public string Sxml
        {
            get
            {
                return _sxml;
            }

            set
            {
                _sxml = value;
            }
        }

        public Cfdi(String nameSpace, TransformerXML plantillaXslt)
        {
            _nscfdi = nameSpace;
            _plantillaXslt = plantillaXslt;
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

        public bool ValidaSelloAsync()
        {

            XmlDocument comprobanteXml = new XmlDocument();
            comprobanteXml.LoadXml(_sxml);
            _plantillaXslt.getCadenaOriginal(comprobanteXml);

            XDocument xdoc = XDocument.Parse(_sxml);
            var comprobante = xdoc.Descendants(_nscfdi + "Comprobante")
                                .Select(c => new
                                {
                                    Sello = c.Attribute("Sello") == null ? "" : c.Attribute("Sello").Value,
                                    Certificado = c.Attribute("Certificado") == null ? "" : c.Attribute("Certificado").Value
                                }).First();

            return (VerificaIntegridadDeDatosSellados(_plantillaXslt.cadenaOriginal, comprobante.Sello, comprobante.Certificado));

        }

    }
}
