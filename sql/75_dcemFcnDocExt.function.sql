IF OBJECT_ID ('dbo.DcemFcnDocExt') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnDocExt]
GO

create function [dbo].[DcemFcnDocExt](
									@ORCTRNUM varchar(21), @ORTRXTYP smallint, @SOURCDOC VARCHAR(11), @SERIES smallint)
returns xml 
as
--Propósito. Comprobante de compra o venta extranjera
--02/2015 jmg Creación
--7/4/15 jcf Replanteo de consulta. Debe obtener resultados sólo si es factura 
--
begin
 declare @cncp xml;
  WITH XMLNAMESPACES
(
    'www.sat.gob.mx/esquemas/ContabilidadE/1_1/PolizasPeriodo' as "PLZ"
)
 
 select @cncp = (
		SELECT
			rtrim(ltrim(do.docnumbr))				'@NumFactExt', 
			dbo.DcemFcnReplace(rtrim(ltrim(left(do.txrgnnum, 30)))) '@TaxID',
			cast(do.docamnt as numeric(16,2))		'@MontoTotal',
			rtrim(do.ISOCURRC)						'@Moneda' ,
			cast(do.xchgrate as numeric(19,5))		'@TipCamb' 
		from dbo.dcemFnGetDocumentoOriginal(@ORCTRNUM, @ORTRXTYP, @SOURCDOC) do
		WHERE do.ccode not in ('MX', '')
			and (
				(@SERIES = 4
				and @SOURCDOC in ('PMTRX', 'RECVG')
				and @ORTRXTYP <= 5)
				or
				(@SERIES = 3
				and @SOURCDOC in ('SJ')
				and @ORTRXTYP <= 7)
				)
		FOR XML path('PLZ:CompExt'), type
)
 return @cncp
end

GO

IF (@@Error = 0) PRINT 'Creación exitosa de la función: [DcemFcnDocExt]()'
ELSE PRINT 'Error en la creación de la función: [DcemFcnDocExt]()'
GO
