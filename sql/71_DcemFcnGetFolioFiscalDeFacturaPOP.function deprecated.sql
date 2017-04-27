
IF OBJECT_ID ('dbo.DcemFcnGetFolioFiscalDeFacturaPOP') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnGetFolioFiscalDeFacturaPOP]
GO

create function [dbo].[DcemFcnGetFolioFiscalDeFacturaPOP]
(
@ORCTRNUM varchar(21) 
,@ORTRXTYP smallint 
) returns table

as
--Propósito. Obtiene folio fiscal en caso de factura POP
--12/2014 jmg Creación
--7/4/15 jcf Agrega txrgnnum
--
return
(
 select 
	 --'' '@UUID_CFDI',
	 rtrim(ac.MexFolioFiscal) MexFolioFiscal,
	 pt.txrgnnum
 from vwPopRecepcionesHdr pr    
	left join vwPmTransaccionesTodas pt 
	on pr.VCHRNMBR = pt.VCHRNMBR
	and pt.DOCTYPE = 1 
	left join ACA_IETU00400 ac    
	on pt.DOCTYPE = ac.DOCTYPE     
	and pt.VCHRNMBR = ac.VCHRNMBR
 where pr.POPRCTNM = @ORCTRNUM
 )
GO

IF (@@Error = 0) PRINT 'Creación exitosa de la función: [DcemFcnGetFolioFiscalDeFacturaPOP]()'
ELSE PRINT 'Error en la creación de la función: [DcemFcnGetFolioFiscalDeFacturaPOP]()'
GO
