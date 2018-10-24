IF EXISTS (
  SELECT * 
    FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = 'dace'
     AND SPECIFIC_NAME = 'spComprobanteCFDIRelacionadoIns' 
)
   DROP PROCEDURE dace.spComprobanteCFDIRelacionadoIns;
GO

CREATE PROCEDURE dace.spComprobanteCFDIRelacionadoIns@CFDI_TIPOCOMPROBANTE     varchar(1) = NULL,@CFDI_UUID                varchar(50) = NULL,@comprobanteXmlString	NVARCHAR(MAX)AS
	declare @comprobanteXml xml = @comprobanteXmlString;
 
	INSERT INTO dace.ComprobanteCFDIRelacionado							(CFDI_TIPOCOMPROBANTE,CFDI_UUID,UUIDRELACIONADO)				SELECT  @CFDI_TIPOCOMPROBANTE,  @CFDI_UUID,    node.value('.', 'varchar(50)')
				FROM    @comprobanteXml.nodes('/*:Comprobante/*:Complemento/*:Pagos/*:Pago/*:DoctoRelacionado/@IdDocumento') t(node)
 
GO

-----------------------------------------------------------------------------------
IF EXISTS (
  SELECT * 
    FROM INFORMATION_SCHEMA.ROUTINES 
   WHERE SPECIFIC_SCHEMA = 'dace'
     AND SPECIFIC_NAME = 'spComprobanteCFDIInsDel' 
)
   DROP PROCEDURE dace.spComprobanteCFDIInsDel;
GO
--Propósito. Ingresa los datos del comprobante de compra xml de ingreso o pago--24/10/18 JCF Creación--CREATE PROCEDURE dace.spComprobanteCFDIInsDel@UUID                varchar(50),@TIPOCOMPROBANTE     varchar(1),@FOLIO               varchar(35) = NULL,@FECHA               varchar(20) = NULL,@TOTAL               numeric(19,5) = NULL,@MONEDA              varchar(15) = NULL,@METODOPAGO          varchar(5) = NULL,@EMISOR_RFC          varchar(30) = NULL,@RESUMENCFDI         varchar(100) = NULL,@NOMBREARCHIVO       varchar(255) = NULL,@CARPETAARCHIVO      varchar(255) = NULL,@comprobanteXml		 NVARCHAR(MAX)AS

IF EXISTS (SELECT 1 FROM dace.ComprobanteCFDI			WHERE UUID = @UUID			   AND TIPOCOMPROBANTE = @TIPOCOMPROBANTE )
BEGIN
 
	delete from dace.ComprobanteCFDI	 WHERE UUID = @UUID	AND TIPOCOMPROBANTE = @TIPOCOMPROBANTEEND
ELSE
BEGIN
	INSERT INTO dace.ComprobanteCFDI (UUID,TIPOCOMPROBANTE,FOLIO,FECHA,TOTAL,MONEDA,METODOPAGO,EMISOR_RFC,RESUMENCFDI,NOMBREARCHIVO,CARPETAARCHIVO)							SELECT @UUID,@TIPOCOMPROBANTE,@FOLIO,@FECHA,@TOTAL,@MONEDA,@METODOPAGO,@EMISOR_RFC,@RESUMENCFDI,@NOMBREARCHIVO,@CARPETAARCHIVO
 
	if (@TIPOCOMPROBANTE = 'P')	--pago
		exec dace.spComprobanteCFDIRelacionadoIns @TIPOCOMPROBANTE, @UUID, @comprobanteXml
END
 
GO
