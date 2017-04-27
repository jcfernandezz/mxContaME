IF OBJECT_ID ('dbo.DcemFcnOtro') IS NOT NULL
   DROP FUNCTION dbo.[DcemFcnOtro]
GO

create function [dbo].[DcemFcnOtro](@ORCTRNUM varchar(21), @ORTRXTYP smallint, @SOURCDOC VARCHAR(11))
returns xml 
as
--Propósito. Otro tipo de pago
--12/2014 jmg Creación
--7/4/15 jcf Agrega filtro @ORTRXTYP
--22/4/15 jcf SAT v2 no requiere cobros en póliza
--
begin
 declare @cncp xml;
  WITH XMLNAMESPACES
(
    'www.sat.gob.mx/esquemas/ContabilidadE/1_1/PolizasPeriodo' as "PLZ"
)
 
 select @cncp = (
		select 
			   dbo.DcemFcnReplace(rtrim(codMetodoPago))   '@MetPagoPol',
			   dbo.DcemFcnReplace(cast(docdate as date))  '@Fecha',
			   dbo.DcemFcnReplace(rtrim(beneficiariosat)) '@Benef',
			   dbo.DcemFcnReplace(rtrim(txrgnnum))        '@RFC',
				cast(docamnt as numeric(16,2))            '@Monto',
			   dbo.DcemFcnReplace(rtrim(ISOCURRC))        '@Moneda',
			   cast(xchgrate as numeric(19,5))            '@TipCamb'
		  from dbo.dcemFnGetDocumentoOriginal(@ORCTRNUM, @ORTRXTYP, @SOURCDOC)
		where codMetodoPago not in( '02', '03')
		and voided = 0
		and (
			(@SOURCDOC in ('PMPAY', 'PMCHK', 'PMTRX')
			and @ORTRXTYP = 6)
			--or
			--(@SOURCDOC in ('CRJ', 'RMJ', 'SJ')
			--and @ORTRXTYP = 9)
			)
		FOR XML path('PLZ:OtrMetodoPago'), type
)
 return @cncp
end

go
IF (@@Error = 0) PRINT 'Creación exitosa de la función: DcemFcnOtro()'
ELSE PRINT 'Error en la creación de la función: DcemFcnOtro()'
GO
