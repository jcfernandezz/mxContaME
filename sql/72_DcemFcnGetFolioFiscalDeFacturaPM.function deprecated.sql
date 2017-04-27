
IF OBJECT_ID ('dbo.DcemFcnGetFolioFiscalDeFacturaPM') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnGetFolioFiscalDeFacturaPM]
GO

create function [dbo].[DcemFcnGetFolioFiscalDeFacturaPM]
(
@ORCTRNUM varchar(21) 
,@ORTRXTYP smallint 
) returns table
as
--Propósito. Obtiene folio fiscal dada una factura
--12/2014 jmg Creación
--
return ( 
		select rtrim(MexFolioFiscal) MexFolioFiscal
          from ACA_IETU00400 
         where DOCTYPE = @ORTRXTYP 
           and VCHRNMBR = @ORCTRNUM
          ) 

--sp_columns ACA_IETU00400
GO

IF (@@Error = 0) PRINT 'Creación exitosa de la función: [DcemFcnGetFolioFiscalDeFacturaPM]()'
ELSE PRINT 'Error en la creación de la función: [DcemFcnGetFolioFiscalDeFacturaPM]()'
GO
