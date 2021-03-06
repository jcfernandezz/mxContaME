
IF OBJECT_ID ('dbo.DcemFcnGetFolioFiscalDeFacturaSOP') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnGetFolioFiscalDeFacturaSOP]
GO

create function [dbo].[DcemFcnGetFolioFiscalDeFacturaSOP]
(
@ORCTRNUM varchar(21) 
,@ORTRXTYP smallint 
) returns table
as
--Propósito. Obtiene folio fiscal de factura SOP
--12/2014 jmg Creación
--7/4/15 jcf Agrega rfc, docid
--
return ( 
   select rtrim(foliofiscal) foliofiscal,
          rtrim(rfcReceptor) RFC,
          docid 
     from dbo.vwCfdDocumentosAImprimir 
	 where soptype = @ORTRXTYP 
		and sopnumbe = @ORCTRNUM
          ) 
GO

IF (@@Error = 0) PRINT 'Creación exitosa de la función: [DcemFcnGetFolioFiscalDeFacturaSOP]()'
ELSE PRINT 'Error en la creación de la función: [DcemFcnGetFolioFiscalDeFacturaSOP]()'
GO
