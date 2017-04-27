-------------------------------------------------------------------------------------------------
IF OBJECT_ID ('dbo.DcemFcnDocNacFolio') IS NOT NULL
   DROP FUNCTION dbo.DcemFcnDocNacFolio
GO

create function [dbo].[DcemFcnDocNacFolio]
(
@ORCTRNUM  varchar(21), 
@ORTRXTYP  smallint, 
@SOURCDOC  VARCHAR(11),
@ORMSTRID  varchar(31),
@SERIES    smallint
)
returns xml 
as
--Propósito. Facturas y pagos Nacionales
--02/2015 jmg Creación
--7/4/15 jcf Replanteo de consulta. Corrección de parámetros.
--	
begin
declare @cncp xml;
WITH XMLNAMESPACES
  ('www.sat.gob.mx/esquemas/ContabilidadE/1_1/AuxiliarFolios' as "RepAux")
select @cncp = 
(
		select 
			rtrim(left(docu.MexFolioFiscal, 36)) '@UUID_CFDI',
			cast(docu.docamnt as numeric(16,2))  '@MontoTotal',
			dbo.DcemFcnReplace(docu.txrgnnum)    '@RFC',
			rtrim(docu.codMetodoPago)			'@MetPagoAux',
			rtrim(docu.ISOCURRC)				 '@Moneda',
			cast(docu.xchgrate as numeric(19,5)) '@TipCamb'
		from dbo.dcemFnGetDocumentoOriginal(@ORCTRNUM, @ORTRXTYP, @SOURCDOC) docu
		WHERE docu.ccode in ('MX', '')
			AND @SERIES in (3, 4)
			and docu.MexFolioFiscal is not null
		FOR XML path('RepAux:ComprNal'), type
)
 return @cncp
end
go


IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnDocNacFolio()'
ELSE PRINT 'Error en la creación de la función: DcemFcnDocNacFolio()'
GO

